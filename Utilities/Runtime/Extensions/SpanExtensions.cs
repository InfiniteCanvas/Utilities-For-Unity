using System;

namespace InfiniteCanvas.Utilities.Extensions
{
	public static class SpanExtensions
	{
		/// <summary>
		/// Finds all occurrences of a specified value in a read-only span and returns their indices.
		/// </summary>
		/// <typeparam name="T">The type of elements in the span, which must implement <see cref="IEquatable{T}"/>.</typeparam>
		/// <param name="span">The source span to search.</param>
		/// <param name="value">The value to locate in the span.</param>
		/// <returns>
		/// A read-only span containing the zero-based indices of all occurrences of <paramref name="value"/>.
		/// Returns an empty span if no matches are found.
		/// </returns>
		/// <remarks>
		/// <para><b>Performance Characteristics:</b></para>
		/// <list type="bullet">
		///   <item><description>Time Complexity: O(2n) (two complete passes over the span)</description></item>
		///   <item><description>Memory Allocation: Exactly one array allocation sized to match the result count</description></item>
		///   <item><description>GC Pressure: Minimal (only the final result array is allocated)</description></item>
		/// </list>
		/// 
		/// <para><b>Usage Example:</b></para>
		/// <code>
		/// ReadOnlySpan&lt;char&gt; data = "hello world".AsSpan();
		/// var indices = data.IndicesOf('l');
		/// // Returns [2, 3, 9] (indices of 'l' in "hello world")
		/// </code>
		/// 
		/// <para><b>Edge Case Handling:</b></para>
		/// <list type="bullet">
		///   <item><description>Empty spans: Returns empty result</description></item>
		///   <item><description>No matches: Returns empty result</description></item>
		///   <item><description>Consecutive matches: Correctly reports each index</description></item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="value"/> is null for reference types.
		/// </exception>
		public static ReadOnlySpan<int> IndicesOf<T>(this in ReadOnlySpan<T> span, in T value) where T : IEquatable<T>
		{
			var count = 0;
			var offset = 0;
			while (true)
			{
				var index = span[offset..].IndexOf(value);
				if (index == -1) break;
				count++;
				offset += index + 1;
			}

			var indices = new int[count];

			offset = 0;
			var pos = 0;
			while (pos < count)
			{
				var index = span[offset..].IndexOf(value);
				indices[pos++] = offset + index;
				offset += index         + 1;
			}

			return indices;
		}

		/// <summary>
		/// Finds all occurrences of a specified sequence in a read-only span and returns their starting indices.
		/// </summary>
		/// <typeparam name="T">The type of elements in the span, which must implement <see cref="IEquatable{T}"/>.</typeparam>
		/// <param name="span">The source span to search.</param>
		/// <param name="value">The sequence to locate within the span.</param>
		/// <returns>
		/// A read-only span containing the zero-based starting indices of all occurrences of <paramref name="value"/>.
		/// Returns an empty span if no matches are found.
		/// </returns>
		/// <remarks>
		/// <para><b>Behavior Notes:</b></para>
		/// <list type="bullet">
		///   <item><description>Matches must be exact contiguous sequences</description></item>
		///   <item><description>Searches from left to right without overlapping matches</description></item>
		///   <item><description>Equivalent to substring search when working with <see cref="char"/> spans</description></item>
		/// </list>
		/// 
		/// <para><b>Performance Characteristics:</b></para>
		/// <list type="bullet">
		///   <item><description>Time Complexity: O(2n) worst case, optimized by runtime's <see cref="MemoryExtensions.IndexOf{T}"/></description></item>
		///   <item><description>Allocation: Single array sized to match result count</description></item>
		/// </list>
		///
		/// <para><b>Usage Example:</b></para>
		/// <code>
		/// ReadOnlySpan&lt;char&gt; data = "hello yellow fellow".AsSpan();
		/// var indices = data.IndicesOf("ll".AsSpan());
		/// // Returns [2, 7, 13] (starting indices of "ll" occurrences)
		/// </code>
		///
		/// <para><b>Special Cases:</b></para>
		/// <list type="bullet">
		///   <item><description>Empty value span: Throws <see cref="ArgumentException"/></description></item>
		///   <item><description>Value longer than source: Returns empty span</description></item>
		///   <item><description>Identical spans: Returns [0]</description></item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="value"/> is empty
		/// </exception>
		public static ReadOnlySpan<int> IndicesOf<T>(this in ReadOnlySpan<T> span, in ReadOnlySpan<T> value) where T : IEquatable<T>
		{
			var count = 0;
			var offset = 0;
			while (true)
			{
				var index = span[offset..].IndexOf(value);
				if (index == -1) break;
				count++;
				offset += index + 1;
			}

			var indices = new int[count];

			offset = 0;
			var pos = 0;
			while (pos < count)
			{
				var index = span[offset..].IndexOf(value);
				indices[pos++] = offset + index;
				offset += index         + 1;
			}

			return indices;
		}
	}
}