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

namespace Gimela.Knifer.CommandLines.AddText
{
	internal static class AddTextOptions
	{
		public static readonly ReadOnlyCollection<string> TopOptions;
		public static readonly ReadOnlyCollection<string> BottomOptions;
		public static readonly ReadOnlyCollection<string> TextOptions;
		public static readonly ReadOnlyCollection<string> FileOptions;
		public static readonly ReadOnlyCollection<string> FromFileOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<AddTextOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static AddTextOptions()
		{
			TopOptions = new ReadOnlyCollection<string>(new string[] { "t", "top" });
			BottomOptions = new ReadOnlyCollection<string>(new string[] { "b", "bottom" });
			TextOptions = new ReadOnlyCollection<string>(new string[] { "s", "string" });
			FileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			FromFileOptions = new ReadOnlyCollection<string>(new string[] { "c", "from" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<AddTextOptionType, ICollection<string>>();
			Options.Add(AddTextOptionType.Top, TopOptions);
			Options.Add(AddTextOptionType.Bottom, BottomOptions);
			Options.Add(AddTextOptionType.Text, TextOptions);
			Options.Add(AddTextOptionType.File, FileOptions);
			Options.Add(AddTextOptionType.FromFile, FromFileOptions);
			Options.Add(AddTextOptionType.Help, HelpOptions);
			Options.Add(AddTextOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(AddTextOptions.TopOptions);
			singleOptionList.AddRange(AddTextOptions.BottomOptions);
			singleOptionList.AddRange(AddTextOptions.HelpOptions);
			singleOptionList.AddRange(AddTextOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	addtext - add text into file 

SYNOPSIS

	addtext [OPTION] FILE STRING

DESCRIPTION

	Add text into a specified file.

OPTIONS

	-s, --string=TEXT
	{0}{0}The text string that be added into a file.
	-f, --file=FILE
	{0}{0}The specified file.
	-t, --top
	{0}{0}Insert text string into the top of a file. 
	-b, --bottom
	{0}{0}Append text string into the bottom of a file. 
	-c, --from=FILE
	{0}{0}The text string comes from this file.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	addtext -t -f myfile.txt -s chundong
	Add 'chundong' into the top of file 'myfile.txt'.

	addtext -t -f myfile.txt -c ./1.txt
	Add content of './1.txt' into the top of file 'myfile.txt'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static AddTextOptionType GetOptionType(string option)
		{
			AddTextOptionType optionType = AddTextOptionType.None;

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
