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

namespace Gimela.Knifer.CommandLines.Select
{
	internal static class SelectOptions
	{
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> ExtensionOptions;
		public static readonly ReadOnlyCollection<string> OutputOptions;
		public static readonly ReadOnlyCollection<string> CopyOptions;
		public static readonly ReadOnlyCollection<string> MoveOptions;
		public static readonly ReadOnlyCollection<string> KeepDepthOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<SelectOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static SelectOptions()
		{
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			ExtensionOptions = new ReadOnlyCollection<string>(new string[] { "e", "extension" });
			OutputOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			CopyOptions = new ReadOnlyCollection<string>(new string[] { "c", "copy" });
			MoveOptions = new ReadOnlyCollection<string>(new string[] { "m", "move" });
			KeepDepthOptions = new ReadOnlyCollection<string>(new string[] { "k", "keepdepth" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<SelectOptionType, ICollection<string>>();
			Options.Add(SelectOptionType.Directory, DirectoryOptions);
			Options.Add(SelectOptionType.Recursive, RecursiveOptions);
			Options.Add(SelectOptionType.Extension, ExtensionOptions);
			Options.Add(SelectOptionType.Output, OutputOptions);
			Options.Add(SelectOptionType.Copy, CopyOptions);
			Options.Add(SelectOptionType.Move, MoveOptions);
			Options.Add(SelectOptionType.KeepDepth, KeepDepthOptions);
			Options.Add(SelectOptionType.Help, HelpOptions);
			Options.Add(SelectOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(SelectOptions.RecursiveOptions);
			singleOptionList.AddRange(SelectOptions.CopyOptions);
			singleOptionList.AddRange(SelectOptions.MoveOptions);
			singleOptionList.AddRange(SelectOptions.HelpOptions);
			singleOptionList.AddRange(SelectOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	select - select files with specified file type

SYNOPSIS

	select [OPTION]...

DESCRIPTION

	Find files with specified file type and copy or move files. 

OPTIONS

	-d, --directory=DIR
	{0}{0}Specify a directory, read all files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-e, --extension=FILEEXTENSION
	{0}{0}Indicate the file extension type to be searched.
	{0}{0}Splits filter string to list with ',' or ';'.
	-o, --output=OUTPUT
	{0}{0}Specify a output directory, copy or move files into it.
	-c, --copy
	{0}{0}Copy to output folder when found files.
	-m, --move
	{0}{0}Move to output folder when found files.
	-k, --keepdepth=DEPTH
	{0}{0}Keep the file path depth in source directory to output folder.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	select -d 'E:\Books' -r -e '.pdf' -o 'E:\tmp' -c -k 3
	Find '.pdf' files in 'E:\Books' and copy to folder 'E:\tmp'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static SelectOptionType GetOptionType(string option)
		{
			SelectOptionType optionType = SelectOptionType.None;

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
