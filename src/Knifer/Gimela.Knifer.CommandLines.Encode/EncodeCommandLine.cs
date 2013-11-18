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
using System.Text;
using System.Text.RegularExpressions;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Encode
{
  public class EncodeCommandLine : CommandLine
  {
    #region Fields

    private EncodeCommandLineOptions options;

    #endregion

    #region Constructors

    public EncodeCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = EncodeOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, EncodeOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartEncode();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartEncode()
    {
      try
      {
        if (options.IsSetInputFile)
        {
          string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.InputFile);
          if (WildcardCharacterHelper.IsContainsWildcard(path))
          {
            FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(path)).GetFiles();
            foreach (var file in files)
            {
              Regex r = new Regex(WildcardCharacterHelper.TranslateWildcardToRegex(path));
              if (r.IsMatch(file.FullName) || r.IsMatch(file.Name))
              {
                EncodeFile(file.FullName);
              }
            }
          }
          else
          {
            EncodeFile(path);
          }
        }

        if (options.IsSetDirectory)
        {
          string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
          EncodeDirectory(path);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void EncodeFile(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        if (options.IsSetOutputFile)
        {
          ConvertFile(path, options.OutputFile);
        }
        else
        {
          ConvertFile(path, path);
        }
      }
    }

    private void EncodeDirectory(string path)
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
          EncodeFile(file.FullName);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            EncodeDirectory(item.FullName);
          }
        }
      }
    }

    private void ConvertFile(string fromFile, string toFile)
    {
      string text = string.Empty;
      Stream fileReadStream = null;
      Stream fileWriteStream = null;

      try
      {
        fileReadStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fileReadStream, options.FromEncoding))
        {
          fileReadStream = null;
          text = sr.ReadToEnd();
        }
      }
      finally
      {
        if (fileReadStream != null)
          fileReadStream.Dispose();
      }

      File.Delete(toFile);

      try
      {
        fileWriteStream = new FileStream(toFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        using (StreamWriter sw = new StreamWriter(fileWriteStream, options.ToEncoding))
        {
          fileWriteStream = null;
          sw.AutoFlush = true;
          sw.Write(text);
          sw.Flush();
        }
      }
      finally
      {
        if (fileWriteStream != null)
          fileWriteStream.Dispose();
      }

      OutputFileInformation(string.Format(CultureInfo.CurrentCulture,
        "From {0,-20} file : {1}", options.FromEncoding.EncodingName, fromFile));
      OutputFileInformation(string.Format(CultureInfo.CurrentCulture,
        "To   {0,-20} file : {1}", options.ToEncoding.EncodingName, toFile));
    }

    private void OutputFileInformation(string information)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", information, Environment.NewLine));
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BigEndianUnicode"),
     SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static EncodeCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      EncodeCommandLineOptions targetOptions = new EncodeCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          EncodeOptionType optionType = EncodeOptions.GetOptionType(arg);
          if (optionType == EncodeOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case EncodeOptionType.InputFile:
              targetOptions.IsSetInputFile = true;
              targetOptions.InputFile = commandLineOptions.Arguments[arg];
              break;
            case EncodeOptionType.OutputFile:
              targetOptions.IsSetOutputFile = true;
              targetOptions.OutputFile = commandLineOptions.Arguments[arg];
              break;
            case EncodeOptionType.FromEncoding:
              targetOptions.IsSetFromEncoding = true;
              if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"ASCII")
              {
                targetOptions.FromEncoding = Encoding.ASCII;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF7")
              {
                targetOptions.FromEncoding = Encoding.UTF7;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF8")
              {
                targetOptions.FromEncoding = Encoding.UTF8;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UNICODE")
              {
                targetOptions.FromEncoding = Encoding.Unicode;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF32")
              {
                targetOptions.FromEncoding = Encoding.UTF32;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"BIGENDIANUNICODE")
              {
                targetOptions.FromEncoding = Encoding.BigEndianUnicode;
              }
              else
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid encoding, support ASCII, UTF7, UTF8, UTF32, Unicode, BigEndianUnicode."));
              }
              break;
            case EncodeOptionType.ToEncoding:
              targetOptions.IsSetToEncoding = true;
              if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"ASCII")
              {
                targetOptions.ToEncoding = Encoding.ASCII;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF7")
              {
                targetOptions.ToEncoding = Encoding.UTF7;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF8")
              {
                targetOptions.ToEncoding = Encoding.UTF8;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UNICODE")
              {
                targetOptions.ToEncoding = Encoding.Unicode;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"UTF32")
              {
                targetOptions.ToEncoding = Encoding.UTF32;
              }
              else if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"BIGENDIANUNICODE")
              {
                targetOptions.ToEncoding = Encoding.BigEndianUnicode;
              }
              else
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid encoding, support ASCII, UTF7, UTF8, UTF32, Unicode, BigEndianUnicode."));
              }
              break;
            case EncodeOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case EncodeOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case EncodeOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case EncodeOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetInputFile)
        {
          targetOptions.IsSetInputFile = true;
          targetOptions.InputFile = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(EncodeCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetInputFile && !checkedOptions.IsSetDirectory)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a input file or a directory."));
        }
        if (!checkedOptions.IsSetFromEncoding)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify the input file current encoding."));
        }
        if (!checkedOptions.IsSetToEncoding)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify the output file target encoding."));
        }
        if (checkedOptions.IsSetInputFile && WildcardCharacterHelper.IsContainsWildcard(checkedOptions.InputFile)
          && checkedOptions.IsSetOutputFile)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "output file path has been set, so can only set one input file."));
        }
        if (checkedOptions.IsSetDirectory && checkedOptions.IsSetOutputFile)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "output file path has been set, so can not set a input directory."));
        }
      }
    }

    #endregion
  }
}
