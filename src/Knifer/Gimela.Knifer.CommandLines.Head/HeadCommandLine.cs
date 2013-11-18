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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Head
{
  public class HeadCommandLine : CommandLine
  {
    #region Fields

    private HeadCommandLineOptions options;

    #endregion

    #region Constructors

    public HeadCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = HeadOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, HeadOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartHead();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartHead()
    {
      try
      {
        if (options.IsSetFile)
        {
          string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.File);
          HeadFile(path, options.Number);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void HeadFile(string path, long lineCount)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        ReadFile(path, lineCount);
      }
    }

    private void ReadFile(string path, long lineCount)
    {
      List<string> readText = new List<string>();
      Stream stream = null;
      try
      {
        stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(stream))
        {
          stream = null;
          while (!sr.EndOfStream)
          {
            readText.Add(sr.ReadLine());
            if (readText.Count >= lineCount) break;
          }
        }
      }
      finally
      {
        if (stream != null)
          stream.Dispose();
      }

      foreach (var item in readText)
      {
        OutputText(item);
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static HeadCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      HeadCommandLineOptions targetOptions = new HeadCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          HeadOptionType optionType = HeadOptions.GetOptionType(arg);
          if (optionType == HeadOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case HeadOptionType.File:
              targetOptions.IsSetFile = true;
              targetOptions.File = commandLineOptions.Arguments[arg];
              break;
            case HeadOptionType.Number:
              long outputLines = 0;
              if (!long.TryParse(commandLineOptions.Arguments[arg], out outputLines))
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid output lines number."));
              }
              if (outputLines <= 0)
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid output lines number."));
              }
              targetOptions.Number = outputLines;
              break;
            case HeadOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case HeadOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetFile)
        {
          targetOptions.IsSetFile = true;
          targetOptions.File = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(HeadCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetFile || string.IsNullOrEmpty(checkedOptions.File))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file."));
        }
      }
    }

    #endregion
  }
}
