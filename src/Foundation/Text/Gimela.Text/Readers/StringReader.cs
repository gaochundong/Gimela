using System;

namespace Gimela.Text
{
  /// <summary>
  /// 字符串读取器
  /// </summary>
  /// <example>
  /// new StringReader("a = \"a1\"; b = \"b2\"");
  /// </example>
  public class StringReader : ITextReader
  {
    #region Fields
    
    private string _buffer;

    #endregion

    #region Ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringReader"/> class.
    /// </summary>
    /// <param name="buffer">Buffer to process.</param>
    public StringReader(string buffer)
    {
      Assign(buffer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringReader"/> class.
    /// </summary>
    public StringReader()
    {
    }

    #endregion

    #region ITextReader Members

    /// <summary>
    /// Gets or sets line number.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets if end of buffer have been reached
    /// </summary>
    /// <value></value>
    public bool EOF
    {
      get { return Index >= Length; }
    }

    /// <summary>
    /// Gets if more bytes can be processed.
    /// </summary>
    /// <value></value>
    public bool HasMore
    {
      get { return Index < Length; }
    }

    /// <summary>
    /// Gets next character
    /// </summary>
    /// <value><see cref="char.MinValue"/> if end of buffer.</value>
    public char Peek
    {
      get { return Index < Length - 1 ? _buffer[Index + 1] : char.MinValue; }
    }

    /// <summary>
    /// Gets current character
    /// </summary>
    /// <value><see cref="char.MinValue"/> if end of buffer.</value>
    public char Current
    {
      get { return HasMore ? _buffer[Index] : char.MinValue; }
    }

    /// <summary>
    /// Gets or sets current position in buffer.
    /// </summary>
    /// <remarks>
    /// THINK before you manually change the position since it can blow up
    /// the whole parsing in your face.
    /// </remarks>
    public int Index { get; set; }

    /// <summary>
    /// Gets total length of buffer.
    /// </summary>
    /// <value></value>
    public int Length { get; private set; }

    /// <summary>
    /// Gets number of bytes left.
    /// </summary>
    public int RemainingLength
    {
      get { return Length - Index; }
    }

    /// <summary>
    /// Assign a new buffer
    /// </summary>
    /// <param name="buffer">Buffer to process.</param>
    /// <param name="offset">Where to start process buffer</param>
    /// <param name="length">Buffer length</param>
    /// <remarks><paramref name="buffer"/> MUST be of type <see cref="string"/>.</remarks>
    /// <exception cref="ArgumentException">buffer needs to be of type string</exception>
    public void Assign(object buffer, int offset, int length)
    {
      if (!(buffer is string))
        throw new ArgumentException("buffer needs to be of type string", "buffer");

      _buffer = (string)buffer;
      Index = offset;
      Length = length;
    }

    /// <summary>
    /// Assign a new buffer
    /// </summary>
    /// <param name="buffer">Buffer to process</param>
    /// <remarks><paramref name="buffer"/> MUST be of type <see cref="string"/>.</remarks>
    /// <exception cref="ArgumentException">buffer needs to be of type string</exception>
    public void Assign(object buffer)
    {
      if (!(buffer is string))
        throw new ArgumentException("buffer needs to be of type string", "buffer");
      _buffer = (string)buffer;
      Index = 0;
      Length = _buffer.Length;
    }

    /// <summary>
    /// Read a character.
    /// </summary>
    /// <returns>
    /// Character if not EOF; otherwise <c>null</c>.
    /// </returns>
    public char Read()
    {
      return _buffer[Index++];
    }
    
    /// <summary>
    /// Get a text line. 
    /// </summary>
    /// <returns></returns>
    /// <remarks>Will merge multiline headers.</remarks> 
    public string ReadLine()
    {
      int startIndex = Index;
      while (HasMore && Current != '\n')
        Consume();

      // EOF? Then we havent enough bytes.
      if (EOF)
      {
        Index = startIndex;
        return null;
      }

      Consume(); // eat \n too.

      string thisLine = _buffer.Substring(startIndex, Index - startIndex - 2);

      // Multi line message?
      if (Current == '\t' || Current == ' ')
      {
        Consume();
        string extra = ReadLine();

        // Multiline isn't complete, wait for more bytes.
        if (extra == null)
        {
          Index = startIndex;
          return null;
        }

        return thisLine + " " + extra.TrimStart(' ', '\t');
      }

      return thisLine;
    }

    /// <summary>
    /// Read quoted string
    /// </summary>
    /// <returns>string if current character (in buffer) is a quote; otherwise <c>null</c>.</returns>
    public string ReadQuotedString()
    {
      Consume(' ', '\t');
      if (Current != '\"')
        return null;

      int startPos = Index;
      Consume();
      string buffer = string.Empty;
      while (!EOF)
      {
        switch (Current)
        {
          case '\\':
            Consume();
            buffer += Current;
            break;
          case '"':
            Consume();
            return buffer;
          default:
            buffer += Current;
            break;
        }
        ++Index;
      }

      Index = startPos;
      return null;
    }

    /// <summary>
    /// Read until end of string, or to one of the delimiters are found.
    /// </summary>
    /// <param name="delimiters">characters to stop at</param>
    /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public string ReadToEnd(string delimiters)
    {
      if (EOF)
        return string.Empty;

      int startIndex = Index;

      bool isDelimitersNewLine = delimiters.IndexOfAny(new[] { '\r', '\n' }) != -1;
      while (true)
      {
        if (EOF)
          return GetString(startIndex, Index);

        if (delimiters.IndexOf(Current) != -1)
          return GetString(startIndex, Index, true);

        // Delimiter is not new line and we got one.
        if (isDelimitersNewLine && Current == '\r' || Current == '\n')
          throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                              "[CRLF].");

        ++Index;
      }
    }

    /// <summary>
    /// Read until end of string, or to one of the delimiters are found.
    /// </summary>
    /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
    /// <remarks>
    /// Will not consume the delimiter.
    /// </remarks>
    public string ReadToEnd()
    {
      int index = Index;
      Index = Length;
      return _buffer.Substring(index);
    }

    /// <summary>
    /// Read to end of buffer, or until specified delimiter is found.
    /// </summary>
    /// <param name="delimiter">Delimiter to find.</param>
    /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public string ReadToEnd(char delimiter)
    {
      if (EOF)
        return string.Empty;

      int startIndex = Index;

      while (true)
      {
        if (EOF)
          return GetString(startIndex, Index);

        if (Current == delimiter)
          return GetString(startIndex, Index, true);

        // Delimiter is not new line and we got one.
        if (delimiter != '\r' && delimiter != '\n' && Current == '\r' || Current == '\n')
          throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                              "[CRLF].");

        ++Index;
      }
    }

