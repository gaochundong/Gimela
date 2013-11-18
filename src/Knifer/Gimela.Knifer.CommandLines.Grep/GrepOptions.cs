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

namespace Gimela.Knifer.CommandLines.Grep
{
	internal static class GrepOptions
	{
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> FileOptions;
		public static readonly ReadOnlyCollection<string> FixedStringsOptions;
		public static readonly ReadOnlyCollection<string> IgnoreCaseOptions;
		public static readonly ReadOnlyCollection<string> InvertMatchOptions;
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> CountOptions;
		public static readonly ReadOnlyCollection<string> FilesWithoutMatchOptions;
		public static readonly ReadOnlyCollection<string> FilesWithMatchsOptions;
		public static readonly ReadOnlyCollection<string> NoMessagesOptions;
		public static readonly ReadOnlyCollection<string> WithFileNameOptions;
		public static readonly ReadOnlyCollection<string> NoFileNameOptions;
		public static readonly ReadOnlyCollection<string> LineNumberOptions;
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> ExcludeFilesOptions;
		public static readonly ReadOnlyCollection<string> ExcludeDirectoriesOptions;
		public static readonly ReadOnlyCollection<string> IncludeFilesOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<GrepOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static GrepOptions()
		{
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			FileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			FixedStringsOptions = new ReadOnlyCollection<string>(new string[] { "F", "fixed-strings" });
			IgnoreCaseOptions = new ReadOnlyCollection<string>(new string[] { "i", "ignore-case" });
			InvertMatchOptions = new ReadOnlyCollection<string>(new string[] { "V", "invert-match" });
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			CountOptions = new ReadOnlyCollection<string>(new string[] { "c", "count" });
			FilesWithoutMatchOptions = new ReadOnlyCollection<string>(new string[] { "L", "files-without-match" });
			FilesWithMatchsOptions = new ReadOnlyCollection<string>(new string[] { "l", "files-with-matches" });
			NoMessagesOptions = new ReadOnlyCollection<string>(new string[] { "s", "no-messages" });
			WithFileNameOptions = new ReadOnlyCollection<string>(new string[] { "P", "with-filename" });
			NoFileNameOptions = new ReadOnlyCollection<string>(new string[] { "p", "no-filename" });
			LineNumberOptions = new ReadOnlyCollection<string>(new string[] { "n", "line-number" });
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "directory" });
			ExcludeFilesOptions = new ReadOnlyCollection<string>(new string[] { "exclude" });
			ExcludeDirectoriesOptions = new ReadOnlyCollection<string>(new string[] { "exclude-dir" });
			IncludeFilesOptions = new ReadOnlyCollection<string>(new string[] { "include" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<GrepOptionType, ICollection<string>>();
			Options.Add(GrepOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(GrepOptionType.File, FileOptions);
			Options.Add(GrepOptionType.FixedStrings, FixedStringsOptions);
			Options.Add(GrepOptionType.IgnoreCase, IgnoreCaseOptions);
			Options.Add(GrepOptionType.InvertMatch, InvertMatchOptions);
			Options.Add(GrepOptionType.OutputFile, OutputFileOptions);
			Options.Add(GrepOptionType.Count, CountOptions);
			Options.Add(GrepOptionType.FilesWithoutMatch, FilesWithoutMatchOptions);
			Options.Add(GrepOptionType.FilesWithMatchs, FilesWithMatchsOptions);
			Options.Add(GrepOptionType.NoMessages, NoMessagesOptions);
			Options.Add(GrepOptionType.WithFileName, WithFileNameOptions);
			Options.Add(GrepOptionType.NoFileName, NoFileNameOptions);
			Options.Add(GrepOptionType.LineNumber, LineNumberOptions);
			Options.Add(GrepOptionType.Directory, DirectoryOptions);
			Options.Add(GrepOptionType.ExcludeFiles, ExcludeFilesOptions);
			Options.Add(GrepOptionType.ExcludeDirectories, ExcludeDirectoriesOptions);
			Options.Add(GrepOptionType.IncludeFiles, IncludeFilesOptions);
			Options.Add(GrepOptionType.Recursive, RecursiveOptions);
			Options.Add(GrepOptionType.Help, HelpOptions);
			Options.Add(GrepOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(GrepOptions.FixedStringsOptions);
			singleOptionList.AddRange(GrepOptions.IgnoreCaseOptions);
			singleOptionList.AddRange(GrepOptions.InvertMatchOptions);
			singleOptionList.AddRange(GrepOptions.CountOptions);
			singleOptionList.AddRange(GrepOptions.FilesWithoutMatchOptions);
			singleOptionList.AddRange(GrepOptions.FilesWithMatchsOptions);
			singleOptionList.AddRange(GrepOptions.NoMessagesOptions);
			singleOptionList.AddRange(GrepOptions.WithFileNameOptions);
			singleOptionList.AddRange(GrepOptions.NoFileNameOptions);
			singleOptionList.AddRange(GrepOptions.LineNumberOptions);
			singleOptionList.AddRange(GrepOptions.DirectoryOptions);
			singleOptionList.AddRange(GrepOptions.RecursiveOptions);
			singleOptionList.AddRange(GrepOptions.HelpOptions);
			singleOptionList.AddRange(GrepOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	grep - search file(s) for lines that match a given pattern

SYNOPSIS

	grep [OPTIONS] PATTERN [FILE...]
	grep [OPTIONS] [-e PATTERN | -f FILE] [FILE...]

DESCRIPTION

	The grep utility searches text files for a pattern and prints 
	all lines that contain that pattern. 

	Be careful using the characters $, *, [, ^, |, (, ), and \ in 
	the PATTERN because they are also meaningful to the shell. It 
	is safest to enclose the entire PATTERN in single quotes '...'.

	The file name is printed before each line found if there is 
	more than one input file.

OPTIONS

	Matching Control

	-e PATTERN, --regex=PATTERN
	{0}{0}Use PATTERN as the pattern. This can be used to specify 
	{0}{0}multiple search patterns, or to protect a pattern beginning 
	{0}{0}with a hyphen (-).
	-f FILE, --file=FILE
	{0}{0}Obtain patterns from FILE, one per line. The empty file 
	{0}{0}contains zero patterns, and therefore matches nothing.
	-F, --fixed-strings
	{0}{0}Interpret PATTERN as a list of fixed strings, separated by 
	{0}{0}newlines, any of which is to be matched.
	-i, --ignore-case
	{0}{0}Ignore case distinctions in both the PATTERN and the input 
	{0}{0}files.
	-V, --invert-match
	{0}{0}Invert the sense of matching, to select non-matching lines.

	General Output Control

	-c, --count
	{0}{0}Suppress normal output; instead print a count of matching lines 
	{0}{0}for each input file. With the -v, --invert-match option, 
	{0}{0}count non-matching lines.
	-L, --files-without-match
	{0}{0}Suppress normal output; instead print the name of each input 
	{0}{0}file from which no output would normally have been printed.
	-l, --files-with-matches
	{0}{0}Suppress normal output; instead print the name of each input 
	{0}{0}file from which output would normally have been printed.
	-s, --no-messages
	{0}{0}Suppress error messages about nonexistent or unreadable files. 
	-o, --output=FILE
	{0}{0}Write all the output lines into target output file. 

	Output Line Prefix Control

	-P, --with-filename
	{0}{0}Print the file name for each match.
	-p, --no-filename
	{0}{0}Suppress the prefixing of file names on output.
	-n, --line-number
	{0}{0}Prefix each line of output with the 1-based line number within 
	{0}{0}its input file.

	File and Directory Selection

	-d, --directory
	{0}{0}If an input file is a directory, read files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	--exclude=GLOB
	{0}{0}Skip files whose base name matches GLOB (using wildcard matching). 
	{0}{0}A file-name glob can use *, ? as wildcards, and \ to 
	{0}{0}quote a wildcard or backslash character literally.
	--exclude-dir=DIR
	{0}{0}Exclude directories matching the pattern DIR from recursive 
	{0}{0}searches.
	--include=GLOB
	{0}{0}Search only files whose base name matches GLOB (using wildcard 
	{0}{0}matching as described under --exclude).

	Other Options

	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	grep 'chundong' ./*.txt
	Search all .txt files in the current directory, and print the 
	file name and lines that contain 'chundong'.

	grep 'chundong' 'C:\Logs' -d -F -n
	Search all files in 'C:\Logs' directory, and print the 
	file name and lines number that contain 'chundong'.

	grep 'chundong' 'C:\Logs' -d -F -c
	Search all files in 'C:\Logs' directory, and print the 
	file name and lines count that contain 'chundong'.

	grep -e 'chundong.*' 'C:\Logs' -d -c
	Search all files in 'C:\Logs' directory, and print the 
	file name and lines count that match regex 'chundong.*'.

	grep -e 'Exception' -d 'C:\Logs' -o 'C:\exceptions.log' -n -F
	Search all files in 'C:\Logs' directory, and print the 
	file name and lines number that match string 'Exception'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static GrepOptionType GetOptionType(string option)
		{
			GrepOptionType optionType = GrepOptionType.None;

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
