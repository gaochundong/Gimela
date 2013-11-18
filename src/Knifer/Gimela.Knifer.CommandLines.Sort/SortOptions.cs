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

namespace Gimela.Knifer.CommandLines.Sort
{
	internal static class SortOptions
	{
		public static readonly ReadOnlyCollection<string> InputFileOptions;
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> ReverseOrderOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<SortOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static SortOptions()
		{
			InputFileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			ReverseOrderOptions = new ReadOnlyCollection<string>(new string[] { "r", "reverse" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<SortOptionType, ICollection<string>>();
			Options.Add(SortOptionType.InputFile, InputFileOptions);
			Options.Add(SortOptionType.OutputFile, OutputFileOptions);
			Options.Add(SortOptionType.ReverseOrder, ReverseOrderOptions);
			Options.Add(SortOptionType.Help, HelpOptions);
			Options.Add(SortOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(SortOptions.ReverseOrderOptions);
			singleOptionList.AddRange(SortOptions.HelpOptions);
			singleOptionList.AddRange(SortOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	sort - sort lines of a text file

SYNOPSIS

	sort [OPTION] FILE

DESCRIPTION

	The Sort utility program sorts the lines in a text file.

OPTIONS

	-f, --file=FILE
	{0}{0}The FILE that needs to be sorted.
	-o, --output=FILE
	{0}{0}The FILE represents the output file.
	-r, --reverse
	{0}{0}Sorts in reverse order.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	sort -r file.txt
	Sort the file 'file.txt' in reverse order.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static SortOptionType GetOptionType(string option)
		{
			SortOptionType optionType = SortOptionType.None;

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
