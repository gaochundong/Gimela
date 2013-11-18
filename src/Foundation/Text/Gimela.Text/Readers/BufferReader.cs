using System;
using System.Text;

namespace Gimela.Text
{
  /// <summary>
  /// 从Byte数组中读取字符串
  /// </summary>
  /// <example>
  /// ASCIIEncoding encoding = new ASCIIEncoding();
  /// new BufferReader(encoding.GetBytes("a = \"a1\"; b = \"b2\""), encoding);
  /// </example>
  public class BufferReader : ITextReader
  {
    #region Fields
    
    private readonly Encoding _encoding;
    private byte[] _buffer;

    #endregion

    #region Ctors

    /// <summary>
    /// 从Byte数组中读取字符串
    /// </summary>
    public BufferReader()
    {
      _encoding = Encoding.Default;
    }

    /// <summary>
    /// 从Byte数组中读取字符串
    /// </summary>
    /// <param name="encoding">指定编码方式</param>
    public BufferReader(Encoding encoding)
    {
      _encoding = encoding;
    }

    /// <summary>
    /// 从Byte数组中读取字符串
    /// </summary>
    /// <param name="buffer">指定缓存数组</param>
    /// <param name="encoding">指定编码方式</param>
    public BufferReader(byte[] buffer, Encoding encoding)
    {
      _buffer = buffer;
      Length = buffer.Length;
      _encoding = encoding;
    }

    #endregion

    #region ITextReader Members

    /// <summary>
    /// 获取或设置解析出的行号
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// 获取是否已读取到缓存的尾部
    /// </summary>
    /// <value></value>
    public bool EOF
    {
      get { return Index >= Length; }
    }

    /// <summary>
    /// 获取是否仍有缓存可供读取
    /// </summary>
    /// <value></value>
    public bool HasMore
    {
      get { return Index < Length; }
    }

    /// <summary>
    /// 获取下一个字符
    /// </summary>
    /// <value><see cref="char.MinValue"/> if end of buffer.</value>
    public char Peek
    {
      get { return Index < Length - 1 ? (char)_buffer[Index + 1] : char.MinValue; }
    }

    /// <summary>
    /// 获取当前字符
    /// </summary>
    /// <value><see cref="char.MinValue"/> if end of buffer.</value>
    public char Current
    {
      get { return HasMore ? (char)_buffer[Index] : char.MinValue; }
    }

    /// <summary>
    /// 获取或设置当前在缓存中读取到的位置
    /// </summary>
    /// <remarks>
    /// THINK before you manually change the position since it can blow up
    /// the whole parsing in your face.
    /// </remarks>
    public int Index { get; set; }

    /// <summary>
    /// 获取要在缓存中处理的Byte长度
    /// </summary>
    /// <value></value>
    public int Length { get; private set; }

    /// <summary>
    /// 获取还未读取的缓存长度
    /// </summary>
    public int RemainingLength
    {
      get { return Length - Index; }
    }
    
    /// <summary>
    /// 指定新的缓存数组
    /// </summary>
    /// <param name="buffer">指定缓存数组</param>
    /// <param name="offset">在缓存中开始处理的位置</param>
    /// <param name="length">要处理的Byte数量</param>
    /// <exception cref="ArgumentException">Buffer needs to be a byte array</exception>
    public void Assign(object buffer, int offset, int length)
    {
      if (!(buffer is byte[]))
        throw new ArgumentException("Buffer needs to be a byte array", "buffer");

      _buffer = (byte[])buffer;
      Index = offset;
      Length = length;
    }

    /// <summary>
    /// 指定新的缓存数组
    /// </summary>
    /// <param name="buffer">指定缓存数组</param>
    /// <exception cref="ArgumentException">Buffer needs to be a byte array</exception>
    public void Assign(object buffer)
    {
      if (!(buffer is byte[]))
        throw new ArgumentException("Buffer needs to be a byte array", "buffer");

      _buffer = (byte[])buffer;
      Index = 0;
      Length = _buffer.Length;
    }

