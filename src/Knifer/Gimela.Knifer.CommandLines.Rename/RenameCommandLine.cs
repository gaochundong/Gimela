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
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Rename
{
  public class RenameCommandLine : CommandLine
  {
    #region Fields

    private RenameCommandLineOptions options;
    private readonly string executingFile = Assembly.GetExecutingAssembly().Location;
    private List<string> matchSaltList = new List<string>();

    #endregion

    #region Constructors

    public RenameCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = RenameOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, RenameOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartRename();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartRename()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.InputDirectory);
        if (options.IsSetPadString)
        {
          SearchDirectoryForPadString(path);
        }
        else
        {
          SearchDirectory(path);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

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
        FileInfo[] files = directory.GetFiles();
        foreach (var file in files.OrderBy(f => f.Name))
        {
          RenameFile(file.FullName);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories;

          if (options.IsSetFolder)
          {
            directories = directory.GetDirectories();
            foreach (var item in directories.OrderBy(d => d.Name))
            {
              if (!options.Excludes.Contains(item.Name))
              {
                if (item.Name.Contains(options.RegexPattern))
                {
                  string oldPath = item.FullName;
                  string newPath = Path.Combine(item.Parent.FullName, item.Name.Replace(options.RegexPattern, options.OutputPattern));
                  item.MoveTo(newPath);
                  OutputText(string.Format(CultureInfo.CurrentCulture, "Folder From: {0}", oldPath));
                  OutputText(string.Format(CultureInfo.CurrentCulture, "       To  : {0}", newPath));
                }
              }
            }
          }

          directories = directory.GetDirectories();
          foreach (var item in directories.OrderBy(d => d.Name))
          {
            if (!options.Excludes.Contains(item.Name))
            {
              SearchDirectory(item.FullName);
            }
          }
        }
      }
    }

    private void RenameFile(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        if (file.Name.Contains(options.RegexPattern))
        {
          string newPath = Path.Combine(file.Directory.FullName, file.Name.Replace(options.RegexPattern, options.OutputPattern));
          file.MoveTo(newPath);
          OutputText(string.Format(CultureInfo.CurrentCulture, "File From: {0}", path));
          OutputText(string.Format(CultureInfo.CurrentCulture, "     To  : {0}", newPath));
        }
      }
    }

    private void SearchDirectoryForPadString(string path)
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
        foreach (var file in files.OrderBy(f => f.Name))
        {
          RenameFileWithPadString(file.FullName);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories.OrderBy(d => d.Name))
          {
            SearchDirectory(item.FullName);
          }
        }
      }
    }

    private void RenameFileWithPadString(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        Regex r = new Regex(options.RegexPattern, RegexOptions.None);
        Match m = r.Match(file.Name);
        if (m.Success)
        {
          if (m.Groups.Count >= 4)
          {
            string pattern = string.Format(CultureInfo.CurrentCulture,
              "#0%#1:{0}%#2%", options.PadString);
            pattern = pattern.Replace("#", "{").Replace("%", "}");
            string newName = string.Format(CultureInfo.CurrentCulture,
              pattern,
              m.Groups[1].ToString(), int.Parse(m.Groups[2].ToString(), CultureInfo.CurrentCulture), m.Groups[3].ToString());
            string newPath = Path.Combine(file.Directory.FullName, newName);

            file.MoveTo(newPath);

            OutputText(string.Format(CultureInfo.CurrentCulture, "File From: {0}", path));
            OutputText(string.Format(CultureInfo.CurrentCulture, "     To  : {0}", newPath));
          }
        }
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static RenameCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      RenameCommandLineOptions targetOptions = new RenameCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          RenameOptionType optionType = RenameOptions.GetOptionType(arg);
          if (optionType == RenameOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case RenameOptionType.RegexPattern:
              targetOptions.RegexPattern = commandLineOptions.Arguments[arg];
              break;
            case RenameOptionType.InputDirectory:
              targetOptions.InputDirectory = commandLineOptions.Arguments[arg];
              break;
            case RenameOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case RenameOptionType.OutputPattern:
              targetOptions.OutputPattern = commandLineOptions.Arguments[arg];
              break;
            case RenameOptionType.Folder:
              targetOptions.IsSetFolder = true;
              break;
            case RenameOptionType.Exclude:
              targetOptions.Excludes.AddRange(
                commandLineOptions.Arguments[arg].Trim().Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());
              break;
            case RenameOptionType.PadString:
              targetOptions.IsSetPadString = true;
              targetOptions.PadString = commandLineOptions.Arguments[arg];
              break;
            case RenameOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case RenameOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(RenameCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a regex pattern."));
        }
        if (string.IsNullOrEmpty(checkedOptions.InputDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a input directory."));
        }
        if (!Directory.Exists(checkedOptions.InputDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "no such input directory."));
        }
        if (checkedOptions.IsSetPadString)
        {
          if (string.IsNullOrEmpty(checkedOptions.PadString))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "bad pad string format."));
          }
          if (!checkedOptions.RegexPattern.Contains("(") || !checkedOptions.RegexPattern.Contains(")"))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "regex pattern must contain a pair of '()'."));
          }
        }
        else
        {
          if (string.IsNullOrEmpty(checkedOptions.OutputPattern))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "bad output pattern."));
          }
        }
      }
    }

    #endregion
  }
}
