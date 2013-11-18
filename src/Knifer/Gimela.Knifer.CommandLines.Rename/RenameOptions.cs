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

namespace Gimela.Knifer.CommandLines.Rename
{
	internal static class RenameOptions
	{
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> InputDirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> OutputPatternOptions;
		public static readonly ReadOnlyCollection<string> FolderOptions;
		public static readonly ReadOnlyCollection<string> ExcludeOptions;
		public static readonly ReadOnlyCollection<string> PadStringOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<RenameOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static RenameOptions()
		{
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			InputDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "dir", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			OutputPatternOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			FolderOptions = new ReadOnlyCollection<string>(new string[] { "f", "folder" });
			ExcludeOptions = new ReadOnlyCollection<string>(new string[] { "x", "exclude" });
			PadStringOptions = new ReadOnlyCollection<string>(new string[] { "p", "pad" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<RenameOptionType, ICollection<string>>();
			Options.Add(RenameOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(RenameOptionType.InputDirectory, InputDirectoryOptions);
			Options.Add(RenameOptionType.Recursive, RecursiveOptions);
			Options.Add(RenameOptionType.OutputPattern, OutputPatternOptions);
			Options.Add(RenameOptionType.Folder, FolderOptions);
			Options.Add(RenameOptionType.Exclude, ExcludeOptions);
			Options.Add(RenameOptionType.PadString, PadStringOptions);
			Options.Add(RenameOptionType.Help, HelpOptions);
			Options.Add(RenameOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(RenameOptions.RecursiveOptions);
			singleOptionList.AddRange(RenameOptions.FolderOptions);
			singleOptionList.AddRange(RenameOptions.HelpOptions);
			singleOptionList.AddRange(RenameOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	rename - rename files

SYNOPSIS

	extract [OPTION] [REGEX] [INPUT_DIRECTORY] [PAD_STRING]

DESCRIPTION

	Rename will rename the specified files specified format.

OPTIONS

	-e PATTERN, --regex=PATTERN
	{0}{0}Use PATTERN as the file matched pattern.
	-d, --dir, --directory=DIRECTORY
	{0}{0}The input directory, read files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-o PATTERN, --output=PATTERN
	{0}{0}Use PATTERN as the output file name pattern.
	-f, --folder
	{0}{0}Also rename the folder.
	-x, --exclude=EXCLUDE_LIST
	{0}{0}Exclude the specified directory names.
	{0}{0}Splits filter string to list with ',' or ';'.
	-p, --pad=PAD_STRING
	{0}{0}Pad a number with leading zeros.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	rename.exe -e '^(.*\.)(\d+)(\.log)$' -d 'C:\Logs' -p '00000'
	Search all files in directory 'C:\Logs', match the regex '^.*\.(\d+)\.log$',
	and rename the files pad the integer with leading zeros '00000'.

	rename.exe -e '.chundong.' -d 'C:\Logs' -o '.good.' -x 'bin,obj' -f -r
	Search all files in directory 'C:\Logs', match the pattern '.chundong.',
	and rename the files and folders repalce '.chundong.' to '.good.'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static RenameOptionType GetOptionType(string option)
		{
			RenameOptionType optionType = RenameOptionType.None;

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
