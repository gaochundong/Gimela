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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Gimela.Knifer.CommandLines.Foundation
{
  public abstract class CommandLine : ICommandLine
  {
    #region Ctors

    protected CommandLine(string[] args)
    {
      this.Arguments = new ReadOnlyCollection<string>(args);
    }

    #endregion

    #region Properties

    public ReadOnlyCollection<string> Arguments { get; private set; }

    #endregion

    #region ICommandLine Members

    public virtual void Execute()
    {
      IsExecuting = true;
    }

    public virtual void Terminate()
    {
      IsExecuting = false;
    }

    public bool IsExecuting { get; protected set; }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    protected virtual void Dispose(bool disposing)
    {
    }

    #endregion

    #region Events

    public event EventHandler<CommandLineUsageEventArgs> CommandLineUsage;

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    protected virtual void RaiseCommandLineUsage(object sender, string usage)
    {
      EventHandler<CommandLineUsageEventArgs> handler = CommandLineUsage;
      if (handler != null)
      {
        handler(sender, new CommandLineUsageEventArgs(usage));
      }
    }

    public event EventHandler<CommandLineDataChangedEventArgs> CommandLineDataChanged;

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    protected virtual void RaiseCommandLineDataChanged(object sender, string data)
    {
      EventHandler<CommandLineDataChangedEventArgs> handler = CommandLineDataChanged;
      if (handler != null)
      {
        handler(sender, new CommandLineDataChangedEventArgs(data));
      }
    }

    public event EventHandler<CommandLineExceptionEventArgs> CommandLineException;

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    protected virtual void RaiseCommandLineException(object sender, CommandLineException exception)
    {
      EventHandler<CommandLineExceptionEventArgs> handler = CommandLineException;
      if (handler != null)
      {
        handler(sender, new CommandLineExceptionEventArgs(exception));
      }
    }

    #endregion

    #region Output

    protected virtual void OutputText(string text)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", text, Environment.NewLine));
    }

    protected void OutputFormatText(string format, params object[] args)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture, format, args));
    }

    #endregion

    #region Version

    public virtual string Version
    {
      get
      {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "{0} {1}",
          VersionHelper.GetExecutingAssemblyName(),
          VersionHelper.GetExecutingAssemblyVersion()));

        sb.AppendLine();

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Company  : {0}",
          VersionHelper.GetExecutingAssemblyCompanyName()));

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Product  : {0}",
          VersionHelper.GetExecutingAssemblyProductName()));

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Copyright: {0}",
          VersionHelper.GetExecutingAssemblyCopyright()));

        return sb.ToString();
      }
    }

    #endregion
  }
}
