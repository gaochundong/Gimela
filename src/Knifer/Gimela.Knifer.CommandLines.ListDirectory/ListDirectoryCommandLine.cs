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
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.ListDirectory
{
  public class ListDirectoryCommandLine : CommandLine
  {
    #region Fields

    private ListDirectoryCommandLineOptions options;
    private static readonly string DefaultDateTimeFormat = @"yyyy-MM-dd HH:mm:ss";

    #endregion

    #region Constructors

    public ListDirectoryCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = ListDirectoryOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, ListDirectoryOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartListDirectory();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartListDirectory()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.InputDirectory);
        ListDirectory(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void ListDirectory(string path)
    {
      DirectoryInfo dir = new DirectoryInfo(path);
      if (!dir.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", dir.FullName));
      }
      else
      {
        try
        {
          OutputFormatText("{0, -18}\t{1, -18}\t{2, -18}\t{3, -16}\t{4, -30}{5}", "CreateTime", "LastAccessTime", "LastWriteTime", "Size(Bytes)", "Name", Environment.NewLine);
          OutputFormatText("{0, -18}\t{1, -18}\t{2, -18}\t{3, -16}\t{4, -30}{5}", "-------------------", "-------------------", "-------------------", "----------------", "------------------", Environment.NewLine);

          if (options.IsSetDirectory)
          {
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
              OutputFormatText("{0, -18}\t{1, -18}\t{2, -18}\t{3, 16}\t{4, -30}{5}",
                d.CreationTime.ToString(DefaultDateTimeFormat, CultureInfo.CurrentCulture),
                d.LastAccessTime.ToString(DefaultDateTimeFormat, CultureInfo.CurrentCulture),
                d.LastWriteTime.ToString(DefaultDateTimeFormat, CultureInfo.CurrentCulture),
                "<DIR>",
                d.Name, 
                Environment.NewLine);
            }
          }

          if (options.IsSetFile)
          {
            foreach (FileInfo f in dir.GetFiles())
            {
              OutputFormatText("{0, -18}\t{1, -18}\t{2, -18}\t{3, 16}\t{4, -30}{5}",
                f.CreationTime.ToString(DefaultDateTimeFormat, CultureInfo.CurrentCulture),
                f.LastAccessTime.ToString(DefaultDateTimeFormat, CultureInfo.CurrentCulture),
                f.LastWriteTime.ToString(DefaultDateTimeFormat, CultureInfo.CurrentCulture),
                f.Length,
                f.Name, 
                Environment.NewLine);
            }
          }
        }
        catch (UnauthorizedAccessException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
        }
        catch (PathTooLongException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
        }
        catch (DirectoryNotFoundException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
        }
        catch (NotSupportedException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
        }
        catch (IOException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
        }
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static ListDirectoryCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      ListDirectoryCommandLineOptions targetOptions = new ListDirectoryCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          ListDirectoryOptionType optionType = ListDirectoryOptions.GetOptionType(arg);
          if (optionType == ListDirectoryOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case ListDirectoryOptionType.InputDirectory:
              targetOptions.InputDirectory = commandLineOptions.Arguments[arg];
              break;
            case ListDirectoryOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              break;
            case ListDirectoryOptionType.File:
              targetOptions.IsSetFile = true;
              break;
            case ListDirectoryOptionType.List:
              targetOptions.IsSetList = true;
              break;
            case ListDirectoryOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case ListDirectoryOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (string.IsNullOrEmpty(targetOptions.InputDirectory))
        {
          targetOptions.InputDirectory = commandLineOptions.Parameters.First();
        }
      }

      // set default the current directory
      if (string.IsNullOrEmpty(targetOptions.InputDirectory))
      {
        targetOptions.InputDirectory = @".";
      }

      // set default options
      if (!targetOptions.IsSetDirectory && !targetOptions.IsSetFile)
      {
        targetOptions.IsSetDirectory = true;
        targetOptions.IsSetFile = true;
      }

      return targetOptions;
    }

    private static void CheckOptions(ListDirectoryCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.InputDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
      }
    }

    #endregion
  }
}
