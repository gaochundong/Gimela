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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gimela.Knifer.CommandLines.Foundation.UnitTest
{
  /// <summary>
  /// This is a test class for CommandLineParserTest and is intended
  /// to contain all CommandLineParserTest Unit Tests
  ///</summary>
  [TestClass()]
  public class CommandLineParserTest
  {
    #region Test Context

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #endregion

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    public void IsArgument01Test()
    {
      string token = "-a";
      bool actual = CommandLineParser_Accessor.IsArgument(token);
      Assert.IsTrue(actual);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    public void IsArgument02Test()
    {
      string token = "--a";
      bool actual = CommandLineParser_Accessor.IsArgument(token);
      Assert.IsTrue(actual);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    public void IsArgument03Test()
    {
      string token = @"---a";
      bool actual = CommandLineParser_Accessor.IsArgument(token);
      Assert.IsTrue(actual);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    public void NextToken01Test()
    {
      string[] args = new string[] { "a" };
      int index = 0;
      int indexExpected = 0;
      string expected = args[0];
      string actual = CommandLineParser_Accessor.NextToken(args, ref index);
      Assert.AreEqual(expected, actual);
      Assert.AreEqual(indexExpected, index);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    public void NextToken02Test()
    {
      string[] args = new string[] { "a", "b" };
      int index = 1;
      int indexExpected = 1;
      string expected = args[1];
      string actual = CommandLineParser_Accessor.NextToken(args, ref index);
      Assert.AreEqual(expected, actual);
      Assert.AreEqual(indexExpected, index);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    public void NextToken03Test()
    {
      string[] args = new string[] { "a", "", "c" };
      int index = 1;
      int indexExpected = 2;
      string expected = args[2];
      string actual = CommandLineParser_Accessor.NextToken(args, ref index);
      Assert.AreEqual(expected, actual);
      Assert.AreEqual(indexExpected, index);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "A out of range index was inappropriately allowed.")]
    public void NextToken04Test()
    {
      string[] args = new string[] { "a" };
      int index = -1;
      CommandLineParser_Accessor.NextToken(args, ref index);
    }

    [TestMethod()]
    [DeploymentItem("Gimela.Knifer.CommandLines.Foundation.dll")]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "A out of range index was inappropriately allowed.")]
    public void NextToken05Test()
    {
      string[] args = new string[] { "a" };
      int index = 99999999;
      CommandLineParser_Accessor.NextToken(args, ref index);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException), "A null argument was inappropriately allowed.")]
    public void Parse00Test()
    {
      string[] args = null;
      CommandLineOptions actual = CommandLineParser.Parse(args);
    }

    [TestMethod()]
    public void Parse01Test()
    {
      string[] args = new string[] { 
        "a", 
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Parameters.Count);
    }

    [TestMethod()]
    public void Parse02Test()
    {
      string[] args = new string[] { 
        "a", 
        "b"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(2, actual.Parameters.Count);
    }

    [TestMethod()]
    public void Parse03Test()
    {
      string[] args = new string[] { 
        "-a",
        "value"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse04Test()
    {
      string[] args = new string[] { 
        "-a=",
        "value"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse05Test()
    {
      string[] args = new string[] { 
        "-a",
        "=",
        "value"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse06Test()
    {
      string[] args = new string[] { 
        "-a",
        "=value"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse07Test()
    {
      string[] args = new string[] { 
        "-a=value"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse08Test()
    {
      string[] args = new string[] { 
        "-a==value"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse09Test()
    {
      string[] args = new string[] { 
        "-a",
        "-b"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(2, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse10Test()
    {
      string[] args = new string[] { 
        "a",
        "b",
        "-c"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(2, actual.Parameters.Count);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse11Test()
    {
      string[] args = new string[] { 
        "a",
        "-b",
        "-c"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Parameters.Count);
      Assert.AreEqual(2, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse12Test()
    {
      string[] args = new string[] { 
        "-a",
        "b",
        "-c"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(2, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse13Test()
    {
      string[] args = new string[] { 
        "-a",
        "-b",
        "-c"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(3, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse14Test()
    {
      string[] args = new string[] { 
        "-a",
        "b",
        "-c",
        "d"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(2, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse15Test()
    {
      string[] args = new string[] { 
        "-a",
        "b",
        "-c",
        "d",
        "e"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Parameters.Count);
      Assert.AreEqual(2, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse16Test()
    {
      string[] args = new string[] { 
        "-a",
        "b",
        "-c",
        "d",
        "e",
        "f"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(2, actual.Parameters.Count);
      Assert.AreEqual(2, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse17Test()
    {
      string[] args = new string[] { 
        "-a",
        "b",
        "c",
        "-d",
        "-e",
        "f"
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Parameters.Count);
      Assert.AreEqual(3, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse18Test()
    {
      string[] args = new string[] { 
        "a",
        "-b",
        "bvalue",
        "--c",
        "cvalue",
        "d",
        @"e=evalue",
        "f",
        "g",
        "--h",
        "=",
        "hvalue",
        "i",
        "j",
        "-k=",
        "kvalue",
        "-l",
        "=lvalue",
        "-m",
        "-n",
        @"--o=-ovalue",
        @"--p===pvalue",
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(7, actual.Parameters.Count);
      Assert.AreEqual(9, actual.Arguments.Count);
    }

    [TestMethod()]
    public void Parse19Test()
    {
      string[] args = new string[] { 
        "-f",
        @"\\192.168.0.1\c$\Program Files\a.log",
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
      Assert.AreEqual(1, actual.Arguments.Count);
    }

    [TestMethod()]
    [ExpectedException(typeof(CommandLineException), "A duplicate argument was inappropriately allowed.")]
    public void Parse20Test()
    {
      string[] args = new string[] { 
        "-k",
        "-k",
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
    }

    [TestMethod()]
    [ExpectedException(typeof(CommandLineException), "A duplicate argument was inappropriately allowed.")]
    public void Parse21Test()
    {
      string[] args = new string[] { 
        "a",
        "a",
      };

      CommandLineOptions actual = CommandLineParser.Parse(args);
    }
  }
}