    /// <summary>
    /// 读取一个字符
    /// </summary>
    /// <returns>
    /// Character if not EOF; otherwise <c>null</c>.
    /// </returns>
    public char Read()
    {
      return (char)_buffer[Index++];
    }

    /// <summary>
    /// 获取一行文本
    /// </summary>
    /// <returns></returns>
    /// <remarks>Will merge multi line headers.</remarks> 
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

      string thisLine = _encoding.GetString(_buffer, startIndex, Index - startIndex - 2);

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
    /// 读取引用符号字符串
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
            break;
          case '"':
            Consume();
            return buffer;
          default:
            buffer += Current;
            Consume();
            break;
        }
      }

      Index = startPos;
      return null;
    }

    /// <summary>
    /// 读取缓存直到抵达字符串的末尾，或者匹配到指定的分隔符
    /// </summary>
    /// <param name="delimiters">characters to stop at</param>
    /// <returns>
    /// A string (can be <see cref="string.Empty"/>).
    /// </returns>
    /// <remarks>
    /// Will not consume the delimiter.
    /// </remarks>
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
    /// 读取缓存直到抵达字符串的末尾，或者匹配到指定的分隔符
    /// </summary>
    /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
    /// <remarks>
    /// Will not consume the delimiter.
    /// </remarks>
    public string ReadToEnd()
    {
      int index = Index;
      Index = Length;
      return _encoding.GetString(_buffer, index, Length - index);
    }

    /// <summary>
    /// 读取缓存直到抵达字符串的末尾，或者匹配到指定的分隔符
    /// </summary>
    /// <param name="delimiter">Delimiter to find.</param>
    /// <returns>
    /// A string (can be <see cref="string.Empty"/>).
    /// </returns>
    /// <remarks>
    /// Will not consume the delimiter.
    /// </remarks>
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
    /// 读取缓存直到匹配到指定的字符
    /// </summary>
    /// <param name="delimiter">Character to stop at.</param>
    /// <returns>
    /// A string if the delimiter was found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Will trim away spaces and tabs from the end.</remarks>
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
    /// 读取缓存直到匹配到指定的字符串
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
    /// 读取一个单词，通过单词后的空格或TAB键判断
    /// </summary>
    /// <returns>A string if a white space was found; otherwise <c>null</c>.</returns>
    public string ReadWord()
    {
      return ReadUntil(" \t");
    }

    /// <summary>
    /// 越过当前字符
    /// </summary>
    public void Consume()
    {
      ++Index;
    }

    /// <summary>
    /// 越过指定的字符数组
    /// </summary>
    /// <param name="chars">One or more characters.</param>
    public void Consume(params char[] chars)
    {
      while (HasMore)
      {
        foreach (var ch in chars)
        {
          if (ch == Current)
            ++Index;
          else
            return;
        }
      }
    }

    /// <summary>
    /// 越过空格或TAB键
    /// </summary>
    public void ConsumeWhiteSpaces()
    {
      Consume(' ', '\t');
    }

    /// <summary>
    /// 越过空格或指定的字符
    /// </summary>
    /// <param name="extraCharacter">Extra character to consume</param>
    public void ConsumeWhiteSpaces(char extraCharacter)
    {
      Consume(' ', '\t', extraCharacter);
    }

    /// <summary>
    /// 检查遗留的未读缓存中是否包含指定的字符
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
        ++index;
        if (ch == _buffer[index])
          return true;
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
      return _encoding.GetString(_buffer, startIndex, endIndex - startIndex);
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
        --endIndex; // need to move one back to be able to trim.
        while (endIndex > 0 && _buffer[endIndex] == ' ' || _buffer[endIndex] == '\t')
          --endIndex;
        ++endIndex;
      }
      return _encoding.GetString(_buffer, startIndex, endIndex - startIndex);
    }

    #endregion
  }
}