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

namespace Gimela.Knifer.CommandLines.Replace
{
	internal static class ReplaceOptions
	{
		public static readonly ReadOnlyCollection<string> InputFileOptions;
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> FromTextOptions;
		public static readonly ReadOnlyCollection<string> ToTextOptions;
		public static readonly ReadOnlyCollection<string> InputDirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> ExtensionOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<ReplaceOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static ReplaceOptions()
		{
			InputFileOptions = new ReadOnlyCollection<string>(new string[] { "i", "inputfile" });
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "outputfile" });
			FromTextOptions = new ReadOnlyCollection<string>(new string[] { "f", "fromstring" });
			ToTextOptions = new ReadOnlyCollection<string>(new string[] { "t", "tostring" });
			InputDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "dir", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			ExtensionOptions = new ReadOnlyCollection<string>(new string[] { "e", "extension" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<ReplaceOptionType, ICollection<string>>();
			Options.Add(ReplaceOptionType.InputFile, InputFileOptions);
			Options.Add(ReplaceOptionType.OutputFile, OutputFileOptions);
			Options.Add(ReplaceOptionType.FromText, FromTextOptions);
			Options.Add(ReplaceOptionType.ToText, ToTextOptions);
			Options.Add(ReplaceOptionType.InputDirectory, InputDirectoryOptions);
			Options.Add(ReplaceOptionType.Recursive, RecursiveOptions);
			Options.Add(ReplaceOptionType.Extension, ExtensionOptions);
			Options.Add(ReplaceOptionType.Help, HelpOptions);
			Options.Add(ReplaceOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(ReplaceOptions.RecursiveOptions);
			singleOptionList.AddRange(ReplaceOptions.HelpOptions);
			singleOptionList.AddRange(ReplaceOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	replace - changes strings in place in a file

SYNOPSIS

	replace [OPTION] FILE STRING1 STRING2

DESCRIPTION

	The replace utility program changes strings in place in files.

OPTIONS

	-i, --inputfile=FILE
	{0}{0}The FILE represents the input file.
	-o, --outputfile=FILE
	{0}{0}The FILE represents the output file.
	-f, --from=STRING
	{0}{0}Represents a string to look for and to represents its replacement.
	-t, --to=STRING
	{0}{0}Represents a string to replace. 
	-d, --dir, --directory=DIRECTORY
	{0}{0}The input directory, read files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-e, --extension=FILTER_EXTENSION_LIST
	{0}{0}Filter the files in input directory with FILTER_EXTENSION_LIST.
	{0}{0}Splits filter string to list with ',' or ';'.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	replace -i example.txt -f chundong -t gaochundong
	Replace the word 'chundong' with the word 'gaochundong' in the example.txt file.

	replace -f chundong -t gaochundong -d 'C:\Projects' -e '.cs,.csproj,.sln' -r
	Search files in directory 'C:\Projects', match file extensions in '.cs,.csproj,.sln',
	and replace the word 'chundong' with the word 'gaochundong' in the all matched file.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static ReplaceOptionType GetOptionType(string option)
		{
			ReplaceOptionType optionType = ReplaceOptionType.None;

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
