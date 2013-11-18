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

namespace Gimela.Knifer.CommandLines.Extract
{
	internal static class ExtractOptions
	{
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> InputDirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> InputFileExtensionFilterOptions;
		public static readonly ReadOnlyCollection<string> OutputDirectoryOptions;
		public static readonly ReadOnlyCollection<string> OutputFileExtensionOptions;
		public static readonly ReadOnlyCollection<string> ExcludeOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<ExtractOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static ExtractOptions()
		{
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			InputDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "dir", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			InputFileExtensionFilterOptions = new ReadOnlyCollection<string>(new string[] { "f", "filter" });
			OutputDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			OutputFileExtensionOptions = new ReadOnlyCollection<string>(new string[] { "t", "extension" });
			ExcludeOptions = new ReadOnlyCollection<string>(new string[] { "x", "exclude" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<ExtractOptionType, ICollection<string>>();
			Options.Add(ExtractOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(ExtractOptionType.InputDirectory, InputDirectoryOptions);
			Options.Add(ExtractOptionType.Recursive, RecursiveOptions);
			Options.Add(ExtractOptionType.InputFileExtensionFilter, InputFileExtensionFilterOptions);
			Options.Add(ExtractOptionType.OutputDirectory, OutputDirectoryOptions);
			Options.Add(ExtractOptionType.OutputFileExtension, OutputFileExtensionOptions);
			Options.Add(ExtractOptionType.Exclude, ExcludeOptions);
			Options.Add(ExtractOptionType.Help, HelpOptions);
			Options.Add(ExtractOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(ExtractOptions.RecursiveOptions);
			singleOptionList.AddRange(ExtractOptions.HelpOptions);
			singleOptionList.AddRange(ExtractOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	extract - extract regex matches to files

SYNOPSIS

	extract [OPTION] [REGEX] [INPUT_DIRECTORY] [OUTPUT_DIRECTORY]

DESCRIPTION

	Extract utility program extract regex matches to files.

OPTIONS

	-e PATTERN, --regex=PATTERN
	{0}{0}Use PATTERN as the pattern.
	-d, --dir, --directory=DIRECTORY
	{0}{0}The input directory, read files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-f, --filter=FILTER_EXTENSION_LIST
	{0}{0}Filter the files in input directory with FILTER_EXTENSION_LIST.
	{0}{0}Splits filter string to list with ',' or ';'.
	-o, --output=FILE
	{0}{0}The FILE represents the output file.
	-t, --extension=OUTPUT_EXTENSION
	{0}{0}Name all the output files with extension OUTPUT_EXTENSION.
	-x, --exclude=EXCLUDE_LIST
	{0}{0}Filter and exclude the matches string with EXCLUDE_LIST.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	extract -e 'Name\[(.+?)\]' -d 'C:\Logs' -o 'C:\Names'
	Search all files in directory 'C:\Logs', match the regex 'Name\[(.+?)\]',
	and named a file with matched string like 'Name[chundong]' to the
	file 'C:\Names\chundong.txt'. And then matches lines in the 'C:\Logs',
	append all the lines with 'Name[chundong]' into this new file.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static ExtractOptionType GetOptionType(string option)
		{
			ExtractOptionType optionType = ExtractOptionType.None;

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
