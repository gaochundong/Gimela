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

namespace Gimela.Knifer.CommandLines.Find
{
	internal static class FindOptions
	{
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<FindOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static FindOptions()
		{
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<FindOptionType, ICollection<string>>();
			Options.Add(FindOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(FindOptionType.Directory, DirectoryOptions);
			Options.Add(FindOptionType.Recursive, RecursiveOptions);
			Options.Add(FindOptionType.Help, HelpOptions);
			Options.Add(FindOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(FindOptions.RecursiveOptions);
			singleOptionList.AddRange(FindOptions.HelpOptions);
			singleOptionList.AddRange(FindOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	find - search for files in a directory hierarchy

SYNOPSIS

	find [PATH] [EXPRESSION]

DESCRIPTION

	Find utility searches one or more files assuming that 
	you know their approximate filenames.

OPTIONS

	-e, --regex=PATTERN
	{0}{0}File name matches regular expression pattern. 
	{0}{0}This is a match on the whole path, not a search. 
	{0}{0}For example, to match a file named './fubar3', 
	{0}{0}you can use the regular expression `.*bar.' 
	{0}{0}or `.*b.*3', but not `b.*r3'.
	-d, --directory
	{0}{0}Specify a directory, a path name of a starting point 
	{0}{0}in the directory hierarchy.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	find . a.txt
	In the above command the system would search for 
	any file named a.txt in the current directory.

	find -r . *.txt
	In the above example the system would search for 
	any file ending with file in the current directory 
	and any subdirectory.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static FindOptionType GetOptionType(string option)
		{
			FindOptionType optionType = FindOptionType.None;

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
