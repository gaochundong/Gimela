using System.Globalization;
using System.IO;
using System.Windows.Controls;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidDirectoryAndFileExistValidationRule : ValidationRule
  {
    private string INVALID_DIRECTORY_NOT_EXIST = LanguageString.Find("Framework_ValidationRules_DirectoryNotExisted");
    private string INVALID_FILE_NOT_EXIST = LanguageString.Find("Framework_ValidationRules_FileNotExisted");
    private string INVALID_DIRECTORY_OR_FILE_NOT_EXIST = LanguageString.Find("Framework_ValidationRules_DirectoryOrFileNotExisted");

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (value == null)
          return new ValidationResult(false, INVALID_DIRECTORY_OR_FILE_NOT_EXIST);
        if (string.IsNullOrEmpty(value.ToString()))
          return new ValidationResult(false, INVALID_DIRECTORY_OR_FILE_NOT_EXIST);

        FileInfo info = new FileInfo(value.ToString());

        if (!Directory.Exists(info.Directory.FullName))
        {
          return new ValidationResult(false, INVALID_DIRECTORY_NOT_EXIST);
        }

        if (!File.Exists(info.FullName))
        {
          return new ValidationResult(false, INVALID_FILE_NOT_EXIST);
        }

        return new ValidationResult(true, null);
      }
      catch
      {
        return new ValidationResult(false, INVALID_DIRECTORY_OR_FILE_NOT_EXIST);
      }
    }
  }
}
