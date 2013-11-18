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

namespace Gimela.Knifer.CommandLines.Unique
{
  public class UniqueCommandLine : CommandLine
  {
    #region Fields

    private UniqueCommandLineOptions options;

    #endregion

    #region Constructors

    public UniqueCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = UniqueOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, UniqueOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartUnique();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartUnique()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.InputFile);
        Unique(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void Unique(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        try
        {
          string renamedFile = file.FullName + ".original";
          File.Delete(renamedFile);

          IList<string> readText = new List<string>();
          using (StreamReader sr = new StreamReader(file.FullName))
          {
            while (!sr.EndOfStream)
            {
              readText.Add(sr.ReadLine());
            }
          }

          List<string> uniqueText = readText.Distinct().ToList();
          if (options.IsSetSort)
          {
            uniqueText.Sort();
          }

          using (StreamWriter sw = new StreamWriter(renamedFile, false, System.Text.Encoding.UTF8))
          {
            foreach (var item in uniqueText)
            {
              sw.WriteLine(item);
            }
          }

          if (options.IsSetOutputFile)
          {
            string outputPath = WildcardCharacterHelper.TranslateWildcardFilePath(options.OutputFile);
            File.Delete(outputPath);
            File.Move(renamedFile, outputPath);
          }
          else
          {
            File.Delete(file.FullName);
            File.Move(renamedFile, file.FullName);
          }
          File.Delete(renamedFile);
        }
        catch (UnauthorizedAccessException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (PathTooLongException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (DirectoryNotFoundException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (NotSupportedException ex)
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

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static UniqueCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      UniqueCommandLineOptions targetOptions = new UniqueCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          UniqueOptionType optionType = UniqueOptions.GetOptionType(arg);
          if (optionType == UniqueOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case UniqueOptionType.InputFile:
              targetOptions.InputFile = commandLineOptions.Arguments[arg];
              break;
            case UniqueOptionType.OutputFile:
              targetOptions.IsSetOutputFile = true;
              targetOptions.OutputFile = commandLineOptions.Arguments[arg];
              break;
            case UniqueOptionType.Sort:
              targetOptions.IsSetSort = true;
              break;
            case UniqueOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case UniqueOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (string.IsNullOrEmpty(targetOptions.InputFile))
        {
          targetOptions.InputFile = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(UniqueCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.InputFile))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file."));
        }
        if (checkedOptions.IsSetOutputFile && string.IsNullOrEmpty(checkedOptions.OutputFile))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad output file path."));
        }
      }
    }

    #endregion
  }
}
