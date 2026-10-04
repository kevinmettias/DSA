using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.LongestCommonPrefix;

// LeetCode 14. Longest Common Prefix: the longest string that is a prefix of
// every string in the input array.
//
// The two strategies differ in how they locate that length - shrinking a
// candidate prefix one character at a time versus binary-searching the
// monotone predicate "a prefix of length n is shared by every string".
internal static class LongestCommonPrefixSolution
{
    // The textbook baseline: take the first string as a candidate prefix and
    // shrink it whenever the next string does not start with it.
    public static string PrefixByLinearScan(string[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        var prefix = values[0];

        foreach (var value in values.Skip(1))
        {
            while (!value.StartsWith(prefix, StringComparison.Ordinal))
            {
                prefix = prefix[..^1];
            }
        }

        return prefix;
    }

    // "All strings share a prefix of length n" is monotone in n, so
    // MonotonePredicateSearch.LastTrue finds the longest shared length directly
    // instead of shrinking a candidate one character at a time. Length 0 is always
    // shared, so the search never comes back empty.
    public static string PrefixByBinarySearch(string[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        var shortest = values.Min(value => value.Length);
        var prefixLength = MonotonePredicateSearch.LastTrue(0, shortest, new PrefixSharedByAll(values));

        return values[0][..prefixLength];
    }

    // Holds(length) is "every string starts with the first string's prefix of this
    // length" - true up to the answer and false past it. Lengths only run up to the
    // shortest string's, so every span taken is in range.
    private readonly struct PrefixSharedByAll(string[] values) : IMonotonePredicate<int>
    {
        public bool Holds(int length)
        {
            var firstSpan = values[0].AsSpan(0, length);

            for (var i = 1; i < values.Length; i++)
            {
                var candidateSpan = values[i].AsSpan(0, length);

                if (!firstSpan.SequenceEqual(candidateSpan))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
