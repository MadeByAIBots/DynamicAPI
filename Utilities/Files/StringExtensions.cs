namespace DynamicApi.Utilities.Files
{
	public static class StringExtensions
	{
		public static string ToNumbered(this string[] inputLines)
		{
			return string.Join(Environment.NewLine, inputLines).ToNumbered();
		}

		public static string ToNumbered(this string inputText)
		{
			return TextUtils.FormatTextWithNumbersAndHashes(inputText);
		}
	}
}