// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.ILSpy
{
	static class CommandLineHelpers
	{
		#region CommandLine <-> Argument Array
		/// <summary>
		/// Decodes a command line into an array of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static unsafe string[] CommandLineToArgumentArray(string commandLine)
		{
			bool inQuotes = false;

			return Split(commandLine, c =>
				{
					if (c == '\"')
					{
						inQuotes = !inQuotes;
					}
					return !inQuotes && c == ' ';
				}).Select(arg => TrimMatchingQuotes(arg, '\"'))
				.Where(arg => !string.IsNullOrEmpty(arg))
				.ToArray();
		}
		
		static string TrimMatchingQuotes(string input, char quote)
		{
			input = input.Trim();
			if (input.Length >= 2 && input[0] == quote && input[input.Length-1] == quote)
			{
				return input.Substring(1, input.Length - 2);
			}
			return input;
		}
		
		static IEnumerable<string> Split(string str, Func<char, bool> controller)
		{
			int nextPiece = 0;
			for (int c = 0; c < str.Length; c++)
			{
				if (controller(str[c]))
				{
					yield return str.Substring(nextPiece, c - nextPiece);
					nextPiece = c + 1;
				}
			}

			yield return str.Substring(nextPiece);
		}

		static readonly char[] charsNeedingQuoting = { ' ', '\t', '\n', '\v', '"' };

		/// <summary>
		/// Escapes a set of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static string ArgumentArrayToCommandLine(params string[] arguments)
		{
			if (arguments == null)
				return null;
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < arguments.Length; i++)
			{
				if (i > 0)
					b.Append(' ');
				AppendArgument(b, arguments[i]);
			}
			return b.ToString();
		}

		static void AppendArgument(StringBuilder b, string arg)
		{
			if (arg == null)
			{
				return;
			}

			if (arg.Length > 0 && arg.IndexOfAny(charsNeedingQuoting) < 0)
			{
				b.Append(arg);
			}
			else
			{
				b.Append('"');
				for (int j = 0; ; j++)
				{
					int backslashCount = 0;
					while (j < arg.Length && arg[j] == '\\')
					{
						backslashCount++;
						j++;
					}
					if (j == arg.Length)
					{
						b.Append('\\', backslashCount * 2);
						break;
					}
					else if (arg[j] == '"')
					{
						b.Append('\\', backslashCount * 2 + 1);
						b.Append('"');
					}
					else
					{
						b.Append('\\', backslashCount);
						b.Append(arg[j]);
					}
				}
				b.Append('"');
			}
		}
		#endregion
	}
}
