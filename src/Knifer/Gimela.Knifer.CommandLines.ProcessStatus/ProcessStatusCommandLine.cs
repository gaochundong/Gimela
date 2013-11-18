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
using Gimela.Knifer.CommandLines.Foundation;

namespace Gimela.Knifer.CommandLines.ProcessStatus
{
  public class ProcessStatusCommandLine : CommandLine
  {
    #region Fields

    private ProcessStatusCommandLineOptions options;

    #endregion

    #region Constructors

    public ProcessStatusCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = ProcessStatusOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, ProcessStatusOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartProcessStatus();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartProcessStatus()
    {
      try
      {
        ProcessStatus();
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void ProcessStatus()
    {
      try
      {
        Process[] processes = Process.GetProcesses();

        OutputFormatText("{0, -8}\t{1, -10}\t{2, 13}\t{3, 13}\t{4, 8}\t{5, -30}{6}", "PID", "Status", "WorkingSet", "VirtualMemory", "CPU Time", "Process Name", Environment.NewLine);
        OutputFormatText("{0, -8}\t{1, -10}\t{2, 13}\t{3, 13}\t{4, 8}\t{5, -30}{6}", "--------", "--------", "-------------", "-------------", "--------", "-------------------", Environment.NewLine);

        foreach (Process p in processes.OrderBy(f => f.ProcessName))
        {
          try
          {
            TimeSpan tpt = p.TotalProcessorTime;
            string processorTime = string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", tpt.Hours, tpt.Minutes, tpt.Seconds);

            OutputFormatText("{0, -8}\t{1, -10}\t{2, 13}\t{3, 13}\t{4, -8}\t{5, -30}{6}",
              p.Id,
              p.Responding ? "Running" : "Idle",
              p.WorkingSet64,
              p.VirtualMemorySize64,
              processorTime,
              p.ProcessName,
              Environment.NewLine);
          }
          catch (Win32Exception)
          {
            continue; // catch access is denied
          }
        }
      }
      catch (PlatformNotSupportedException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}", ex.Message));
      }
      catch (InvalidOperationException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}", ex.Message));
      }
      catch (NotSupportedException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}", ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static ProcessStatusCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      ProcessStatusCommandLineOptions targetOptions = new ProcessStatusCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          ProcessStatusOptionType optionType = ProcessStatusOptions.GetOptionType(arg);
          if (optionType == ProcessStatusOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case ProcessStatusOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case ProcessStatusOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(ProcessStatusCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        ;
      }
    }

    #endregion
  }
}
