namespace DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;

// The patterns a substring index is asked about for one text: every short substring it holds, each
// one with its last letter changed (absent, or another substring), the whole text, and patterns
// that run past the text's end, which a search must not count as matches.
internal static class SubstringPatterns
{
    // The longest pattern drawn from a text, beyond which every substring is checked only as the
    // whole text.
    private const int LongestDrawnPattern = 8;

    // A character no fixture text holds, so a pattern ending in it is absent.
    private const char AbsentLetter = '#';

    /// <summary>Lists the patterns a text's index is asked about.</summary>
    /// <param name="text">The indexed text.</param>
    /// <returns>
    /// The empty pattern, each distinct substring up to eight characters, each of those with its
    /// last character changed (often absent), the whole text, and patterns running past its end.
    /// </returns>
    public static IReadOnlyList<string> PatternsFor(string text)
    {
        var patterns = new HashSet<string>(StringComparer.Ordinal)
        {
            string.Empty,
            text,
            text + AbsentLetter,
        };

        for (var start = 0; start < text.Length; start++)
        {
            var longest = Math.Min(LongestDrawnPattern, text.Length - start);

            for (var length = 1; length <= longest; length++)
            {
                var substring = text.Substring(start, length);
                patterns.Add(substring);
                patterns.Add(substring[..^1] + AbsentLetter);
                patterns.Add(substring[..^1] + NextLetter(substring[^1]));
            }
        }

        return [.. patterns.Order(StringComparer.Ordinal)];
    }

    private static char NextLetter(char letter) => (char)(letter + 1);
}
