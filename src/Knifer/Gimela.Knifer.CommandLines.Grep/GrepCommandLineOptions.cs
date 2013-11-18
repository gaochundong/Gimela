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

namespace Gimela.Knifer.CommandLines.Grep
{
  internal class GrepCommandLineOptions
  {
    public GrepCommandLineOptions()
    {
      this.FilePaths = new List<string>();
    }

    public bool IsSetRegexPattern { get; set; }
    public string RegexPattern { get; set; }

    public bool IsSetPath { get; set; }
    public ICollection<string> FilePaths { get; set; }

    public bool IsSetFixedStrings { get; set; }
    public bool IsSetIgnoreCase { get; set; }
    public bool IsSetInvertMatch { get; set; }

    public bool IsSetOutputFile { get; set; }
    public string OutputFile { get; set; }

    public bool IsSetCount { get; set; }
    public bool IsSetFilesWithoutMatch { get; set; }
    public bool IsSetFilesWithMatchs { get; set; }
    public bool IsSetNoMessages { get; set; }
    public bool IsSetWithFileName { get; set; }
    public bool IsSetNoFileName { get; set; }
    public bool IsSetLineNumber { get; set; }
    public bool IsSetDirectory { get; set; }
    public bool IsSetRecursive { get; set; }

    public bool IsSetExcludeFiles { get; set; }
    public string ExcludeFilesPattern { get; set; }

    public bool IsSetExcludeDirectories { get; set; }
    public string ExcludeDirectoriesPattern { get; set; }

    public bool IsSetIncludeFiles { get; set; }
    public string IncludeFilesPattern { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
