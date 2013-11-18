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

namespace Gimela.Knifer.CommandLines.RemoveDirectory
{
	internal static class RemoveDirectoryOptions
	{
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> FixedStringOptions;
		public static readonly ReadOnlyCollection<string> EmptyOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<RemoveDirectoryOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static RemoveDirectoryOptions()
		{
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "D", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "R", "recursive" });
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			FixedStringOptions = new ReadOnlyCollection<string>(new string[] { "f", "F", "fixedstring" });
			EmptyOptions = new ReadOnlyCollection<string>(new string[] { "p", "empty" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<RemoveDirectoryOptionType, ICollection<string>>();
			Options.Add(RemoveDirectoryOptionType.Directory, DirectoryOptions);
			Options.Add(RemoveDirectoryOptionType.Recursive, RecursiveOptions);
			Options.Add(RemoveDirectoryOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(RemoveDirectoryOptionType.FixedString, FixedStringOptions);
			Options.Add(RemoveDirectoryOptionType.Empty, EmptyOptions);
			Options.Add(RemoveDirectoryOptionType.Help, HelpOptions);
			Options.Add(RemoveDirectoryOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(RemoveDirectoryOptions.RecursiveOptions);
			singleOptionList.AddRange(RemoveDirectoryOptions.FixedStringOptions);
			singleOptionList.AddRange(RemoveDirectoryOptions.EmptyOptions);
			singleOptionList.AddRange(RemoveDirectoryOptions.HelpOptions);
			singleOptionList.AddRange(RemoveDirectoryOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	rmdir - remove directories 

SYNOPSIS

	rmdir [OPTION]... DIRECTORY...

DESCRIPTION

	Remove the DIRECTORY(ies).

OPTIONS

	-d, -D, --directory=DIRECTORY
	{0}{0}Specify a directory, a path name of a starting point 
	{0}{0}in the directory hierarchy.
	-r, -R, --recursive
	{0}{0}Remove the contents of directories recursively.
	-e, --regex=PATTERN
	{0}{0}Directory name matches regular expression pattern. 
	-f, -F, --fixedstring
	{0}{0}Indicate that the regex pattern string is fixed string.
	-p, --empty
	{0}{0}Only remove the empty folders.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	rmdir -d . -r -e 'object' -f
	Remove all the 'object' empty directories in the current directory
	and any subdirectory.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static RemoveDirectoryOptionType GetOptionType(string option)
		{
			RemoveDirectoryOptionType optionType = RemoveDirectoryOptionType.None;

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
