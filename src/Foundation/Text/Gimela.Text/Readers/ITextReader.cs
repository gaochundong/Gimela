namespace Gimela.Text
{
  /// <summary>
  /// 从不同的源读取字符串标记的接口
  /// </summary>
  public interface ITextReader
  {
    /// <summary>
    /// 获取或设置解析出的行号
    /// </summary>
    int LineNumber { get; set; }

    /// <summary>
    /// 获取是否已读取到缓存的尾部
    /// </summary>
    /// <value></value>
    bool EOF { get; }

    /// <summary>
    /// 获取是否仍有缓存可供读取
    /// </summary>
    /// <value></value>
    bool HasMore { get; }

    /// <summary>
    /// 获取下一个字符
    /// </summary>
    /// <value><see cref="char.MinValue"/> if end of buffer.</value>
    char Peek { get; }

    /// <summary>
    /// 获取当前字符
    /// </summary>
    /// <value><see cref="char.MinValue"/> if end of buffer.</value>
    char Current { get; }

    /// <summary>
    /// 获取或设置当前在缓存中读取到的位置
    /// </summary>
    /// <remarks>
    /// THINK before you manually change the position since it can blow up
    /// the whole parsing in your face.
    /// </remarks>
    int Index { get; set; }

    /// <summary>
    /// 获取要在缓存中处理的Byte长度
    /// </summary>
    /// <value></value>
    int Length { get; }

    /// <summary>
    /// 获取还未读取的缓存长度
    /// </summary>
    int RemainingLength { get; }

    /// <summary>
    /// 指定新的缓存数组
    /// </summary>
    /// <param name="buffer">指定缓存数组</param>
    /// <param name="offset">在缓存中开始处理的位置</param>
    /// <param name="length">要处理的Byte数量</param>
    void Assign(object buffer, int offset, int length);

    /// <summary>
    /// 指定新的缓存数组
    /// </summary>
    /// <param name="buffer">指定缓存数组</param>
    void Assign(object buffer);

    /// <summary>
    /// 读取一个字符
    /// </summary>
    /// <returns>
    /// Character if not EOF; otherwise <c>null</c>.
    /// </returns>
    char Read();

    /// <summary>
    /// 获取一行文本
    /// </summary>
    /// <returns></returns>
    /// <remarks>Will merge multi line headers.</remarks> 
    string ReadLine();

    /// <summary>
    /// 读取引用符号字符串
    /// </summary>
    /// <returns>string if current character (in buffer) is a quote; otherwise <c>null</c>.</returns>
    string ReadQuotedString();

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
    string ReadToEnd(string delimiters);

    /// <summary>
    /// 读取缓存直到抵达字符串的末尾，或者匹配到指定的分隔符
    /// </summary>
    /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
    /// <remarks>
    /// Will not consume the delimiter.
    /// </remarks>
    string ReadToEnd();

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
    string ReadToEnd(char delimiter);

    /// <summary>
    /// 读取缓存直到匹配到指定的字符
    /// </summary>
    /// <param name="delimiter">Character to stop at.</param>
    /// <returns>
    /// A string if the delimiter was found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Will trim away spaces and tabs from the end.</remarks>
    string ReadUntil(char delimiter);

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
    string ReadUntil(string delimiters);

    /// <summary>
    /// 读取一个单词，通过单词后的空格或TAB键判断
    /// </summary>
    /// <returns>A string if a white space was found; otherwise <c>null</c>.</returns>
    string ReadWord();

    /// <summary>
    /// 越过当前字符
    /// </summary>
    void Consume();

    /// <summary>
    /// 越过指定的字符数组
    /// </summary>
    /// <param name="chars">One or more characters.</param>
    void Consume(params char[] chars);

    /// <summary>
    /// 越过空格或TAB键
    /// </summary>
    void ConsumeWhiteSpaces();

    /// <summary>
    /// 越过空格或指定的字符
    /// </summary>
    /// <param name="extraCharacter">Extra character to consume</param>
    void ConsumeWhiteSpaces(char extraCharacter);

    /// <summary>
    /// 检查遗留的未读缓存中是否包含指定的字符
    /// </summary>
    /// <param name="ch">Character to find.</param>
    /// <returns>
    /// 	<c>true</c> if found; otherwise <c>false</c>.
    /// </returns>
    bool Contains(char ch);
  }
}