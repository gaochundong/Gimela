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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.Environment
{
  public class EnvironmentCommandLine : CommandLine
  {
    #region Fields

    private EnvironmentCommandLineOptions options;

    #endregion

    #region Constructors

    public EnvironmentCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = EnvironmentOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, EnvironmentOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartEnvironment();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartEnvironment()
    {
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "MachineName", System.Environment.MachineName));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "OSVersion", System.Environment.OSVersion));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "ProcessorCount", System.Environment.ProcessorCount));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "SystemPageSize", System.Environment.SystemPageSize));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "UserDomainName", System.Environment.UserDomainName));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "UserName", System.Environment.UserName));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "Version", System.Environment.Version));
      OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "WorkingSet", System.Environment.WorkingSet));

      IDictionary variables = System.Environment.GetEnvironmentVariables();
      foreach (DictionaryEntry item in variables)
      {
        OutputText(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", item.Key, item.Value));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static EnvironmentCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      EnvironmentCommandLineOptions targetOptions = new EnvironmentCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          EnvironmentOptionType optionType = EnvironmentOptions.GetOptionType(arg);
          if (optionType == EnvironmentOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case EnvironmentOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case EnvironmentOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(EnvironmentCommandLineOptions checkedOptions)
    {
      
    }

    #endregion
  }
}
