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
using System.Text.RegularExpressions;
using System.Threading;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Split
{
  public class SplitCommandLine : CommandLine
  {
    #region Fields

    private SplitCommandLineOptions options;
    private const int BufferSize = 1024 * 20; // 20K
    private static readonly char[] EscapeChars = new char[] { '\\', '*', '+', '?', '|', '{', '[', '(', ')', '^', '$', '#' };
    private static readonly char[] NameChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    #endregion

    #region Constructors

    public SplitCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = SplitOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, SplitOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartSplit();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartSplit()
    {
      try
      {
        if (options.IsSetFile)
        {
          string file = WildcardCharacterHelper.TranslateWildcardFilePath(options.File);
          string folder = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
          SplitFile(file, folder);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Knifer.CommandLines.Split.SplitCommandLine.OutputText(System.String)")]
    private void SplitFile(string inputFile, string outputFolder)
    {
      FileInfo file = new FileInfo(inputFile);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }

      CreateOutputFolder(outputFolder);

      byte[] buffer = new byte[BufferSize];
      using (Stream input = File.OpenRead(inputFile))
      {
        string lastOutputFileName = string.Empty;
        while (input.Position < input.Length)
        {
          string outputFile = GetNewOutputFileName(outputFolder, lastOutputFileName);
          using (Stream output = File.Create(outputFile))
          {
            int remaining = options.Bytes;        // we write a file for the specified chunk bytes
            int bytesRead;                        // read the input file every time
            while (remaining > 0 && (bytesRead = input.Read(buffer, 0, Math.Min(remaining, BufferSize))) > 0)
            {
              output.Write(buffer, 0, bytesRead); // write data to the output file
              remaining -= bytesRead;             // compute the remaining size for the output file
            }
          }

          if (options.IsSetTimestamp)
          {
            OutputText(string.Format(CultureInfo.CurrentCulture, "Timestamp : {0}, Output file : {1}",
              DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.CurrentCulture), outputFile));
          }
          else
          {
            OutputText(string.Format(CultureInfo.CurrentCulture, "Output file : {0}", outputFile));
          }
          lastOutputFileName = outputFile;

          Thread.Sleep(500);                      // courteous pause, shared hosting environment
        }
      }
    }

    private string GetNewOutputFileName(string outputFolder, string lastOutputFile)
    {
      string outputFile = string.Empty;

      if (string.IsNullOrEmpty(lastOutputFile))
      {
        outputFile += options.Prefix;
        for (int i = 1; i <= options.SuffixLength; i++)
        {
          outputFile += NameChars[0].ToString();
        }
      }
      else
      {
        FileInfo lastfile = new FileInfo(lastOutputFile);

        if (lastfile.Name.Length <= options.Prefix.Length)
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Create new output file name failed -- {0}, {1}", lastfile.Name, "file name length less than prefix."));

        string lastOutputFileNameSuffix = lastfile.Name.Substring(options.Prefix.Length);
        if (lastOutputFileNameSuffix.Length != options.SuffixLength)
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Create new output file name failed -- {0}, {1}, {2}", lastfile.Name, lastOutputFileNameSuffix, "suffix length is not match."));

        char[] lastSuffix = lastOutputFileNameSuffix.ToCharArray();
        if (lastSuffix.First() == NameChars[NameChars.Length - 1]
          && lastSuffix.Last() == NameChars[NameChars.Length - 1])
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Create new output file name failed -- {0}, {1}", lastfile.Name, "suffix length is not enough."));

        char[] newSuffix = lastSuffix;
        for (int i = lastSuffix.Length - 1; i >= 0; i--)
        {
          if (!NameChars.Contains(lastSuffix[i]))
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Create new output file name failed -- {0}, {1}", lastfile.Name, "bad file name."));

          int index = GetNameCharIndex(lastSuffix[i]);
          if (index != NameChars.Length - 1) // not z
          {
            newSuffix[i] = NameChars[index + 1];
            for (int k = i + 1; k < lastSuffix.Length; k++)
            {
              newSuffix[k] = NameChars[0];
            }
            break;
          }
        }

        outputFile = options.Prefix + new string(newSuffix);
      }

      outputFile = Path.Combine(outputFolder, outputFile);
      if (File.Exists(outputFile))
      {
        if (options.IsSetOverwrite)
        {
          File.Delete(outputFile);
        }
        else
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Output file is existent -- {0}", outputFile));
        }
      }

      return outputFile;
    }

    private static int GetNameCharIndex(char c)
    {
      int index = 0;

      for (int h = 0; h < NameChars.Length; h++)
      {
        if (c == NameChars[h])
        {
          index = h;
          break;
        }
      }

      return index;
    }

    private static void CreateOutputFolder(string outputFolder)
    {
      try
      {
        Directory.CreateDirectory(outputFolder);
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Create output folder failed -- {0}, {1}", outputFolder, ex.Message));
      }
      catch (PathTooLongException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Create output folder failed -- {0}, {1}", outputFolder, ex.Message));
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Create output folder failed -- {0}, {1}", outputFolder, ex.Message));
      }
      catch (NotSupportedException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Create output folder failed -- {0}, {1}", outputFolder, ex.Message));
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Create output folder failed -- {0}, {1}", outputFolder, ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static SplitCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      SplitCommandLineOptions targetOptions = new SplitCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          SplitOptionType optionType = SplitOptions.GetOptionType(arg);
          if (optionType == SplitOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case SplitOptionType.File:
              targetOptions.IsSetFile = true;
              targetOptions.File = commandLineOptions.Arguments[arg];
              break;
            case SplitOptionType.Prefix:
              targetOptions.IsSetPrefix = true;
              targetOptions.Prefix = commandLineOptions.Arguments[arg];
              break;
            case SplitOptionType.SuffixLength:
              int suffixLength = 0;
              if (!int.TryParse(commandLineOptions.Arguments[arg], out suffixLength))
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid suffix length."));
              }
              if (suffixLength <= 0 || suffixLength > 10)
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid suffix length."));
              }
              targetOptions.SuffixLength = suffixLength;
              break;
            case SplitOptionType.Bytes:
              targetOptions.Bytes = GetBytesSize(commandLineOptions.Arguments[arg]);
              break;
            case SplitOptionType.Directory:
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case SplitOptionType.Timestamp:
              targetOptions.IsSetTimestamp = true;
              break;
            case SplitOptionType.Overwrite:
              targetOptions.IsSetOverwrite = true;
              break;
            case SplitOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case SplitOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetFile)
        {
          targetOptions.IsSetFile = true;
          targetOptions.File = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static int GetBytesSize(string args)
    {
      if (string.IsNullOrEmpty(args))
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "invalid bytes size."));

      int size = 0;

      Regex r = new Regex(@"^(\d+)([a-zA-Z]*)$", RegexOptions.IgnoreCase);
      Match m = r.Match(args);

      if (m.Success)
      {
        int baseValue = 0;
        if (!int.TryParse(m.Groups[1].Value, out baseValue))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "invalid bytes size."));
        }
        if (baseValue <= 0)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "invalid bytes size."));
        }

        string suffix = m.Groups[2].Value;
        if (string.IsNullOrEmpty(suffix))
        {
          size = baseValue;
        }
        else if (suffix.ToUpperInvariant() == @"B")
        {
          size = baseValue;
        }
        else if (suffix.ToUpperInvariant() == @"K")
        {
          size = baseValue * 1024;
        }
        else if (suffix.ToUpperInvariant() == @"M")
        {
          size = baseValue * 1024 * 1024;
        }
        else
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "invalid bytes size."));
        }
      }
      else
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "invalid bytes size."));
      }

      return size;
    }

    private static void CheckOptions(SplitCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetFile || string.IsNullOrEmpty(checkedOptions.File))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file."));
        }
        if (!checkedOptions.IsSetPrefix || string.IsNullOrEmpty(checkedOptions.Prefix))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a prefix."));
        }
        foreach (var c in EscapeChars)
        {
          if (checkedOptions.Prefix.Contains(c))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "bad prefix format."));
          }
        }
        if (string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a output folder."));
        }
        if (checkedOptions.Bytes <= 0)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify the split bytes."));
        }
      }
    }

    #endregion
  }
}
