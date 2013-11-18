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

namespace Gimela.Knifer.CommandLines.Grep
{
  /// <summary>
  /// Global Regular Expression Print
  /// </summary>
  public class GrepCommandLine : CommandLine
  {
    #region Fields

    private GrepCommandLineOptions options;
    private readonly string executingFile = Assembly.GetExecutingAssembly().Location;

    #endregion

    #region Constructors

    public GrepCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = GrepOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, GrepOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        if (options.IsSetOutputFile)
        {
          DeleteOutputFile();
        }

        StartGrep();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartGrep()
    {
      try
      {
        foreach (var item in options.FilePaths)
        {
          string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(item);
          if (options.IsSetDirectory)
          {
            GrepDirectory(path);
          }
          else
          {
            if (WildcardCharacterHelper.IsContainsWildcard(path))
            {
              string dir = path;
              if (!path.Contains("\\") && !path.Contains("/"))
              {
                dir = Environment.CurrentDirectory + Path.DirectorySeparatorChar + path;
              }
              FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(dir)).GetFiles();
              foreach (var file in files.OrderBy(f => f.Name))
              {
                Regex r = new Regex(WildcardCharacterHelper.TranslateWildcardToRegex(path));
                if (r.IsMatch(file.FullName) || r.IsMatch(file.Name))
                {
                  GrepFile(file.FullName);
                }
              }
            }
            else
            {
              GrepFile(path);
            }
          }
        }
      }
      catch (ArgumentException ex)
      {
        RaiseCommandLineException(this, new CommandLineException("Path is invalid.", ex));
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void GrepFile(string path)
    {
      if (IsCanGrepFile(path))
      {
        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
          if (!options.IsSetNoMessages)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "No such file -- {0}", file.FullName));
          }
        }
        else
        {
          MatchFile(path);
        }
      }
    }

    private void GrepDirectory(string path)
    {
      if (IsCanGrepDirectory(path))
      {
        DirectoryInfo directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
          if (!options.IsSetNoMessages)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "No such directory -- {0}", directory.FullName));
          }
        }
        else
        {
          FileInfo[] files = directory.GetFiles();
          foreach (var file in files.OrderBy(f => f.Name))
          {
            GrepFile(file.FullName);
          }

          if (options.IsSetRecursive)
          {
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (var item in directories.OrderBy(d => d.Name))
            {
              GrepDirectory(item.FullName);
            }
          }
        }
      }
    }

    private bool IsCanGrepFile(string file)
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
      else if (options.IsSetIncludeFiles)
      {
        Regex r = new Regex(WildcardCharacterHelper.TranslateWildcardToRegex(options.IncludeFilesPattern));
        if (r.IsMatch(file))
        {
          result = true;
        }
      }
      else if (options.IsSetExcludeFiles)
      {
        Regex r = new Regex(WildcardCharacterHelper.TranslateWildcardToRegex(options.ExcludeFilesPattern));
        if (!r.IsMatch(file))
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

    private bool IsCanGrepDirectory(string directory)
    {
      bool result = false;

      if (string.IsNullOrEmpty(directory))
      {
        result = false;
      }
      else if (options.IsSetExcludeDirectories)
      {
        Regex r = new Regex(WildcardCharacterHelper.TranslateWildcardToRegex(options.ExcludeDirectoriesPattern));
        if (!r.IsMatch(directory))
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

      int matchingLineCount = 0;
      for (int i = 0; i < readText.Count; i++)
      {
        if (options.IsSetFixedStrings)
        {
          if (options.IsSetInvertMatch)
          {
            if (!readText[i].Contains(options.RegexPattern))
            {
              matchingLineCount++;
              if (!options.IsSetCount)
              {
                OutputFileData(path, i + 1, readText[i]);
              }
            }
          }
          else
          {
            if (readText[i].Contains(options.RegexPattern))
            {
              matchingLineCount++;
              if (!options.IsSetCount)
              {
                OutputFileData(path, i + 1, readText[i]);
              }
            }
          }
        }
        else
        {
          try
          {
            Regex r = null;
            if (options.IsSetIgnoreCase)
            {
              r = new Regex(options.RegexPattern, RegexOptions.IgnoreCase);
            }
            else
            {
              r = new Regex(options.RegexPattern, RegexOptions.None);
            }

            Match m = r.Match(readText[i]);
            if (options.IsSetInvertMatch)
            {
              if (!m.Success)
              {
                matchingLineCount++;
                if (!options.IsSetCount)
                {
                  OutputFileData(path, i + 1, readText[i]);
                }
              }
            }
            else
            {
              if (m.Success)
              {
                matchingLineCount++;
                if (!options.IsSetCount)
                {
                  OutputFileData(path, i + 1, readText[i]);
                }
              }
            }
          }
          catch (ArgumentException ex)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Bad regex pattern -- {0}, {1}", options.RegexPattern, ex.Message));
          }
        }
      }

      if (options.IsSetFilesWithoutMatch)
      {
        if (matchingLineCount == 0)
        {
          OutputFilesWithoutMatch(path);
        }
      }
      else if (options.IsSetFilesWithMatchs)
      {
        if (matchingLineCount > 0)
        {
          OutputFilesWithMatch(path);
        }
      }
      else if (options.IsSetCount)
      {
        OutputFileMatchingLineCount(path, matchingLineCount);
      }
    }

    private void OutputFilesWithoutMatch(string path)
    {
      string data = string.Format(CultureInfo.CurrentCulture, "{0}{1}", path, Environment.NewLine);
      RaiseCommandLineDataChanged(this, data);
      if (options.IsSetOutputFile)
      {
        WriteOutputFile(data);
      }
    }

    private void OutputFilesWithMatch(string path)
    {
      string data = string.Format(CultureInfo.CurrentCulture, "{0}{1}", path, Environment.NewLine);
      RaiseCommandLineDataChanged(this, data);
      if (options.IsSetOutputFile)
      {
        WriteOutputFile(data);
      }
    }

    private void OutputFileMatchingLineCount(string path, int matchingLineCount)
    {
      string data = string.Format(CultureInfo.CurrentCulture, "{0} : {1} matches.{2}", path, matchingLineCount, Environment.NewLine);
      RaiseCommandLineDataChanged(this, data);
      if (options.IsSetOutputFile)
      {
        WriteOutputFile(data);
      }
    }

    private void OutputFileData(string path, int lineNumber, string lineText)
    {
      string data = string.Empty;

      if (options.IsSetWithFileName)
      {
        if (options.IsSetLineNumber)
        {
          data = string.Format(CultureInfo.CurrentCulture, "{0} : {1, -10} : {2}{3}", path, lineNumber, lineText, Environment.NewLine);
          RaiseCommandLineDataChanged(this, data);
        }
        else
        {
          data = string.Format(CultureInfo.CurrentCulture, "{0} : {1}{2}", path, lineText, Environment.NewLine);
          RaiseCommandLineDataChanged(this, data);
        }
      }
      else if (options.IsSetNoFileName)
      {
        if (options.IsSetLineNumber)
        {
          data = string.Format(CultureInfo.CurrentCulture, "{0, -10} : {1}{2}", lineNumber, lineText, Environment.NewLine);
          RaiseCommandLineDataChanged(this, data);
        }
        else
        {
          data = string.Format(CultureInfo.CurrentCulture, "{0}{1}", lineText, Environment.NewLine);
          RaiseCommandLineDataChanged(this, data);
        }
      }
      else
      {
        if (options.IsSetLineNumber)
        {
          data = string.Format(CultureInfo.CurrentCulture, "{0} : {1, -10} : {2}{3}", path, lineNumber, lineText, Environment.NewLine);
          RaiseCommandLineDataChanged(this, data);
        }
        else
        {
          data = string.Format(CultureInfo.CurrentCulture, "{0} : {1}{2}", path, lineText, Environment.NewLine);
          RaiseCommandLineDataChanged(this, data);
        }
      }

      if (options.IsSetOutputFile)
      {
        WriteOutputFile(data);
      }
    }

    private void WriteOutputFile(string data)
    {
      try
      {
        using (StreamWriter sw = new StreamWriter(options.OutputFile, true, System.Text.Encoding.UTF8))
        {
          sw.AutoFlush = true;
          sw.Write(data);
        }
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
      catch (PathTooLongException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
      catch (SecurityException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Write output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
    }

    private void DeleteOutputFile()
    {
      FileInfo file = new FileInfo(options.OutputFile);

      try
      {
        if (file.Exists)
        {
          file.Delete();
        }

        if (!file.Directory.Exists)
        {
          file.Directory.Create();
        }
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Delete output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
      catch (SecurityException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Delete output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Delete output file failed -- {0}, {1}", options.OutputFile, ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static GrepCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      GrepCommandLineOptions targetOptions = new GrepCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          GrepOptionType optionType = GrepOptions.GetOptionType(arg);
          if (optionType == GrepOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case GrepOptionType.RegexPattern:
              targetOptions.IsSetRegexPattern = true;
              targetOptions.RegexPattern = commandLineOptions.Arguments[arg];
              break;
            case GrepOptionType.File:
              targetOptions.IsSetPath = true;
              targetOptions.FilePaths.Add(commandLineOptions.Arguments[arg]);
              break;
            case GrepOptionType.FixedStrings:
              targetOptions.IsSetFixedStrings = true;
              break;
            case GrepOptionType.IgnoreCase:
              targetOptions.IsSetIgnoreCase = true;
              break;
            case GrepOptionType.InvertMatch:
              targetOptions.IsSetInvertMatch = true;
              break;
            case GrepOptionType.OutputFile:
              targetOptions.IsSetOutputFile = true;
              targetOptions.OutputFile = commandLineOptions.Arguments[arg];
              break;
            case GrepOptionType.Count:
              targetOptions.IsSetCount = true;
              break;
            case GrepOptionType.FilesWithoutMatch:
              targetOptions.IsSetFilesWithoutMatch = true;
              break;
            case GrepOptionType.FilesWithMatchs:
              targetOptions.IsSetFilesWithMatchs = true;
              break;
            case GrepOptionType.NoMessages:
              targetOptions.IsSetNoMessages = true;
              break;
            case GrepOptionType.WithFileName:
              targetOptions.IsSetWithFileName = true;
              break;
            case GrepOptionType.NoFileName:
              targetOptions.IsSetNoFileName = true;
              break;
            case GrepOptionType.LineNumber:
              targetOptions.IsSetLineNumber = true;
              break;
            case GrepOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              break;
            case GrepOptionType.ExcludeFiles:
              targetOptions.IsSetExcludeFiles = true;
              targetOptions.ExcludeFilesPattern = commandLineOptions.Arguments[arg];
              break;
            case GrepOptionType.ExcludeDirectories:
              targetOptions.IsSetExcludeDirectories = true;
              targetOptions.ExcludeDirectoriesPattern = commandLineOptions.Arguments[arg];
              break;
            case GrepOptionType.IncludeFiles:
              targetOptions.IsSetIncludeFiles = true;
              targetOptions.IncludeFilesPattern = commandLineOptions.Arguments[arg];
              break;
            case GrepOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case GrepOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case GrepOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetRegexPattern)
        {
          targetOptions.IsSetRegexPattern = true;
          targetOptions.RegexPattern = commandLineOptions.Parameters.First();

          for (int i = 1; i < commandLineOptions.Parameters.Count; i++)
          {
            targetOptions.IsSetPath = true;
            targetOptions.FilePaths.Add(commandLineOptions.Parameters.ElementAt(i));
          }
        }
        else
        {
          if (!targetOptions.IsSetPath)
          {
            targetOptions.IsSetPath = true;
            foreach (var item in commandLineOptions.Parameters)
            {
              targetOptions.FilePaths.Add(item);
            }
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(GrepCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetRegexPattern || string.IsNullOrEmpty(checkedOptions.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify regex pattern for matching."));
        }
        if (!checkedOptions.IsSetPath || checkedOptions.FilePaths.Count <= 0)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a path for grep."));
        }
        if (checkedOptions.IsSetOutputFile && string.IsNullOrEmpty(checkedOptions.OutputFile))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad output file path format."));
        }
      }
    }

    #endregion
  }
}
