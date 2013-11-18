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

namespace Gimela.Knifer.CommandLines.Remove
{
  public class RemoveCommandLine : CommandLine
  {
    #region Fields

    private RemoveCommandLineOptions options;

    #endregion

    #region Constructors

    public RemoveCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = RemoveOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, RemoveOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartRemove();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartRemove()
    {
      try
      {
        if (options.IsSetDirectory)
        {
          string dirPath = WildcardCharacterHelper.TranslateWildcardFilePath(options.Directory);
          foreach (var pattern in options.Files)
          {
            SearchFiles(dirPath, WildcardCharacterHelper.TranslateWildcardToRegex(pattern));
          }
        }
        else
        {
          foreach (var file in options.Files)
          {
            string filePath = WildcardCharacterHelper.TranslateWildcardFilePath(file);
            string dirPath = Path.GetDirectoryName(filePath);
            string pattern = Path.GetFileName(filePath);
            if (string.IsNullOrEmpty(dirPath))
            {
              DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
              dirPath = currentDirectory.FullName;
            }
            SearchFiles(dirPath, WildcardCharacterHelper.TranslateWildcardToRegex(pattern));
          }
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void SearchFiles(string path, string pattern)
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
          if (IsMatchRegexPattern(file.Name, pattern))
          {
            RemoveFile(file.FullName);
          }
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            SearchFiles(item.FullName, pattern);
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Knifer.CommandLines.Remove.RemoveCommandLine.OutputText(System.String)")]
    private void RemoveFile(string path)
    {
      try
      {
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
          try
          {
            file.Delete();
            OutputText(string.Format(CultureInfo.CurrentCulture, "Removed - {0}", file.FullName));
          }
          catch (UnauthorizedAccessException ex)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Operation exception -- {0}, {1}", file.FullName, ex.Message));
          }
          catch (SecurityException ex)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Operation exception -- {0}, {1}", file.FullName, ex.Message));
          }
          catch (IOException ex)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Operation exception -- {0}, {1}", file.FullName, ex.Message));
          }
        }
      }
      catch (ArgumentException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", path, ex.Message));
      }
    }

    private bool IsMatchRegexPattern(string name, string pattern)
    {
      bool result = false;

      Regex r = new Regex(pattern);
      if (r.IsMatch(name))
      {
        result = true;
      }

      return result;
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static RemoveCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      RemoveCommandLineOptions targetOptions = new RemoveCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          RemoveOptionType optionType = RemoveOptions.GetOptionType(arg);
          if (optionType == RemoveOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case RemoveOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case RemoveOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case RemoveOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case RemoveOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        foreach (var item in commandLineOptions.Parameters)
        {
          targetOptions.Files.Add(item);
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(RemoveCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (checkedOptions.Files.Count <= 0)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file to be removed."));
        }
        if (checkedOptions.IsSetDirectory && string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad directory path."));
        }
      }
    }

    #endregion
  }
}
