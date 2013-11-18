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

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Gimela.Knifer.CommandLines.Foundation
{
  public static class WildcardCharacterHelper
  {
    public static bool IsContainsWildcard(string path)
    {
      bool result = false;

      if (!string.IsNullOrEmpty(path))
      {
        if (path.Contains(@"*"))
        {
          result = true;
        }
        else if (path.Contains(@"?"))
        {
          result = true;
        }
      }

      return result;
    }

    public static string TranslateWildcardToRegex(string pattern)
    {
      return Regex.Escape(pattern).Replace(@"\*", @".*").Replace(@"\?", @".");
    }

    public static string TranslateWildcardFilePath(string file)
    {
      string path = string.Empty;

      if (!string.IsNullOrEmpty(file))
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        path = file.Replace(@"/", @"\\");
        if (path.StartsWith(@"." + Path.DirectorySeparatorChar, StringComparison.CurrentCulture))
        {
          path = (currentDirectory.FullName
            + Path.DirectorySeparatorChar
            + path.TrimStart('.', Path.DirectorySeparatorChar)).Replace(@"\\", @"\");
        }
      }

      return path;
    }

    public static string TranslateWildcardDirectoryPath(string directory)
    {
      string path = string.Empty;

      if (!string.IsNullOrEmpty(directory))
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        path = directory.Replace(@"/", @"\\");
        if (path == @".")
        {
          path = currentDirectory.FullName;
        }
        else if (path.StartsWith(@"." + Path.DirectorySeparatorChar, StringComparison.CurrentCulture))
        {
          path = (currentDirectory.FullName
            + Path.DirectorySeparatorChar
            + path.TrimStart('.', Path.DirectorySeparatorChar)).Replace(@"\\", @"\");
        }
      }

      return path;
    }
  }
}
