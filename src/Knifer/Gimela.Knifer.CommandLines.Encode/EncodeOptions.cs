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

namespace Gimela.Knifer.CommandLines.Encode
{
	internal static class EncodeOptions
	{
		public static readonly ReadOnlyCollection<string> InputFileOptions;
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> FromEncodingOptions;
		public static readonly ReadOnlyCollection<string> ToEncodingOptions;
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<EncodeOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static EncodeOptions()
		{
			InputFileOptions = new ReadOnlyCollection<string>(new string[] { "i", "input-file" });
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "out-file" });
			FromEncodingOptions = new ReadOnlyCollection<string>(new string[] { "f", "from-encoding" });
			ToEncodingOptions = new ReadOnlyCollection<string>(new string[] { "t", "to-encoding" });
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<EncodeOptionType, ICollection<string>>();
			Options.Add(EncodeOptionType.InputFile, InputFileOptions);
			Options.Add(EncodeOptionType.OutputFile, OutputFileOptions);
			Options.Add(EncodeOptionType.FromEncoding, FromEncodingOptions);
			Options.Add(EncodeOptionType.ToEncoding, ToEncodingOptions);
			Options.Add(EncodeOptionType.Directory, DirectoryOptions);
			Options.Add(EncodeOptionType.Recursive, RecursiveOptions);
			Options.Add(EncodeOptionType.Help, HelpOptions);
			Options.Add(EncodeOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(EncodeOptions.RecursiveOptions);
			singleOptionList.AddRange(EncodeOptions.HelpOptions);
			singleOptionList.AddRange(EncodeOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	encode - convert encoding of given files from one encoding to another

SYNOPSIS

	encode [OPTIONS] [FILE...]

DESCRIPTION

	The encode utility converts the encoding of characters in input 
	file from one coded character set to another.

OPTIONS

	-i FILE, --input-file=FILE
	{0}{0}Specify the input file.
	-o FILE, --output-file=FILE
	{0}{0}Specify the output file.
	-f ENCODING, --from-encoding=ENCODING
	{0}{0}Convert characters from encoding.
	-t ENCODING, --to-encoding=ENCODING
	{0}{0}Convert characters to encoding.
	-d, --directory
	{0}{0}Specify a directory, read input files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	encode -i a.txt -o b.txt -f ascii -t utf8
	In the above command the system would encode the file 'a.txt' 
	ascii encoding to the file 'b.txt' utf8 encoding.

	encode -d 'C:\Logs' -f ascii -t utf8
	In the above command the system would encode the all the files 
	in the 'C:\Logs' directory from ascii encoding to utf8 encoding.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static EncodeOptionType GetOptionType(string option)
		{
			EncodeOptionType optionType = EncodeOptionType.None;

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
