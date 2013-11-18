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

namespace Gimela.Knifer.CommandLines.Extract
{
  public class ExtractCommandLine : CommandLine
  {
    #region Fields

    private ExtractCommandLineOptions options;
    private readonly string executingFile = Assembly.GetExecutingAssembly().Location;
    private List<string> matchSaltList = new List<string>();

    #endregion

    #region Constructors

    public ExtractCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = ExtractOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, ExtractOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartExtract();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartExtract()
    {
      try
      {
        DateTime executeBeginTime = DateTime.Now;

        string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.InputDirectory);
        ExtractDirectory(path);

        DateTime executeEndTime = DateTime.Now;
        TimeSpan duration = executeEndTime - executeBeginTime;
        OutputText(Environment.NewLine);
        OutputText(string.Format(CultureInfo.CurrentCulture, "Extract Begin Time : {0}", executeBeginTime.ToString(@"yyyy-MM-dd HH:mm:ss")));
        OutputText(string.Format(CultureInfo.CurrentCulture, "Extract End   Time : {0}", executeEndTime.ToString(@"yyyy-MM-dd HH:mm:ss")));
        OutputText(string.Format(CultureInfo.CurrentCulture, "Extract Total Time : {0}", 
          string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", 
          duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds)));
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void ExtractDirectory(string path)
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
          ExtractFile(file.FullName);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories.OrderBy(d => d.Name))
          {
            ExtractDirectory(item.FullName);
          }
        }
      }
    }

    private void ExtractFile(string path)
    {
      if (IsCanExtractFile(path))
      {
        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "No such file -- {0}", file.FullName));
        }
        else
        {
          MatchFile(path);
        }
      }
    }

    private void MatchFile(string path)
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
          }
        }
      }
      finally
      {
        if (stream != null)
          stream.Dispose();
      }

      try
      {
        Regex regexSalt = new Regex(options.RegexPattern, RegexOptions.None);
        for (int i = 0; i < readText.Count; i++)
        {
          Match matchSalt = regexSalt.Match(readText[i]);
          if (matchSalt.Success)
          {
            if (!options.Excludes.Contains(matchSalt.Groups[1].ToString()))
            {
              if (!matchSaltList.Contains(matchSalt.Groups[1].ToString()))
              {
                matchSaltList.Add(matchSalt.Groups[1].ToString());
              }
            }
          }

          foreach (var salt in matchSaltList)
          {
            Regex r = new Regex(salt, RegexOptions.None);
            Match m = r.Match(readText[i]);
            if (m.Success)
            {
              OutputFileData(salt, path, i + 1, readText[i]);
            }            
          }
        }
      }
      catch (ArgumentException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Bad regex pattern -- {0}, {1}", options.RegexPattern, ex.Message));
      }
      catch (IndexOutOfRangeException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Bad regex pattern -- {0}, {1}", options.RegexPattern, ex.Message));
      }
    }

    private bool IsCanExtractFile(string file)
    {
      bool result = false;

      if (string.IsNullOrEmpty(file))
      {
        result = false;
      }
      else if (executingFile == file)
      {
        result = false;
      }
      else if (file.ToUpperInvariant().EndsWith(".EXE", StringComparison.CurrentCulture))
      {
        result = false;
      }
      else if (options.InputFileExtensionFilter.Count > 0)
      {
        bool isFound = false;
        foreach (var filter in options.InputFileExtensionFilter)
        {
          if (file.EndsWith(filter, StringComparison.CurrentCulture))
          {
            isFound = true;
            break;
          }
        }
        if (isFound)
        {
          result = true;
        }
      }
      else
      {
        result = true;
      }

      return result;
    }

    private void OutputFileData(string outputFilePrefix, string path, int lineNumber, string lineText)
    {
      string data = string.Format(CultureInfo.CurrentCulture, "{0} : {1, -10} : {2}{3}", path, lineNumber, lineText, Environment.NewLine);
      RaiseCommandLineDataChanged(this, data);
      WriteOutputFile(outputFilePrefix, data);
    }

    private void WriteOutputFile(string outputFilePrefix, string data)
    {
      string path = Path.Combine(options.OutputDirectory, outputFilePrefix.TrimEnd() + options.OutputFileExtension);
      try
      {
        using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.UTF8))
        {
          sw.AutoFlush = true;
          sw.Write(data);
        }
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", path, ex.Message));
      }
      catch (PathTooLongException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", path, ex.Message));
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", path, ex.Message));
      }
      catch (SecurityException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", path, ex.Message));
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", path, ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static ExtractCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      ExtractCommandLineOptions targetOptions = new ExtractCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          ExtractOptionType optionType = ExtractOptions.GetOptionType(arg);
          if (optionType == ExtractOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case ExtractOptionType.RegexPattern:
              targetOptions.RegexPattern = commandLineOptions.Arguments[arg];
              break;
            case ExtractOptionType.InputDirectory:
              targetOptions.InputDirectory = commandLineOptions.Arguments[arg];
              break;
            case ExtractOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case ExtractOptionType.InputFileExtensionFilter:
              targetOptions.InputFileExtensionFilter.AddRange(
                commandLineOptions.Arguments[arg].Trim().Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());
              break;
            case ExtractOptionType.OutputDirectory:
              targetOptions.OutputDirectory = commandLineOptions.Arguments[arg];
              break;
            case ExtractOptionType.OutputFileExtension:
              targetOptions.OutputFileExtension = commandLineOptions.Arguments[arg].Trim();
              break;
            case ExtractOptionType.Exclude:
              targetOptions.Excludes.AddRange(
                commandLineOptions.Arguments[arg].Trim().Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());
              break;
            case ExtractOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case ExtractOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(ExtractCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a regex pattern."));
        }
        if (!checkedOptions.RegexPattern.Contains("(")
          || !checkedOptions.RegexPattern.Contains(")"))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "regex pattern must contain a pair of '()'."));
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
        if (string.IsNullOrEmpty(checkedOptions.OutputDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a output directory."));
        }
        if (!Directory.Exists(checkedOptions.OutputDirectory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "no such output directory."));
        }
        if (!checkedOptions.OutputFileExtension.StartsWith(".", StringComparison.CurrentCulture))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "output file extension must start with '.'."));
        }
        foreach (var filter in checkedOptions.InputFileExtensionFilter)
        {
          if (!filter.StartsWith(".", StringComparison.CurrentCulture))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "input file extension filter item must start with '.'."));
          }
        }
      }
    }

    #endregion
  }
}
