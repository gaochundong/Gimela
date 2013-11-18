using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gimela.Data.Json
{
  internal class JsonSerializer
  {
    #region Fields

    private readonly StringBuilder _builder;
    private int _currentDepth = 0;

    #endregion

    #region Ctor

    public JsonSerializer()
    {
      _builder = new StringBuilder();
      IsSerializeExtension = false;
      IsSerializeNullValue = false;
      IsOutputIndented = false;
    }

    #endregion

    #region Properties

    public bool IsSerializeExtension { get; set; }
    public bool IsSerializeNullValue { get; set; }
    public bool IsOutputIndented { get; set; }

    #endregion

    #region Serialize

    public string Serialize(object target)
    {
      _builder.Clear();
      Parse(target);
      return _builder.ToString();
    }

    private void Parse(object target)
    {
      if (target == null)
      {
        WriteNull();
      }
      else if (target is string || target is char)
      {
        WriteString((string)target);
      }
      else if (target is Guid)
      {
        WriteGuid((Guid)target);
      }
      else if (target is bool)
      {
        WriteBoolean((bool)target);
      }
      else if (target is int || target is long || target is double || target is decimal || target is float || target is byte || target is short || target is sbyte || target is ushort || target is uint || target is ulong)
      {
        WriteNumber(((IConvertible)target));
      }
      else if (target is DateTime)
      {
        WriteDateTime((DateTime)target);
      }
      else if (target is Enum)
      {
        WriteEnum((Enum)target);
      }
      else if (target is byte[])
      {
        WriteBytes((byte[])target);
      }
      else if (target is IDictionary<string, string>)
      {
        WriteStringDictionary((IDictionary)target);
      }
      else if (target is IDictionary)
      {
        WriteDictionary((IDictionary)target);
      }
      else if (target is Array || target is IList || target is ICollection)
      {
        WriteArray((IEnumerable)target);
      }
      else
      {
        WriteObject(target);
      }
    }

    #endregion

    #region Write Methods

    private void WriteNull()
    {
      _builder.Append(JsonConstants.Null);
    }

    private void WriteString(string @string)
    {
      _builder.Append('\"');

      int runIndex = -1;
      for (var index = 0; index < @string.Length; ++index)
      {
        var c = @string[index];

        if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
        {
          if (runIndex == -1)
          {
            runIndex = index;
          }
          continue;
        }
        if (runIndex != -1)
        {
          _builder.Append(@string, runIndex, index - runIndex);
          runIndex = -1;
        }

        switch (c)
        {
          case '\t': _builder.Append("\\t"); break;
          case '\r': _builder.Append("\\r"); break;
          case '\n': _builder.Append("\\n"); break;
          case '"':
          case '\\': _builder.Append('\\'); _builder.Append(c); break;
          default:
            _builder.Append("\\u");
            _builder.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
            break;
        }
      }

      if (runIndex != -1)
      {
        _builder.Append(@string, runIndex, @string.Length - runIndex);
      }

      _builder.Append('\"');
    }

    private void WriteGuid(Guid guid)
    {
      WriteDirectString(guid.ToString());
    }

    private void WriteBoolean(bool @boolean)
    {
      _builder.Append(@boolean ? JsonConstants.True : JsonConstants.False);
    }

    private void WriteNumber(IConvertible number)
    {
      _builder.Append(number.ToString(NumberFormatInfo.InvariantInfo));
    }

    private void WriteDateTime(DateTime dateTime)
    {
      _builder.Append(DateTimeHelper.WriteDateTimeString(dateTime));
    }

    private void WriteEnum(Enum @enum)
    {
      WriteDirectString(@enum.ToString());
    }

    private void WriteBytes(byte[] bytes)
    {
      WriteDirectString(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
    }

    private void WriteStringDictionary(IDictionary dictionary)
    {
      _builder.Append("{");

      _currentDepth++;

      bool pendingSeparator = false;
      foreach (DictionaryEntry entry in dictionary)
      {
        if (pendingSeparator) _builder.Append(',');

        WritePair((string)entry.Key, (string)entry.Value);

        pendingSeparator = true;
      }

      _currentDepth--;

      WriteIndent();
      _builder.Append('}');
    }

    private void WriteDictionary(IDictionary dictionary)
    {
      _builder.Append("{");

      _currentDepth++;

      bool pendingSeparator = false;
      foreach (DictionaryEntry entry in dictionary)
      {
        if (pendingSeparator) _builder.Append(',');

        WritePair((string)entry.Key, entry.Value);

        pendingSeparator = true;
      }

      _currentDepth--;

      WriteIndent();
      _builder.Append('}');
    }

    private void WriteArray(IEnumerable array)
    {
      _builder.Append("[");

      _currentDepth++;

      bool pendingSeperator = false;
      foreach (object obj in array)
      {
        if (pendingSeperator)
        {
          _builder.Append(',');
        }

        WriteIndent();
        Parse(obj);
        
        pendingSeperator = true;
      }

      _currentDepth--;

      WriteIndent();
      _builder.Append(']');
    }

    private void WriteObject(object @object)
    {
      _builder.Append("{");

      _currentDepth++;

      Type objectType = @object.GetType();

      bool appendComma = false;
      if (IsSerializeExtension)
      {
        WritePair("$type", objectType.AssemblyQualifiedName);
        appendComma = true;
      }

      IDictionary<string, PropertyGetter> properties = PropertyHelper.GetTypePropertyGetters(objectType);
      foreach (var p in properties)
      {
        if (appendComma)
          _builder.Append(',');

        object propertyValue = p.Value(@object);
        if (propertyValue == null && IsSerializeNullValue == false)
        {
          appendComma = false;
        }
        else
        {
          WritePair(p.Key, propertyValue);
          appendComma = true;
        }
      }

      _currentDepth--;

      WriteIndent();
      _builder.Append('}');
    }

    private void WriteIndent()
    {
      if (IsOutputIndented)
      {
        _builder.Append("\r\n");
        for (int i = 0; i < _currentDepth; i++)
        {
          _builder.Append("  ");
        }
      }
    }

    private void WritePair(string key, string value)
    {
      if (value == null && IsSerializeNullValue == false)
        return;

      WriteIndent();
      WriteDirectString(key);

      if (IsOutputIndented)
      {
        _builder.Append(": ");
      }
      else
      {
        _builder.Append(":");
      }

      WriteDirectString(value);
    }

    private void WritePair(string key, object value)
    {
      if (value == null && IsSerializeNullValue == false)
        return;

      WriteIndent();
      WriteDirectString(key);

      if (IsOutputIndented)
      {
        _builder.Append(": ");
      }
      else
      {
        _builder.Append(":");
      }

      Parse(value);
    }

    private void WriteDirectString(string @string)
    {
      _builder.Append('\"');
      _builder.Append(@string);
      _builder.Append('\"');
    }

    #endregion
  }
}
