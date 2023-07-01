using System;
using System.Security.Cryptography;
using System.Text;

namespace DynamicApi.Utilities.Files
{
	public static class HashUtils
	{

		public static string GenerateSimpleHash(string line)
		{
			var trimmedLine = line.Trim();
			var hash = GetSha1Hash(trimmedLine);
			var shortHash = hash.Substring(0, Math.Min(3, hash.Length));
			return shortHash;
		}

		public static string GetSha1Hash(string input)
		{
			using (var sha1 = SHA1.Create())
			{
				var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
				// Convert the hash bytes to a hexadecimal string
				return BitConverter.ToString(hash).Replace("-", "").Substring(0, 3);
			}
		}
	}
}