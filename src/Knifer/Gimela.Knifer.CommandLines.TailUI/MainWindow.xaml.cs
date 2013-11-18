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
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Gimela.Knifer.CommandLines.Foundation;
using Gimela.Knifer.CommandLines.Tail;

namespace Gimela.Knifer.CommandLines.TailUI
{
  public partial class MainWindow : Window
  {
    private const string TailCommand = @"Tail";
    private const string CancelCommand = @"Cancel";

    private TailCommandLine tail = null;
    private static readonly int maxLineCount = 5000;
    private static readonly object tailLocker = new object();

    public MainWindow()
    {
      InitializeComponent();
    }

    private void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.Filter = "Log files (*.log)|*.log|Text documents (*.txt)|*.txt|All files (*.*)|*.*";
      dlg.FilterIndex = 3;

      Nullable<bool> result = dlg.ShowDialog();
      if (result == true)
      {
        tbFileName.Text = dlg.FileName;
        OnTailButtonClick(sender, new RoutedEventArgs());
      }
    }

    private void OnTailButtonClick(object sender, RoutedEventArgs e)
    {
      if (btnTail.Content.ToString() == TailCommand)
      {
        if (string.IsNullOrEmpty(tbFileName.Text))
        {
          MessageBox.Show(this, "Please specify a file for tailing.", this.Title,
            MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
          return;
        }

        btnTail.Content = CancelCommand;
        tbFileName.IsEnabled = false;
        btnOpenFile.IsEnabled = false;
        tbFileData.Document.Blocks.Clear();

        try
        {
          string[] args = new string[] { 
            @"-F",
            tbFileName.Text
          };

          tail = new TailCommandLine(args);
          tail.CommandLineException += new EventHandler<CommandLineExceptionEventArgs>(OnCommandLineException);
          tail.CommandLineDataChanged += new EventHandler<CommandLineDataChangedEventArgs>(OnCommandLineDataChanged);

          ThreadPool.QueueUserWorkItem((WaitCallback)TailExecuter, tail);
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, this.Title,
            MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
      }
      else
      {
        btnTail.Content = TailCommand;
        tbFileName.IsEnabled = true;
        btnOpenFile.IsEnabled = true;

        try
        {
          if (tail != null)
          {
            tail.CommandLineException -= new EventHandler<CommandLineExceptionEventArgs>(OnCommandLineException);
            tail.CommandLineDataChanged -= new EventHandler<CommandLineDataChangedEventArgs>(OnCommandLineDataChanged);
            tail.Terminate();
            tail.Dispose();
            tail = null;
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, this.Title,
            MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
      }
    }

    private void TailExecuter(object state)
    {
      TailCommandLine tcl = (TailCommandLine)state;
      tcl.Execute();

      while (tail != null && tail.IsExecuting) ;
    }

    private void OnCommandLineException(object sender, CommandLineExceptionEventArgs e)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal,
        new Action(() =>
        {
          lock (tailLocker)
          {
            tbFileData.AppendText(e.Exception.Message);
            tbFileData.ScrollToEnd();

            OnTailButtonClick(sender, new RoutedEventArgs());
          }
        }));
    }

    private void OnCommandLineDataChanged(object sender, CommandLineDataChangedEventArgs e)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal,
        new Action(() =>
        {
          lock (tailLocker)
          {
            if (tbFileData.Document.Blocks.Count > maxLineCount)
            {
              tbFileData.Document.Blocks.Clear();
            }

            string[] list = e.Data.TrimEnd(new char[] { '\n' }).Replace("\r", "").Split(new char[] { '\n' });
            for (int i = 0; i < list.Length; i++)
            {
              if (list[i].ToUpperInvariant().Contains(@"EXCEPTION"))
              {
                tbFileData.Document.Blocks.Add(new Paragraph(new Run(list[i]) { Foreground = Brushes.Red }));
                tbFileData.Document.Blocks.Add(new Paragraph(new Run()));
              }
              else if (list[i].ToUpperInvariant().Contains(@"CANNOT"))
              {
                tbFileData.Document.Blocks.Add(new Paragraph(new Run(list[i]) { Foreground = Brushes.Yellow }));
                tbFileData.Document.Blocks.Add(new Paragraph(new Run()));
              }
              else if (list[i].ToUpperInvariant().Contains(@"CAN NOT"))
              {
                tbFileData.Document.Blocks.Add(new Paragraph(new Run(list[i]) { Foreground = Brushes.Yellow }));
                tbFileData.Document.Blocks.Add(new Paragraph(new Run()));
              }
              else if (list[i].ToUpperInvariant().Contains(@"COULD NOT"))
              {
                tbFileData.Document.Blocks.Add(new Paragraph(new Run(list[i]) { Foreground = Brushes.Yellow }));
                tbFileData.Document.Blocks.Add(new Paragraph(new Run()));
              }
              else
              {
                if (i == list.Length - 1 && !e.Data.EndsWith("\n", StringComparison.CurrentCulture))
                {
                  tbFileData.AppendText(list[i]);
                }
                else
                {
                  if (!string.IsNullOrEmpty(list[i]))
                  {
                    tbFileData.AppendText(list[i]);
                  }
                  tbFileData.AppendText(Environment.NewLine);
                }
              }
            }

            tbFileData.ScrollToEnd();
          }
        }));
    }
  }
}
