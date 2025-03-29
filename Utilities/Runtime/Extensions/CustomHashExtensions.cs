using System;

namespace InfiniteCanvas.Utilities.Extensions
{
	public static class CustomHashExtensions
	{
		public static int GetCustomHashCode(this ReadOnlySpan<char> span)
		{
			unchecked
			{
				var hash1 = 5381;
				var hash2 = hash1;

				for (var i = 0; i < span.Length && span[i] != '\0'; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ span[i];
					if (i + 1 == span.Length)
						break;

					hash2 = ((hash2 << 5) + hash2) ^ span[i + 1];
				}

				return hash1 + hash2 * 1566083941;
			}
		}

		// making sure strings and their spans have the same hash
		public static int GetCustomHashCode(this string s) => s.AsSpan().GetCustomHashCode();
	}
}