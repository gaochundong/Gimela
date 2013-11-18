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
using System.Text.RegularExpressions;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Find
{
  public class FindCommandLine : CommandLine
  {
    #region Fields

    private FindCommandLineOptions options;

    #endregion

    #region Constructors

    public FindCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = FindOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, FindOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartFind();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartFind()
    {
      try
      {
        if (options.IsSetDirectory)
        {
          string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
          FindDirectory(path);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void FindDirectory(string path)
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
          FindFile(file.DirectoryName, file.Name);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            FindDirectory(item.FullName);
          }
        }
      }
    }

    private void FindFile(string directoryName, string fileName)
    {
      Regex r = new Regex(WildcardCharacterHelper.TranslateWildcardToRegex(options.RegexPattern));
      if (r.IsMatch(fileName))
      {
        OutputText(Path.Combine(directoryName, fileName));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static FindCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      FindCommandLineOptions targetOptions = new FindCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          FindOptionType optionType = FindOptions.GetOptionType(arg);
          if (optionType == FindOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case FindOptionType.RegexPattern:
              targetOptions.IsSetRegexPattern = true;
              targetOptions.RegexPattern = commandLineOptions.Arguments[arg];
              break;
            case FindOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case FindOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case FindOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case FindOptionType.Version:
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

        if (commandLineOptions.Parameters.Count >= 2)
        {
          if (!targetOptions.IsSetRegexPattern)
          {
            targetOptions.IsSetRegexPattern = true;
            targetOptions.RegexPattern = commandLineOptions.Parameters.ElementAt(1);
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(FindCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetDirectory || string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
        if (!checkedOptions.IsSetRegexPattern || string.IsNullOrEmpty(checkedOptions.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify regex pattern for matching."));
        }
      }
    }

    #endregion
  }
}
