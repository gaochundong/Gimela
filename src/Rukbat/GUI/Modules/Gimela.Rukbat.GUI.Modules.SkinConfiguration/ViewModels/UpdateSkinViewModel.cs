using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Presentation.Skins;
using Gimela.Crust.Tectosphere;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Common.Configuration;

namespace Gimela.Rukbat.GUI.Modules.SkinConfiguration.ViewModels
{
  public class UpdateSkinViewModel : ViewModelResponsive
  {
    #region Ctors

    public UpdateSkinViewModel()
    {
      SkinColorType skin = SkinHelper.DefaultSkinColor;

      if (ConfigurationMaster.ContainsKey(SkinHelper.SkinColorConfigurationKey))
      {
        skin = SkinHelper.StringToSkinColorType(ConfigurationMaster.Get(SkinHelper.SkinColorConfigurationKey));
      }

      DefaultColorType = skin;
    }

    #endregion

    #region Properties

    private SkinColorType _defaultColorType;
    public SkinColorType DefaultColorType
    {
      get
      {
        return _defaultColorType;
      }
      set
      {
        _defaultColorType = value;
        RaisePropertyChanged("DefaultColorType");

        SkinColorTypeToBoolean(_defaultColorType);
      }
    }

    private bool _IsBlack;
    public bool IsBlack
    {
      get
      {
        return _IsBlack;
      }
      set
      {
        if (_IsBlack == value) return;

        _IsBlack = value;
        RaisePropertyChanged("IsBlack");
      }
    }

    private bool _IsBlue;
    public bool IsBlue
    {
      get
      {
        return _IsBlue;
      }
      set
      {
        if (_IsBlue == value) return;

        _IsBlue = value;
        RaisePropertyChanged("IsBlue");
      }
    }

    private bool _IsCyan;
    public bool IsCyan
    {
      get
      {
        return _IsCyan;
      }
      set
      {
        if (_IsCyan == value) return;

        _IsCyan = value;
        RaisePropertyChanged("IsCyan");
      }
    }

    private bool _IsGold;
    public bool IsGold
    {
      get
      {
        return _IsGold;
      }
      set
      {
        if (_IsGold == value) return;

        _IsGold = value;
        RaisePropertyChanged("IsGold");
      }
    }

    private bool _IsGreen;
    public bool IsGreen
    {
      get
      {
        return _IsGreen;
      }
      set
      {
        if (_IsGreen == value) return;

        _IsGreen = value;
        RaisePropertyChanged("IsGreen");
      }
    }

    private bool _IsPurple;
    public bool IsPurple
    {
      get
      {
        return _IsPurple;
      }
      set
      {
        if (_IsPurple == value) return;

        _IsPurple = value;
        RaisePropertyChanged("IsPurple");
      }
    }

    private bool _IsRed;
    public bool IsRed
    {
      get
      {
        return _IsRed;
      }
      set
      {
        if (_IsRed == value) return;

        _IsRed = value;
        RaisePropertyChanged("IsRed");
      }
    }

    private bool _IsSilver;
    public bool IsSilver
    {
      get
      {
        return _IsSilver;
      }
      set
      {
        if (_IsSilver == value) return;

        _IsSilver = value;
        RaisePropertyChanged("IsSilver");
      }
    }

    private bool _IsViolet;
    public bool IsViolet
    {
      get
      {
        return _IsViolet;
      }
      set
      {
        if (_IsViolet == value) return;

        _IsViolet = value;
        RaisePropertyChanged("IsViolet");
      }
    }

    private bool _IsYellow;
    public bool IsYellow
    {
      get
      {
        return _IsYellow;
      }
      set
      {
        if (_IsYellow == value) return;

        _IsYellow = value;
        RaisePropertyChanged("IsYellow");
      }
    }

    private bool _IsCadetBlue;
    public bool IsCadetBlue
    {
      get
      {
        return _IsCadetBlue;
      }
      set
      {
        if (_IsCadetBlue == value) return;

        _IsCadetBlue = value;
        RaisePropertyChanged("IsCadetBlue");
      }
    }

    private bool _IsCrimson;
    public bool IsCrimson
    {
      get
      {
        return _IsCrimson;
      }
      set
      {
        if (_IsCrimson == value) return;

        _IsCrimson = value;
        RaisePropertyChanged("IsCrimson");
      }
    }

    private bool _IsDodgerBlue;
    public bool IsDodgerBlue
    {
      get
      {
        return _IsDodgerBlue;
      }
      set
      {
        if (_IsDodgerBlue == value) return;

        _IsDodgerBlue = value;
        RaisePropertyChanged("IsDodgerBlue");
      }
    }

    private bool _IsGainsboro;
    public bool IsGainsboro
    {
      get
      {
        return _IsGainsboro;
      }
      set
      {
        if (_IsGainsboro == value) return;

        _IsGainsboro = value;
        RaisePropertyChanged("IsGainsboro");
      }
    }

    private bool _IsGoldenrod;
    public bool IsGoldenrod
    {
      get
      {
        return _IsGoldenrod;
      }
      set
      {
        if (_IsGoldenrod == value) return;

        _IsGoldenrod = value;
        RaisePropertyChanged("IsGoldenrod");
      }
    }

    private bool _IsLightYellow;
    public bool IsLightYellow
    {
      get
      {
        return _IsLightYellow;
      }
      set
      {
        if (_IsLightYellow == value) return;

        _IsLightYellow = value;
        RaisePropertyChanged("IsLightYellow");
      }
    }

    private bool _IsLimeGreen;
    public bool IsLimeGreen
    {
      get
      {
        return _IsLimeGreen;
      }
      set
      {
        if (_IsLimeGreen == value) return;

        _IsLimeGreen = value;
        RaisePropertyChanged("IsLimeGreen");
      }
    }

