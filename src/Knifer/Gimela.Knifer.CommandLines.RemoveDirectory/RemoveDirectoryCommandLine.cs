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
using System.Security;
using System.Text.RegularExpressions;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.RemoveDirectory
{
  public class RemoveDirectoryCommandLine : CommandLine
  {
    #region Fields

    private RemoveDirectoryCommandLineOptions options;

    #endregion

    #region Constructors

    public RemoveDirectoryCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = RemoveDirectoryOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, RemoveDirectoryOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartRemoveDirectory();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartRemoveDirectory()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
        SearchDirectory(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Knifer.CommandLines.RemoveDirectory.RemoveDirectoryCommandLine.OutputText(System.String)")]
    private void SearchDirectory(string path)
    {
      DirectoryInfo directory = new DirectoryInfo(path);
      if (!directory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", directory.FullName));
      }
      else
      {
        DirectoryInfo[] directories = directory.GetDirectories();
        foreach (var item in directories)
        {
          if (IsMatchRegexPattern(item.Name))
          {
            try
            {
              if (options.IsSetEmpty)
              {
                if (item.GetFiles().Length <= 0 && item.GetDirectories().Length <= 0)
                {
                  item.Delete(true);
                  OutputText(string.Format(CultureInfo.CurrentCulture, "Removed - {0}", item.FullName));
                }
              }
              else
              {
                item.Delete(true);
                OutputText(string.Format(CultureInfo.CurrentCulture, "Removed - {0}", item.FullName));
              }
            }
            catch (UnauthorizedAccessException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
            catch (DirectoryNotFoundException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
            catch (SecurityException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
            catch (IOException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
          }
          else
          {
            if (options.IsSetRecursive)
            {
              SearchDirectory(item.FullName);
            }
          }
        }
      }
    }

    private bool IsMatchRegexPattern(string name)
    {
      bool result = false;

      if (options.IsSetFixedString)
      {
        if (name == options.RegexPattern)
        {
          result = true;
        }
      }
      else
      {
        Regex r = new Regex(options.RegexPattern);
        if (r.IsMatch(name))
        {
          result = true;
        }
      }

      return result;
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static RemoveDirectoryCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      RemoveDirectoryCommandLineOptions targetOptions = new RemoveDirectoryCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          RemoveDirectoryOptionType optionType = RemoveDirectoryOptions.GetOptionType(arg);
          if (optionType == RemoveDirectoryOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case RemoveDirectoryOptionType.Directory:
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case RemoveDirectoryOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case RemoveDirectoryOptionType.RegexPattern:
              targetOptions.RegexPattern = commandLineOptions.Arguments[arg];
              break;
            case RemoveDirectoryOptionType.FixedString:
              targetOptions.IsSetFixedString = true;
              break;
            case RemoveDirectoryOptionType.Empty:
              targetOptions.IsSetEmpty = true;
              break;
            case RemoveDirectoryOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case RemoveDirectoryOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(RemoveDirectoryCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
        if (string.IsNullOrEmpty(checkedOptions.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a pattern for matching."));
        }
      }
    }

    #endregion
  }
}
