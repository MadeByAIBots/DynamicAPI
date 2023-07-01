using System;
using System.Collections.Generic;

namespace DynamicApi.Utilities.Files
{
	public static class TextUtils
	{
		public static string FormatTextWithNumbersAndHashes(string inputText)
		{
			inputText = inputText.TrimEnd('\n');
			var lines = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			var formattedLines = new List<string>();

			for (int i = 0; i < lines.Length; i++)
			{
				var hash = HashUtils.GenerateSimpleHash(lines[i]);
				formattedLines.Add(FormatLine(hash, i + 1, lines[i]));
			}

			return string.Join("\n", formattedLines);
		}

		private static string FormatLine(string hash, int lineNumber, string lineContent)
		{
			return $"[{hash}] {lineNumber}: {lineContent}";
		}
	}
}