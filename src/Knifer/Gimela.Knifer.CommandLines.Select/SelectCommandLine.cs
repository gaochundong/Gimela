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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Select
{
  public class SelectCommandLine : CommandLine
  {
    #region Fields

    private SelectCommandLineOptions options;

    #endregion

    #region Constructors

    public SelectCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = SelectOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, SelectOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartSelect();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartSelect()
    {
      try
      {
        if (options.IsSetDirectory)
        {
          string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
          SelectDirectory(path);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void SelectDirectory(string path)
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
          SelectFile(file.DirectoryName, file.Name);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            SelectDirectory(item.FullName);
          }
        }
      }
    }

    private void SelectFile(string directoryName, string fileName)
    {
      try
      {
        string[] extensions = options.Extension.Split(new char[] { ',', ';' });
        foreach (var extension in extensions)
        {
          if (fileName.EndsWith(extension))
          {
            if (options.IsSetOutput)
            {
              string outputBase = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Output);
              DirectoryInfo outputBaseDir = new DirectoryInfo(outputBase);
              if (!outputBaseDir.Exists)
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "No such directory -- {0}", outputBaseDir.FullName));
              }

              string outputPath = GetDepthOutputDirectory(outputBase, directoryName);
              DirectoryInfo outputPathDir = new DirectoryInfo(outputPath);
              if (!outputPathDir.Exists)
              {
                outputPathDir.Create();
              }

              if (options.IsSetMove)
              {
                File.Move(Path.Combine(directoryName, fileName), Path.Combine(outputPath, fileName));
                OutputText(string.Format(CultureInfo.CurrentCulture, "Move file from -> {0}", Path.Combine(directoryName, fileName)));
                OutputText(string.Format(CultureInfo.CurrentCulture, "            to -> {0}", Path.Combine(outputPath, fileName)));
              }
              else if (options.IsSetCopy)
              {
                File.Copy(Path.Combine(directoryName, fileName), Path.Combine(outputPath, fileName), true);
                OutputText(string.Format(CultureInfo.CurrentCulture, "Copy file from -> {0}", Path.Combine(directoryName, fileName)));
                OutputText(string.Format(CultureInfo.CurrentCulture, "            to -> {0}", Path.Combine(outputPath, fileName)));
              }
            }
          }
        }
      }
      catch (NotSupportedException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
      catch (PathTooLongException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
      catch (FileNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
      catch (SecurityException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", Path.Combine(directoryName, fileName), ex.Message));
      }
    }

    private string GetDepthOutputDirectory(string outputDirectory, string fileDirectoryName)
    {
      string outputPath = outputDirectory;

      if (options.IsSetKeepDepth)
      {
        string selectPath = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
        string partDir = fileDirectoryName.Replace(selectPath, "")
          .TrimStart(new char[] { '/', '\\' })
          .TrimEnd(new char[] { '/', '\\' })
          .Replace('/', '|')
          .Replace('\\', '|');
        if (!string.IsNullOrEmpty(partDir))
        {
          string[] depthDirList = partDir.Split('|').Take(options.KeepDepth).ToArray();          
          foreach (var item in depthDirList)
          {
            outputPath = Path.Combine(outputPath, item);
          }
        }
      }

      return outputPath;
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static SelectCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      SelectCommandLineOptions targetOptions = new SelectCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          SelectOptionType optionType = SelectOptions.GetOptionType(arg);
          if (optionType == SelectOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case SelectOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case SelectOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case SelectOptionType.Extension:
              targetOptions.IsSetExtension = true;
              targetOptions.Extension = commandLineOptions.Arguments[arg];
              break;
            case SelectOptionType.Output:
              targetOptions.IsSetOutput = true;
              targetOptions.Output = commandLineOptions.Arguments[arg];
              break;
            case SelectOptionType.Copy:
              targetOptions.IsSetCopy = true;
              break;
            case SelectOptionType.Move:
              targetOptions.IsSetMove = true;
              break;
            case SelectOptionType.KeepDepth:
              targetOptions.IsSetKeepDepth = true;
              int depth = 0;
              if (!int.TryParse(commandLineOptions.Arguments[arg], out depth))
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid depth."));
              }
              if (depth <= 0 || depth > 10)
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid depth [1, 9]."));
              }
              targetOptions.KeepDepth = depth;
              break;
            case SelectOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case SelectOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (string.IsNullOrEmpty(targetOptions.Directory))
        {
          targetOptions.IsSetDirectory = true;
          targetOptions.Directory = commandLineOptions.Parameters.First();
        }
      }

      if (targetOptions.IsSetOutput && !targetOptions.IsSetMove)
      {
        targetOptions.IsSetCopy = true;
      }

      return targetOptions;
    }

    private static void CheckOptions(SelectCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetDirectory || string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
        if (!checkedOptions.IsSetExtension || string.IsNullOrEmpty(checkedOptions.Extension))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file extension type."));
        }
        if (checkedOptions.IsSetOutput && string.IsNullOrEmpty(checkedOptions.Output))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad output directory."));
        }
        if (checkedOptions.IsSetOutput && !Directory.Exists(checkedOptions.Output))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "no such output directory."));
        }
      }
    }

    #endregion
  }
}
