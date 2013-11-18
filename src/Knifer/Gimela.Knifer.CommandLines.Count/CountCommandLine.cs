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

namespace Gimela.Knifer.CommandLines.Count
{
  public class CountCommandLine : CommandLine
  {
    #region Fields

    private CountCommandLineOptions options;
    private readonly IDictionary<string, int> countSummary;

    #endregion

    #region Constructors

    public CountCommandLine(string[] args)
      : base(args)
    {
      this.countSummary = new Dictionary<string, int>();
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = CountOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, CountOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartCount();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Knifer.CommandLines.Count.CountCommandLine.OutputText(System.String)"),
     SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase"),
     SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "FileType")]
    private void StartCount()
    {
      try
      {
        if (options.IsSetDirectory)
        {
          string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
          CountDirectory(path);
        }

        foreach (var item in countSummary.OrderByDescending(t => t.Value).ThenBy(w => w.Key))
        {
          OutputText(string.Format(CultureInfo.CurrentCulture, "FileType: {0,-30}Count: {1}", item.Key.ToLowerInvariant(), item.Value));
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void CountDirectory(string path)
    {
      DirectoryInfo directory = new DirectoryInfo(path);
      if (!directory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", directory.FullName));
      }
      else
      {
        FileInfo[] files = directory.GetFiles();
        foreach (var file in files)
        {
          CountFile(file.FullName);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            CountDirectory(item.FullName);
          }
        }
      }
    }

    private void CountFile(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        if (countSummary.ContainsKey(file.Extension.ToUpperInvariant()))
        {
          countSummary[file.Extension.ToUpperInvariant()]++;
        }
        else
        {
          countSummary.Add(file.Extension.ToUpperInvariant(), 1);
        }
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static CountCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      CountCommandLineOptions targetOptions = new CountCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          CountOptionType optionType = CountOptions.GetOptionType(arg);
          if (optionType == CountOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case CountOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case CountOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case CountOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case CountOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetDirectory)
        {
          targetOptions.IsSetDirectory = true;
          targetOptions.Directory = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(CountCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetDirectory || string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
      }
    }

    #endregion
  }
}
