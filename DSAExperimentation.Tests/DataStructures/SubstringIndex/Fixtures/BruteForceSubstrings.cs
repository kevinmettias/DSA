namespace DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;

// The reference every substring index is checked against: each answer recomputed by scanning the
// text at every position, in quadratic time or worse, with nothing shared with any index.
internal static class BruteForceSubstrings
{
    /// <summary>Lists every position where a pattern starts in a text.</summary>
    /// <param name="text">The text to scan.</param>
    /// <param name="pattern">The pattern to look for.</param>
    /// <returns>
    /// The positions in ascending order; every position from 0 to the text's length for an empty
    /// pattern.
    /// </returns>
    public static IReadOnlyList<int> Starts(string text, string pattern)
    {
        var starts = new List<int>();

        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            if (text.AsSpan(start, pattern.Length).SequenceEqual(pattern))
            {
                starts.Add(start);
            }
        }

        return starts;
    }

    /// <summary>
    /// Measures the longest substring that starts at two different positions of a text.
    /// </summary>
    /// <param name="text">The text to scan.</param>
    /// <returns>Its length, 0 when no character repeats.</returns>
    public static int LongestRepeatLength(string text)
    {
        var longest = 0;

        for (var first = 0; first < text.Length; first++)
        {
            for (var second = first + 1; second < text.Length; second++)
            {
                longest = Math.Max(longest, LongestCommonPrefix(text, first, second));
            }
        }

        return longest;
    }

    /// <summary>Measures how long a prefix two suffixes of a text share.</summary>
    /// <param name="text">The text.</param>
    /// <param name="first">Where one suffix starts.</param>
    /// <param name="second">Where the other starts.</param>
    /// <returns>The length of their longest common prefix.</returns>
    public static int LongestCommonPrefix(string text, int first, int second)
    {
        var length = 0;

        while (HasSameLetterAt(text, first + length, second + length))
        {
            length++;
        }

        return length;
    }

    // Both positions are still inside the text, and hold the same letter.
    private static bool HasSameLetterAt(string text, int first, int second)
        => first < text.Length && second < text.Length && text[first] == text[second];
}
