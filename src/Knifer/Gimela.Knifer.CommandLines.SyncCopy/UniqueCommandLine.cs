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

namespace Gimela.Knifer.CommandLines.SyncCopy
{
  public class SyncCopyCommandLine : CommandLine
  {
    #region Fields

    private SyncCopyCommandLineOptions options;

    #endregion

    #region Constructors

    public SyncCopyCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = SyncCopyOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, SyncCopyOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartSyncCopy();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartSyncCopy()
    {
      try
      {
        string fromDirectory = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.FromDirectory);
        string toDirectory = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.ToDirectory);
        SyncCopy(fromDirectory, toDirectory);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void SyncCopy(string fromDirectoryPath, string toDirectoryPath)
    {
      DirectoryInfo fromDirectory = new DirectoryInfo(fromDirectoryPath);
      if (!fromDirectory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", fromDirectory.FullName));
      }
      DirectoryInfo toDirectory = new DirectoryInfo(toDirectoryPath);
      if (!toDirectory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", toDirectory.FullName));
      }

      try
      {
        FileInfo[] files = toDirectory.GetFiles();
        foreach (var file in files)
        {
          string sourceFilePath = Path.Combine(fromDirectory.FullName, file.Name);
          FileInfo sourceFile = new FileInfo(sourceFilePath);
          if (!file.Exists)
          {
            OutputText(string.Format(CultureInfo.CurrentCulture,
              "No such file -- {0}", sourceFilePath));
          }
          else
          {
            sourceFile.CopyTo(file.FullName, true);
            OutputText(string.Format(CultureInfo.CurrentCulture,
              "Copy file from {0}", sourceFile.FullName));
            OutputText(string.Format(CultureInfo.CurrentCulture,
              "            to {0}", file.FullName));
          }
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = toDirectory.GetDirectories();
          foreach (var dir in directories)
          {
            string sourceDirectoryPath = Path.Combine(fromDirectory.FullName, dir.Name);
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            SyncCopy(sourceDirectory.FullName, dir.FullName);
          }
        }
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", toDirectoryPath, ex.Message));
      }
      catch (NotSupportedException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", toDirectoryPath, ex.Message));
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", toDirectoryPath, ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static SyncCopyCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      SyncCopyCommandLineOptions targetOptions = new SyncCopyCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          SyncCopyOptionType optionType = SyncCopyOptions.GetOptionType(arg);
          if (optionType == SyncCopyOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case SyncCopyOptionType.FromDirectory:
              targetOptions.FromDirectory = commandLineOptions.Arguments[arg];
              break;
            case SyncCopyOptionType.ToDirectory:
              targetOptions.ToDirectory = commandLineOptions.Arguments[arg];
              break;
            case SyncCopyOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case SyncCopyOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case SyncCopyOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(SyncCopyCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.FromDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a source directory."));
        }
        if (string.IsNullOrEmpty(checkedOptions.ToDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a target directory."));
        }
      }
    }

    #endregion
  }
}