    private bool _IsPeru;
    public bool IsPeru
    {
      get
      {
        return _IsPeru;
      }
      set
      {
        if (_IsPeru == value) return;

        _IsPeru = value;
        RaisePropertyChanged("IsPeru");
      }
    }

    private bool _IsPink;
    public bool IsPink
    {
      get
      {
        return _IsPink;
      }
      set
      {
        if (_IsPink == value) return;

        _IsPink = value;
        RaisePropertyChanged("IsPink");
      }
    }

    private bool _IsSkyBlue;
    public bool IsSkyBlue
    {
      get
      {
        return _IsSkyBlue;
      }
      set
      {
        if (_IsSkyBlue == value) return;

        _IsSkyBlue = value;
        RaisePropertyChanged("IsSkyBlue");
      }
    }

    #endregion

    #region Bindings

    protected override void BindCommands()
    {
      ColorSelectedCommand = new RelayCommand(() =>
      {
        SkinColorType colorType = BooleanToSkinColorType();
        SkinHelper.LoadSkin(colorType);
        SaveSkin(colorType);
      });

      CancelCommand = new RelayCommand(() =>
      {
        SkinColorTypeToBoolean(DefaultColorType);
        SkinHelper.LoadSkin(DefaultColorType);
        Messenger.Default.Send(new NotificationMessage(this, UIMessageType.SkinConfiguration_CloseWindowEvent));
      });

      OKCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(this, UIMessageType.SkinConfiguration_CloseWindowEvent));
      });
    }

    protected override void UnbindCommands()
    {
      ColorSelectedCommand = null;
      CancelCommand = null;
      OKCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand ColorSelectedCommand { get; private set; }

    public RelayCommand CancelCommand { get; private set; }

    public RelayCommand OKCommand { get; private set; }

    #endregion

    #region Methods

    private void SkinColorTypeToBoolean(SkinColorType colorType)
    {
      switch (colorType)
      {
        case SkinColorType.Black:
          IsBlack = true;
          break;
        case SkinColorType.Blue:
          IsBlue = true;
          break;
        case SkinColorType.Cyan:
          IsCyan = true;
          break;
        case SkinColorType.Gold:
          IsGold = true;
          break;
        case SkinColorType.Green:
          IsGreen = true;
          break;
        case SkinColorType.Purple:
          IsPurple = true;
          break;
        case SkinColorType.Red:
          IsRed = true;
          break;
        case SkinColorType.Silver:
          IsSilver = true;
          break;
        case SkinColorType.Violet:
          IsViolet = true;
          break;
        case SkinColorType.Yellow:
          IsYellow = true;
          break;
        case SkinColorType.CadetBlue:
          IsCadetBlue = true;
          break;
        case SkinColorType.Crimson:
          IsCrimson = true;
          break;
        case SkinColorType.DodgerBlue:
          IsDodgerBlue = true;
          break;
        case SkinColorType.Gainsboro:
          IsGainsboro = true;
          break;
        case SkinColorType.Goldenrod:
          IsGoldenrod = true;
          break;
        case SkinColorType.LightYellow:
          IsLightYellow = true;
          break;
        case SkinColorType.LimeGreen:
          IsLimeGreen = true;
          break;
        case SkinColorType.Peru:
          IsPeru = true;
          break;
        case SkinColorType.Pink:
          IsPink = true;
          break;
        case SkinColorType.SkyBlue:
          IsSkyBlue = true;
          break;
        default:
          IsCyan = true;
          break;
      }
    }

    private SkinColorType BooleanToSkinColorType()
    {
      SkinColorType colorType = SkinColorType.Cyan;

      if (IsBlack)
      {
        colorType = SkinColorType.Black;
      }
      else if (IsBlue)
      {
        colorType = SkinColorType.Blue;
      }
      else if (IsCyan)
      {
        colorType = SkinColorType.Cyan;
      }
      else if (IsGold)
      {
        colorType = SkinColorType.Gold;
      }
      else if (IsGreen)
      {
        colorType = SkinColorType.Green;
      }
      else if (IsPurple)
      {
        colorType = SkinColorType.Purple;
      }
      else if (IsRed)
      {
        colorType = SkinColorType.Red;
      }
      else if (IsSilver)
      {
        colorType = SkinColorType.Silver;
      }
      else if (IsViolet)
      {
        colorType = SkinColorType.Violet;
      }
      else if (IsYellow)
      {
        colorType = SkinColorType.Yellow;
      }
      else if (IsCadetBlue)
      {
        colorType = SkinColorType.CadetBlue;
      }
      else if (IsCrimson)
      {
        colorType = SkinColorType.Crimson;
      }
      else if (IsDodgerBlue)
      {
        colorType = SkinColorType.DodgerBlue;
      }
      else if (IsGainsboro)
      {
        colorType = SkinColorType.Gainsboro;
      }
      else if (IsGoldenrod)
      {
        colorType = SkinColorType.Goldenrod;
      }
      else if (IsLightYellow)
      {
        colorType = SkinColorType.LightYellow;
      }
      else if (IsLimeGreen)
      {
        colorType = SkinColorType.LimeGreen;
      }
      else if (IsPeru)
      {
        colorType = SkinColorType.Peru;
      }
      else if (IsPink)
      {
        colorType = SkinColorType.Pink;
      }
      else if (IsSkyBlue)
      {
        colorType = SkinColorType.SkyBlue;
      }

      return colorType;
    }

    private void SaveSkin(SkinColorType colorType)
    {
      ConfigurationMaster.Set(SkinHelper.SkinColorConfigurationKey, colorType.ToString());
    }

    #endregion
  }
}
