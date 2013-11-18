using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;

namespace Gimela.Presentation.Skins
{
  public static class SkinHelper
  {
    public const string SkinColorConfigurationKey = @"SkinColor";
    public const SkinColorType DefaultSkinColor = SkinColorType.Cyan;

    public static SkinColorType StringToSkinColorType(string colorTypeString)
    {
      return (SkinColorType)Enum.Parse(typeof(SkinColorType), colorTypeString, true);
    }

    public static void LoadSkin(SkinColorType colorType)
    {
      List<ResourceDictionary> dicts = new List<ResourceDictionary>();
      foreach (var item in Application.Current.Resources.MergedDictionaries)
      {
        dicts.Add(item);
      }

      Application.Current.Resources.MergedDictionaries.Clear();

      ResourceDictionary colorDict = new ResourceDictionary();

      switch (colorType)
      {
        case SkinColorType.Black:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Black.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Blue:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Blue.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Cyan:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Cyan.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Gold:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Gold.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Green:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Green.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Purple:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Purple.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Red:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Red.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Silver:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Silver.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Violet:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Violet.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Yellow:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Yellow.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.CadetBlue:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/CadetBlue.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Crimson:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Crimson.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.DodgerBlue:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/DodgerBlue.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Gainsboro:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Gainsboro.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Goldenrod:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Goldenrod.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.LightYellow:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/LightYellow.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.LimeGreen:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/LimeGreen.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Peru:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Peru.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.Pink:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Pink.xaml", UriKind.RelativeOrAbsolute);
          break;
        case SkinColorType.SkyBlue:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/SkyBlue.xaml", UriKind.RelativeOrAbsolute);
          break;
        default:
          colorDict.Source = new Uri(@"pack://application:,,,/Gimela.Presentation.Skins;component/Skins/Cyan.xaml", UriKind.RelativeOrAbsolute);
          break;
      }

      Application.Current.Resources.MergedDictionaries.Add(colorDict);

      foreach (var item in dicts)
      {
        if (item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Black.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Blue.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Cyan.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Gold.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Green.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Purple.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Red.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Silver.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Violet.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Yellow.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/CadetBlue.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Crimson.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/DodgerBlue.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Gainsboro.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Goldenrod.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/LightYellow.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/LimeGreen.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Peru.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/Pink.xaml".ToLowerInvariant())
            || item.Source.ToString().ToLowerInvariant().Replace(@"\", @"/").EndsWith(@"component/Skins/SkyBlue.xaml".ToLowerInvariant())
            )
        {
          System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Change Skin : {0}", colorDict.Source.ToString()));
        }
        else
        {
          Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
          {
            Source = new Uri(item.Source.AbsoluteUri, UriKind.RelativeOrAbsolute)
          });
        }
      }

      dicts.Clear();
    }
  }
}
