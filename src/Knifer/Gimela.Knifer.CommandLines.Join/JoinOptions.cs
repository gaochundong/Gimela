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

namespace Gimela.Knifer.CommandLines.Join
{
	internal static class JoinOptions
	{
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<JoinOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static JoinOptions()
		{
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<JoinOptionType, ICollection<string>>();
			Options.Add(JoinOptionType.OutputFile, OutputFileOptions);
			Options.Add(JoinOptionType.Help, HelpOptions);
			Options.Add(JoinOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(JoinOptions.HelpOptions);
			singleOptionList.AddRange(JoinOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	join - join lines of two files on a common field

SYNOPSIS

	join [OPTION]... FILE1 FILE2

DESCRIPTION

	The join command forms, a join of the two relations specified 
	by the lines of file1 and file2.

OPTIONS

	-o, --output=FILE
	{0}{0}The output file which should be outputed.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	join -o 'C:\single.log' a.log b.log
	Join the files a.log and b.log into a 'C:\single.log'. 

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static JoinOptionType GetOptionType(string option)
		{
			JoinOptionType optionType = JoinOptionType.None;

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
