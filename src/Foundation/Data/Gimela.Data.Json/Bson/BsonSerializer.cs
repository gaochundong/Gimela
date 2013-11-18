using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Gimela.Data.Json.Bson
{
  internal class BsonSerializer : IDisposable
  {
    private readonly MemoryStream _stream;
    private readonly BinaryWriter _writer;
    private BsonDocument _current;

    public BsonSerializer()
    {
      IsSerializeNullValue = false;

      _stream = new MemoryStream(250);
      _writer = new BinaryWriter(_stream);
    }

    public bool IsSerializeNullValue { get; set; }

    public byte[] Serialize(object document)
    {
      var type = document.GetType();
      if (type.IsValueType || typeof(IEnumerable).IsAssignableFrom(type))
      {
        throw new BsonSerializationException("Root type must be an non-enumerable object");
      }

      WriteDocument(document);

      return _stream.ToArray();
    }

    private void WriteDocument(object document)
    {
      NewDocument();
      WriteObject(document);
      EndDocument(true);
    }

    private void NewDocument()
    {
      var old = _current;
      _current = new BsonDocument { Parent = old, Length = (int)_writer.BaseStream.Position, Digested = 4 };
      _writer.Write(0); // length placeholder
    }

    private void EndDocument(bool includeEeo)
    {
      var old = _current;
      if (includeEeo)
      {
        Written(1);
        _writer.Write((byte)0);
      }

      _writer.Seek(_current.Length, SeekOrigin.Begin);
      _writer.Write(_current.Digested); // override the document length placeholder
      _writer.Seek(0, SeekOrigin.End); // back to the end

      _current = _current.Parent;
      if (_current != null)
      {
        Written(old.Digested);
      }
    }

    private void Written(int length)
    {
      _current.Digested += length;
    }

    private void WriteObject(object document)
    {
      IDictionary<string, PropertyGetter> properties = PropertyHelper.GetTypePropertyGetters(document.GetType());
      foreach (var p in properties)
      {
        object propertyValue = p.Value(document);
        if (!(propertyValue == null && IsSerializeNullValue == false))
        {
          WriteProperty(p.Key, propertyValue);
        }
      }
    }

    private void WriteProperty(string name, object value)
    {
      if (value == null)
      {
        Write(BsonType.Null);
        WriteName(name);
        return;
      }

      var type = value.GetType();
      if (type.IsEnum)
      {
        type = Enum.GetUnderlyingType(type);
      }

      BsonType storageType;
      if (!BsonTypeMapping.SerializeTypeMapping.TryGetValue(type, out storageType))
      {
        if (value is IDictionary)
        {
          Write(BsonType.Object);
          WriteName(name);
          NewDocument();
          Write((IDictionary)value);
          EndDocument(true);
        }
        else if (value is IEnumerable)
        {
          Write(BsonType.Array);
          WriteName(name);
          NewDocument();
          Write((IEnumerable)value);
          EndDocument(true);
        }
        else
        {
          Write(BsonType.Object);
          WriteName(name);
          WriteDocument(value);
        }
        return;
      }

      Write(storageType);
      WriteName(name);
      switch (storageType)
      {
        case BsonType.Integer:
          Write((int)value);
          return;
        case BsonType.Long:
          Write((long)value);
          return;
        case BsonType.String:
          Write((string)value);
          return;
        case BsonType.Number:
          if (value is float)
            Write((float)value);
          else
            Write((double)value);
          return;
        case BsonType.Boolean:
          Write((bool)value);
          return;
        case BsonType.DateTime:
          Write((DateTime)value);
          return;
        case BsonType.Binary:
          if (value is byte[])
          {
            Write((byte[])value);
          }
          else if (value is Guid)
          {
            Write((Guid)value);
          }
          return;
        case BsonType.ObjectId:
          Write((ObjectId)value);
          return;
        case BsonType.Regex:
          Write((Regex)value);
          break;
      }
    }

    private void WriteName(string name)
    {
      var bytes = Encoding.UTF8.GetBytes(name);
      _writer.Write(bytes);
      _writer.Write((byte)0);
      Written(bytes.Length + 1);
    }

    private void Write(int value)
    {
      _writer.Write(value);
      Written(4);
    }

    private void Write(long value)
    {
      _writer.Write(value);
      Written(8);
    }

    private void Write(float value)
    {
      Write(Convert.ToDouble(value));
    }

    private void Write(double value)
    {
      _writer.Write(value);
      Written(8);
    }

    private void Write(bool value)
    {
      _writer.Write(value ? (byte)1 : (byte)0);
      Written(1);
    }

    private void Write(DateTime value)
    {
      _writer.Write((long)value.Subtract(JsonConstants.Epoch).TotalMilliseconds);
      Written(8);
    }

    private void Write(Guid guid)
    {
      var bytes = guid.ToByteArray();
      _writer.Write(bytes.Length);
      _writer.Write((byte)3);
      _writer.Write(bytes);
      Written(5 + bytes.Length);
    }

    private void Write(byte[] bytes)
    {
      var length = bytes.Length;
      _writer.Write(length + 4);
      _writer.Write((byte)2);
      _writer.Write(length);
      _writer.Write(bytes);
      Written(9 + length);
    }

    private void Write(IEnumerable enumerable)
    {
      var index = 0;
      foreach (var value in enumerable)
      {
        WriteProperty((index++).ToString(CultureInfo.InvariantCulture), value);
      }
    }

    private void Write(IDictionary dictionary)
    {
      foreach (var key in dictionary.Keys)
      {
        WriteProperty((string)key, dictionary[key]);
      }
    }

    private void Write(BsonType type)
    {
      _writer.Write((byte)type);
      Written(1);
    }

    private void Write(string value)
    {
      var bytes = Encoding.UTF8.GetBytes(value);
      _writer.Write(bytes.Length + 1);
      _writer.Write(bytes);
      _writer.Write((byte)0);
      Written(bytes.Length + 5); // stringLength + length + null byte
    }

    private void Write(Regex regex)
    {
      WriteName(regex.ToString());

      var options = string.Empty;
      if ((regex.Options & RegexOptions.ECMAScript) == RegexOptions.ECMAScript)
      {
        options = string.Concat(options, 'e');
      }

      if ((regex.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
      {
        options = string.Concat(options, 'i');
      }

      if ((regex.Options & RegexOptions.CultureInvariant) == RegexOptions.CultureInvariant)
      {
        options = string.Concat(options, 'l');
      }

      if ((regex.Options & RegexOptions.Multiline) == RegexOptions.Multiline)
      {
        options = string.Concat(options, 'm');
      }

      if ((regex.Options & RegexOptions.Singleline) == RegexOptions.Singleline)
      {
        options = string.Concat(options, 's');
      }

      options = string.Concat(options, 'u'); // all .net regex are unicode regex, therefore:
      if ((regex.Options & RegexOptions.IgnorePatternWhitespace) == RegexOptions.IgnorePatternWhitespace)
      {
        options = string.Concat(options, 'w');
      }

      if ((regex.Options & RegexOptions.ExplicitCapture) == RegexOptions.ExplicitCapture)
      {
        options = string.Concat(options, 'x');
      }

      WriteName(options);
    }

    private void Write(ObjectId value)
    {
      _writer.Write(value.Value);
      Written(value.Value.Length);
    }

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_writer != null)
        {
          _writer.Dispose();
        }
        if (_stream != null)
        {
          _stream.Dispose();
        }
      }
    }

    #endregion
  }
}