    /// <summary>
    /// Will read until specified delimiter is found.
    /// </summary>
    /// <param name="delimiter">Character to stop at.</param>
    /// <returns>
    /// A string if the delimiter was found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Will trim away spaces and tabs from the end.
    /// Will not consume the delimiter.
    /// </remarks>
    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public string ReadUntil(char delimiter)
    {
      if (EOF)
        return null;

      int startIndex = Index;

      while (true)
      {
        if (EOF)
        {
          Index = startIndex;
          return null;
        }

        if (Current == delimiter)
          return GetString(startIndex, Index, true);

        // Delimiter is not new line and we got one.
        if (delimiter != '\r' && delimiter != '\n' && Current == '\r' || Current == '\n')
          throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                              "[CRLF].");

        ++Index;
      }
    }

    /// <summary>
    /// Read until one of the delimiters are found.
    /// </summary>
    /// <param name="delimiters">characters to stop at</param>
    /// <returns>
    /// A string if one of the delimiters was found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Will not consume the delimiter.
    /// </remarks>
    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public string ReadUntil(string delimiters)
    {
      if (EOF)
        return null;

      int startIndex = Index;

      bool isDelimitersNewLine = delimiters.IndexOfAny(new[] { '\r', '\n' }) != -1;
      while (true)
      {
        if (EOF)
        {
          Index = startIndex;
          return null;
        }

        if (delimiters.IndexOf(Current) != -1)
          return GetString(startIndex, Index, true);

        // Delimiter is not new line and we got one.
        if (isDelimitersNewLine && Current == '\r' || Current == '\n')
          throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                              "[CRLF].");

        ++Index;
      }
    }

    /// <summary>
    /// Read until a horizontal white space occurs (or end, or end of line).
    /// </summary>
    /// <returns>
    /// A string if a white space was found; otherwise <c>null</c>.
    /// </returns>
    public string ReadWord()
    {
      return ReadToEnd(" \t\r\n");
    }

    /// <summary>
    /// Consume current character.
    /// </summary>
    public void Consume()
    {
      ++Index;
    }

    /// <summary>
    /// Consume specified characters
    /// </summary>
    /// <param name="chars">One or more characters.</param>
    public void Consume(params char[] chars)
    {
      while (HasMore)
      {
        bool found = false;
        foreach (var ch in chars)
        {
          if (Current != ch) continue;
          found = true;
          break;
        }
        if (!found)
          return;

        ++Index;
      }
    }

    /// <summary>
    /// Consumes horizontal white spaces (space and tab).
    /// </summary>
    public void ConsumeWhiteSpaces()
    {
      Consume('\t', ' ');
    }

    /// <summary>
    /// Consume horizontal white spaces and the specified character.
    /// </summary>
    /// <param name="extraCharacter">Extra character to consume</param>
    public void ConsumeWhiteSpaces(char extraCharacter)
    {
      Consume('\t', ' ', extraCharacter);
    }

    /// <summary>
    /// Checks if one of the remaining bytes are a specified character.
    /// </summary>
    /// <param name="ch">Character to find.</param>
    /// <returns>
    /// 	<c>true</c> if found; otherwise <c>false</c>.
    /// </returns>
    public bool Contains(char ch)
    {
      int index = Index;
      while (index < Length)
      {
        if (ch == _buffer[index])
          return true;
        ++index;
      }

      return false;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 从缓存中指定的开始和结束位置读取出字符串
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    private string GetString(int startIndex, int endIndex)
    {
      return _buffer.Substring(startIndex, endIndex - startIndex);
    }

    /// <summary>
    /// 从缓存中指定的开始和结束位置读取出字符串
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <param name="trimEnd"></param>
    /// <returns></returns>
    private string GetString(int startIndex, int endIndex, bool trimEnd)
    {
      if (trimEnd)
      {
        if (endIndex > 0)
          --endIndex; // need to move one back to be able to trim.
        while (endIndex > 0 && _buffer[endIndex] == ' ' || _buffer[endIndex] == '\t')
          --endIndex;
        ++endIndex;
      }
      return _buffer.Substring(startIndex, endIndex - startIndex);
    }

    #endregion
  }
}