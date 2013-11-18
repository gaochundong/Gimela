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

namespace Gimela.Knifer.CommandLines.Foundation
{
  public static class CommandLineConsole
  {
    public static void OnCommandLineException(object sender, CommandLineExceptionEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");

      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(e.Exception.Message);
      Console.ResetColor();

      ICommandLine commandLine = sender as ICommandLine;
      if (commandLine != null)
      {
        commandLine.Terminate();
      }

      Environment.Exit(0);
    }

    public static void OnCommandLineUsage(object sender, CommandLineUsageEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");

      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine(e.Usage);
      Console.ResetColor();
    }

    public static void OnCommandLineDataChanged(object sender, CommandLineDataChangedEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");

      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write(e.Data);
      Console.ResetColor();
    }
  }
}
