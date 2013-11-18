using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Gimela.Data.Json.Bson
{
  internal class BsonDeserializer : IDisposable
  {
    private readonly MemoryStream _stream;
    private readonly BinaryReader _reader;
    private BsonDocument _current;

    public BsonDeserializer()
    {
      _stream = new MemoryStream();
      _reader = new BinaryReader(_stream);
    }

    public T Deserialize<T>(byte[] bson) where T : class
    {
      _stream.Write(bson, 0, bson.Length);
      _stream.Position = 0;

      return ReadDocument<T>();
    }

    private T ReadDocument<T>()
    {
      NewDocument(_reader.ReadInt32());
      return (T)ReadObject(typeof(T));
    }

    private void NewDocument(int length)
    {
      var old = _current;
      _current = new BsonDocument { Length = length, Parent = old, Digested = 4 };
    }

    private object Parse(Property property, BsonType storageType)
    {
      if (storageType == BsonType.Null)
      {
        return null;
      }
      else if (storageType == BsonType.Binary)
      {
        return ReadBinary();
      }

      if (property.IsString)
      {
        return ReadString();
      }
      else if (property.IsInt)
      {
        return ReadInteger(storageType);
      }
      else if (property.IsEnum)
      {
        return ReadEnum(property.Type, storageType);
      }
      else if (property.IsBool)
      {
        Consume(1);
        return _reader.ReadBoolean();
      }
      else if (property.IsDateTime)
      {
        return JsonConstants.Epoch.AddMilliseconds(ReadLong(BsonType.Long));
      }
      else if (property.IsLong)
      {
        return ReadLong(storageType);
      }
      else if (property.IsValueType && property.ValueType == typeof(float))
      {
        Consume(8);
        return (float)_reader.ReadDouble();
      }
      else if (property.IsValueType && property.ValueType == typeof(double))
      {
        Consume(8);
        return _reader.ReadDouble();
      }
      else if (property.IsClass && property.Type == typeof(Regex))
      {
        return ReadRegex();
      }
      else if (property.IsClass && property.Type == typeof(ObjectId))
      {
        Consume(12);
        return new ObjectId(_reader.ReadBytes(12));
      }
      else if (property.IsClass && property.IsStringDictionary)
      {
        return ReadDictionary(property);
      }
      else if (property.IsClass && property.IsDictionary)
      {
        return ReadDictionary(property);
      }
      else if (property.IsClass && property.IsGenericType && typeof(IList).IsAssignableFrom(property.Type))
      {
        return ReadList(property);
      }

      return ReadObject(property.Type);
    }

    private bool IsDone()
    {
      var isDone = _current.Digested + 1 == _current.Length;
      if (isDone)
      {
        _reader.ReadByte(); // EOO
        var old = _current;
        _current = old.Parent;
        if (_current != null) { Consume(old.Length); }
      }
      return isDone;
    }

    private void Consume(int read)
    {
      _current.Digested += read;
    }

    private BsonType ReadType()
    {
      Consume(1);
      return (BsonType)_reader.ReadByte();
    }

    private string ReadName()
    {
      var buffer = new List<byte>(128); //todo: use a pool to prevent fragmentation
      byte b;
      while ((b = _reader.ReadByte()) > 0)
      {
        buffer.Add(b);
      }
      Consume(buffer.Count + 1);
      return Encoding.UTF8.GetString(buffer.ToArray());
    }

    private object ReadObject(Type objectType)
    {
      var instance = Activator.CreateInstance(objectType);
      IDictionary<string, Property> typeProperties = PropertyHelper.GetTypeProperties(objectType);

      while (true)
      {
        var storageType = ReadType();
        var name = ReadName();
        var isNull = false;

        if (storageType == BsonType.Object)
        {
          var length = _reader.ReadInt32();
          if (length == 5)
          {
            _reader.ReadByte(); // eoo
            Consume(5);
            isNull = true;
          }
          else
          {
            NewDocument(length);
          }
        }

        Property property;
        if (typeProperties.TryGetValue(name, out property) == false)
        {
          throw new BsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Type {0} does not have a property named {1}.", objectType.FullName, name));
        }

        var value = isNull ? null : Parse(property, storageType);

        property.Setter(instance, value);

        if (IsDone())
        {
          break;
        }
      }

      return instance;
    }

    private string ReadString()
    {
      var length = _reader.ReadInt32();
      var buffer = _reader.ReadBytes(length - 1); 
      _reader.ReadByte();
      Consume(4 + length);

      return Encoding.UTF8.GetString(buffer);
    }

    private int ReadInteger(BsonType storageType)
    {
      switch (storageType)
      {
        case BsonType.Integer:
          Consume(4);
          return _reader.ReadInt32();
        case BsonType.Long:
          Consume(8);
          return (int)_reader.ReadInt64();
        case BsonType.Number:
          Consume(8);
          return (int)_reader.ReadDouble();
        default:
          throw new BsonSerializationException("Could not create an int from " + storageType);
      }
    }

    private long ReadLong(BsonType storageType)
    {
      switch (storageType)
      {
        case BsonType.Integer:
          Consume(4);
          return _reader.ReadInt32();
        case BsonType.Long:
          Consume(8);
          return _reader.ReadInt64();
        case BsonType.Number:
          Consume(8);
          return (long)_reader.ReadDouble();
        default:
          throw new BsonSerializationException("Could not create an long from " + storageType);
      }
    }

    private object ReadEnum(Type type, BsonType storageType)
    {
      if (storageType == BsonType.Long)
      {
        return Enum.Parse(type, ReadLong(storageType).ToString(CultureInfo.InvariantCulture), false);
      }
      return Enum.Parse(type, ReadInteger(storageType).ToString(CultureInfo.InvariantCulture), false);
    }

    private object ReadRegex()
    {
      var pattern = ReadName();
      var optionsString = ReadName();

      var options = RegexOptions.None;
      if (optionsString.Contains("e")) options = options | RegexOptions.ECMAScript;
      if (optionsString.Contains("i")) options = options | RegexOptions.IgnoreCase;
      if (optionsString.Contains("l")) options = options | RegexOptions.CultureInvariant;
      if (optionsString.Contains("m")) options = options | RegexOptions.Multiline;
      if (optionsString.Contains("s")) options = options | RegexOptions.Singleline;
      if (optionsString.Contains("w")) options = options | RegexOptions.IgnorePatternWhitespace;
      if (optionsString.Contains("x")) options = options | RegexOptions.ExplicitCapture;

      return new Regex(pattern, options);
    }

    private object ReadList(Property property)
    {
      NewDocument(_reader.ReadInt32());

      IList list = (IList)Activator.CreateInstance(property.Type);

      while (!IsDone())
      {
        var storageType = ReadType();
        ReadName();

        if (storageType == BsonType.Object)
        {
          NewDocument(_reader.ReadInt32());
          var value = ReadObject(property.ElementType);
          list.Add(value);
        }
        else
        {
          var value = Parse(PropertyHelper.ConvertTypeToProperty(property.ElementType), storageType);
          list.Add(value);
        }
      }

      return list;
    }

    private object ReadDictionary(Property property)
    {
      IDictionary dict = (IDictionary)Activator.CreateInstance(property.Type);

      while (!IsDone())
      {
        var storageType = ReadType();
        var key = ReadName();

        if (storageType == BsonType.Object)
        {
          NewDocument(_reader.ReadInt32());
          var value = ReadObject(property.GenericTypes[1]);
          dict.Add(key, value);
        }
        else
        {
          var value = Parse(PropertyHelper.ConvertTypeToProperty(property.GenericTypes[1]), storageType);
          dict.Add(key, value);
        }
      }

      return dict;
    }

    private object ReadBinary()
    {
      var length = _reader.ReadInt32();
      var subType = _reader.ReadByte();

      Consume(5 + length);

      if (subType == 2)
      {
        return _reader.ReadBytes(_reader.ReadInt32());
      }
      if (subType == 3)
      {
        return new Guid(_reader.ReadBytes(length));
      }

      throw new BsonSerializationException("No support for binary type: " + subType);
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
        if (_reader != null)
        {
          _reader.Dispose();
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
