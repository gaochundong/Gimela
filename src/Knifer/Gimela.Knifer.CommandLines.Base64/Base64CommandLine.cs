﻿/*
 * [The "BSD Licence"]
 * Copyright (c) 2011-2012 Chundong Gao
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ''AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Base64
{
  public class Base64CommandLine : CommandLine
  {
    #region Fields

    private Base64CommandLineOptions options;

    #endregion

    #region Constructors

    public Base64CommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = Base64Options.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, Base64Options.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartBase64();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartBase64()
    {
      try
      {
        Encoding encoding = Encoding.UTF8;
        if (options.IsSetEncoding)
        {
          encoding = options.Encoding;
        }

        if (options.IsSetDecode)
        {
          DecodeFromBase64(encoding, options.Text);
        }
        else
        {
          if (options.IsSetText)
          {
            EncodeToBase64(encoding, options.Text);
          }
          else if (options.IsSetFile)
          {
            EncodeFileToBase64(encoding, options.File);
          }
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void EncodeFileToBase64(Encoding encoding, string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          byte[] readBytes = new byte[fs.Length];
          fs.Read(readBytes, 0, (int)fs.Length);
          EncodeToBase64(encoding, readBytes);
        }
      }
    }

    private void EncodeToBase64(Encoding encoding, string text)
    {
      byte[] toEncodeAsBytes = encoding.GetBytes(text);
      string base64String = System.Convert.ToBase64String(toEncodeAsBytes);
      OutputText(base64String);
    }

    private void EncodeToBase64(Encoding encoding, byte[] bytes)
    {
      string base64String = System.Convert.ToBase64String(bytes);
      OutputText(base64String);
    }

    private void DecodeFromBase64(Encoding encoding, string text)
    {
      try
      {
        byte[] encodedDataAsBytes = System.Convert.FromBase64String(text);
        string data = encoding.GetString(encodedDataAsBytes);
        OutputText(data);
      }
      catch (FormatException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Bad format -- {0}", ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BigEndianUnicode"),
     SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static Base64CommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      Base64CommandLineOptions targetOptions = new Base64CommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          Base64OptionType optionType = Base64Options.GetOptionType(arg);
          if (optionType == Base64OptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case Base64OptionType.Decode:
              targetOptions.IsSetDecode = true;
              break;
            case Base64OptionType.Encoding:
              targetOptions.IsSetEncoding = true;
              if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"ASCII")
              {
                targetOptions.Encoding = Encoding.ASCII;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF7")
              {
                targetOptions.Encoding = Encoding.UTF7;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF8")
              {
                targetOptions.Encoding = Encoding.UTF8;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UNICODE")
              {
                targetOptions.Encoding = Encoding.Unicode;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF32")
              {
                targetOptions.Encoding = Encoding.UTF32;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"BIGENDIANUNICODE")
              {
                targetOptions.Encoding = Encoding.BigEndianUnicode;
              }
              else
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid encoding, support ASCII, UTF7, UTF8, UTF32, Unicode, BigEndianUnicode."));
              }
              break;
            case Base64OptionType.Text:
              targetOptions.IsSetText = true;
              targetOptions.Text = commandLineOptions.Arguments[arg];
              break;
            case Base64OptionType.File:
              targetOptions.IsSetFile = true;
              targetOptions.File = commandLineOptions.Arguments[arg];
              break;
            case Base64OptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case Base64OptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetText)
        {
          targetOptions.IsSetText = true;
          targetOptions.Text = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(Base64CommandLineOptions options)
    {
      if (!options.IsSetHelp && !options.IsSetVersion)
      {
        if (!options.IsSetFile && !options.IsSetText)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file or a text."));
        }
        if (options.IsSetFile && options.IsSetText)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "can only specify a file or a text."));
        }
        if (options.IsSetFile && string.IsNullOrEmpty(options.File))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad file path."));
        }
        if (options.IsSetText && string.IsNullOrEmpty(options.Text))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad text format."));
        }
      }
    }

    #endregion
  }
}
