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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Knifer.CommandLines.SyncCopy
{
  internal static class SyncCopyOptions
  {
    public static readonly ReadOnlyCollection<string> FromDirectoryOptions;
    public static readonly ReadOnlyCollection<string> ToDirectoryOptions;
    public static readonly ReadOnlyCollection<string> RecursiveOptions;
    public static readonly ReadOnlyCollection<string> HelpOptions;
    public static readonly ReadOnlyCollection<string> VersionOptions;

    public static readonly IDictionary<SyncCopyOptionType, ICollection<string>> Options;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static SyncCopyOptions()
    {
      FromDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "f", "from" });
      ToDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "t", "to" });
      RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
      HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
      VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

      Options = new Dictionary<SyncCopyOptionType, ICollection<string>>();
      Options.Add(SyncCopyOptionType.FromDirectory, FromDirectoryOptions);
      Options.Add(SyncCopyOptionType.ToDirectory, ToDirectoryOptions);
      Options.Add(SyncCopyOptionType.Recursive, RecursiveOptions);
      Options.Add(SyncCopyOptionType.Help, HelpOptions);
      Options.Add(SyncCopyOptionType.Version, VersionOptions);
    }

    public static List<string> GetSingleOptions()
    {
      List<string> singleOptionList = new List<string>();

      singleOptionList.AddRange(SyncCopyOptions.RecursiveOptions);
      singleOptionList.AddRange(SyncCopyOptions.HelpOptions);
      singleOptionList.AddRange(SyncCopyOptions.VersionOptions);

      return singleOptionList;
    }

    #region Usage

    public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	synccopy - copy files from source to target align with target layout

SYNOPSIS

	synccopy [OPTION]

DESCRIPTION

	Copy files from source to target align with target layout.

OPTIONS

	-f, --from=DIRECTORY
	{0}{0}The DIRECTORY that files come from.
	-t, --to=DIRECTORY
	{0}{0}The DIRECTORY that files target.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	synccopy -f 'C:\Libs' -t 'D:\Libs' -r
	Copy those files which are existing in 'D:\Libs' from 
	source folder 'C:\Libs'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

    #endregion

    public static SyncCopyOptionType GetOptionType(string option)
    {
      SyncCopyOptionType optionType = SyncCopyOptionType.None;

      foreach (var pair in Options)
      {
        foreach (var item in pair.Value)
        {
          if (item == option)
          {
            optionType = pair.Key;
            break;
          }
        }
      }

      return optionType;
    }
  }
}
