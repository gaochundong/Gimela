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

namespace Gimela.Knifer.CommandLines.Base64
{
	internal static class Base64Options
	{
		public static readonly ReadOnlyCollection<string> DecodeOptions;
		public static readonly ReadOnlyCollection<string> EncodingOptions;
		public static readonly ReadOnlyCollection<string> TextOptions;
		public static readonly ReadOnlyCollection<string> FileOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<Base64OptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static Base64Options()
		{
			DecodeOptions = new ReadOnlyCollection<string>(new string[] { "d", "decode" });
			EncodingOptions = new ReadOnlyCollection<string>(new string[] { "e", "encoding" });
			TextOptions = new ReadOnlyCollection<string>(new string[] { "t", "text" });
			FileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<Base64OptionType, ICollection<string>>();
			Options.Add(Base64OptionType.Decode, DecodeOptions);
			Options.Add(Base64OptionType.Encoding, EncodingOptions);
			Options.Add(Base64OptionType.Text, TextOptions);
			Options.Add(Base64OptionType.File, FileOptions);
			Options.Add(Base64OptionType.Help, HelpOptions);
			Options.Add(Base64OptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(Base64Options.DecodeOptions);
			singleOptionList.AddRange(Base64Options.HelpOptions);
			singleOptionList.AddRange(Base64Options.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	base64 - base64 encode or decode data

SYNOPSIS

	base64 [OPTION]... [FILE]

DESCRIPTION

	Base64 encode or decode FILE, or standard input, to standard output.

OPTIONS

	-d, --decode
	{0}{0}Decode data.
	-e, --encoding
	{0}{0}Text encoding, support ASCII, UTF7, UTF8, UTF32, Unicode, 
	{0}{0}and BigEndianUnicode, and default UTF8.
	-t, --text
	{0}{0}Specify text.
	-f, --file
	{0}{0}Specify a file to encode.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	base64 -e utf8 '123456789'
	In the above command the system would base64 encode
	the text '123456789'.

	base64 -e utf8 -d 'MTIzNDU2Nzg5'
	In the above command the system would base64 decode
	the text 'MTIzNDU2Nzg5'.

	base64 -e utf8 -f 'C:\a.txt'
	In the above command the system would base64 encode
	the file 'C:\a.txt'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static Base64OptionType GetOptionType(string option)
		{
			Base64OptionType optionType = Base64OptionType.None;

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
