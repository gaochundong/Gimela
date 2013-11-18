using Gimela.Crust;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Cultures;
using Gimela.Presentation.Windows;

namespace Gimela.Rukbat.GUI.Controls
{
  public static class MessageBoxWindowHelper
  {
    public static bool? Show(string message, string detail)
    {
      return Show(message, detail, MessageBoxWindowType.Information, MessageBoxWindowButtonsType.Ok, LanguageString.Find("Presentation_Windows_MessageBoxWindow_Information"));
    }

    public static bool? Show(string message, string detail, MessageBoxWindowType windowType, string title)
    {
      return Show(message, detail, windowType, MessageBoxWindowButtonsType.Ok, title);
    }

    public static bool? Show(string message, string detail, MessageBoxWindowType windowType, MessageBoxWindowButtonsType buttonType, string title)
    {
      MessageBoxWindow window = new MessageBoxWindow();

      window.Title = title;
      window.Message = message;
      window.Detail = detail;
      window.WindowType = windowType;
      window.ButtonsType = buttonType;

      return window.ShowDialog();
    }

    public static bool? Show(string message, string detail, MessageBoxWindowType windowType, MessageBoxWindowButtonsType buttonType)
    {
      MessageBoxWindow window = new MessageBoxWindow();

      window.Message = message;
      window.Detail = detail;
      window.WindowType = windowType;
      window.ButtonsType = buttonType;

      switch (windowType)
      {
        case MessageBoxWindowType.Information:
          window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Information");
          break;
        case MessageBoxWindowType.Question:
          window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Question");
          break;
        case MessageBoxWindowType.Warning:
          window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Warning");
          break;
        case MessageBoxWindowType.Error:
          window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Error");
          break;
        default:
          window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Information");
          break;
      }

      return window.ShowDialog();
    }

    public static bool? ShowQuestionOkCancel(string message, string detail)
    {
      MessageBoxWindow window = new MessageBoxWindow();

      window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Question");
      window.Message = message;
      window.Detail = detail;
      window.WindowType = MessageBoxWindowType.Information;
      window.ButtonsType = MessageBoxWindowButtonsType.OkCancel;

      return window.ShowDialog();
    }

    public static bool? ShowQuestionYesNo(string message, string detail)
    {
      MessageBoxWindow window = new MessageBoxWindow();

      window.Title = LanguageString.Find("Presentation_Windows_MessageBoxWindow_Question");
      window.Message = message;
      window.Detail = detail;
      window.WindowType = MessageBoxWindowType.Information;
      window.ButtonsType = MessageBoxWindowButtonsType.YesNo;

      return window.ShowDialog();
    }

    public static bool? ShowInformation(string message, string detail)
    {
      return Show(message, detail, MessageBoxWindowType.Information, MessageBoxWindowButtonsType.Ok, LanguageString.Find("Presentation_Windows_MessageBoxWindow_Information"));
    }

    public static bool? ShowWarning(string message, string detail)
    {
      return Show(message, detail, MessageBoxWindowType.Warning, MessageBoxWindowButtonsType.Ok, LanguageString.Find("Presentation_Windows_MessageBoxWindow_Warning"));
    }

    public static bool? ShowError(string message, string detail)
    {
      return Show(message, detail, MessageBoxWindowType.Error, MessageBoxWindowButtonsType.Ok, LanguageString.Find("Presentation_Windows_MessageBoxWindow_Error"));
    }

    public static void HandleViewModelMessageBoxMessage(ViewModelMessageBoxMessage message)
    {
      if (message == null) return;

      MessageBoxWindowType windowType;
      MessageBoxWindowButtonsType buttonType;
      MessageBoxTypeTranslator(message.MessageBoxType, out windowType, out buttonType);

      bool? dialogResult = MessageBoxWindowHelper.Show(
        message.Content, 
        message.Detail, 
        windowType, buttonType);

      message.Execute(dialogResult);
    }

    private static void MessageBoxTypeTranslator(ViewModelMessageBoxType messageBoxType, out MessageBoxWindowType windowType, out MessageBoxWindowButtonsType buttonType)
    {
      windowType = MessageBoxWindowType.Information;
      buttonType = MessageBoxWindowButtonsType.Ok;

      switch (messageBoxType)
      {
        case ViewModelMessageBoxType.Question:
          windowType = MessageBoxWindowType.Question;
          buttonType = MessageBoxWindowButtonsType.YesNo;
          break;
        case ViewModelMessageBoxType.Warning:
          windowType = MessageBoxWindowType.Warning;
          break;
        case ViewModelMessageBoxType.Error:
          windowType = MessageBoxWindowType.Error;
          break;
        case ViewModelMessageBoxType.None:
        case ViewModelMessageBoxType.Information:
        default:
          break;
      }
    }
  }
}
