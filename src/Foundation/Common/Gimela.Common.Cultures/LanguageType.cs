
namespace Gimela.Common.Cultures
{
  /// <summary>
  /// 语言文化类型
  /// </summary>
  /// <example>
  /// CultureInfo currentCultureInfo = CultureInfo.InstalledUICulture;
  /// 
  /// currentCultureInfo.TwoLetterISOLanguageName.ToLower() == @"ar"
  /// 
  /// string ietf = currentCultureInfo.IetfLanguageTag.ToLower();
  /// 
  /// if (ietf == @"zh-hk" || ietf == @"zh-mo" || ietf == @"zh-tw" || ietf == @"zh-hant")
  /// {
  ///     culture = @"zh-TW"; // 中文（繁体）
  /// }
  /// else
  /// {
  ///     culture = @"zh-CN"; // 中文（简体）
  /// }
  /// </example>
  /// <![CDATA[
  /// ms-help://MS.MSDNQTR.v90.chs/fxref_mscorlib/html/63c619e3-0969-2f01-a2d4-79d0868a98c6.htm
  /// ms-help://MS.MSDNQTR.v90.chs/fxref_mscorlib/html/48b72120-c19b-fdeb-8deb-418fd323b0f8.htm
  /// ms-help://MS.MSDNQTR.v90.chs/wpf_conceptual/html/6896d0ce-74f7-420a-9ab4-de9bbf390e8d.htm
  /// ms-help://MS.MSDNQTR.v90.chs/dv_vscmds/html/3a3f4e70-ea66-4351-9d62-acb1dec30e8e.htm
  /// http://www.ietf.org/rfc/rfc4646.txt
  /// http://zh.wikipedia.org/wiki/ISO_3166-1
  /// http://zh.wikipedia.org/wiki/ISO_639-1%E4%BB%A3%E7%A0%81%E8%A1%A8
  /// ms-help://MS.MSDNQTR.v90.chs/wpf_conceptual/html/6896d0ce-74f7-420a-9ab4-de9bbf390e8d.htm
  /// ]]>
  public enum LanguageType
  {
    /// <summary>
    /// ar ara 阿拉伯语 Arabic
    /// </summary>
    Arabic,
    /// <summary>
    /// ko kor 朝鲜语 Korean 
    /// </summary>
    Korean,
    /// <summary>
    /// de deu 德语 German 
    /// </summary>
    German,
    /// <summary>
    /// ru rus 俄语 Russian 
    /// </summary>
    Russian,
    /// <summary>
    /// fr fra 法语 French 
    /// </summary>
    French,
    /// <summary>
    /// la lat 拉丁语 Latin 
    /// </summary>
    Latin,
    /// <summary>
    /// pt por 葡萄牙语 Portuguese
    /// </summary>
    Portuguese,
    /// <summary>
    /// ja jpn 日语 Japanese
    /// </summary>
    Japanese,
    /// <summary>
    /// es spa 西班牙语 Spanish 
    /// </summary>
    Spanish,
    /// <summary>
    /// it ita 意大利语 Italian 
    /// </summary>
    Italian,
    /// <summary>
    /// en eng 英语 English 
    /// </summary>
    English,
    /// <summary>
    /// zh zho 汉语 Chinese 
    /// </summary>
    Chinese,
  }
}
