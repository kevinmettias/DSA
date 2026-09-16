using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

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
    // BinarySearch.LowerBound finds the first failing length directly instead
    // of shrinking a candidate one character at a time.
    public static string PrefixByBinarySearch(string[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        var shortest = values.Min(value => value.Length);
        var sequence = new PrefixFeasibilitySequence(values, shortest);
        var firstFailingLength = BinarySearch.LowerBound<int, PrefixFeasibilitySequence>(sequence, 1);
        var prefixLength = firstFailingLength - 1;

        return values[0][..prefixLength];
    }

    private readonly struct PrefixFeasibilitySequence(string[] values, int maxLength) : IRandomAccessSequence<int>
    {
        public int Length => maxLength + 1;

        public int Get(int length) => IsPrefixSharedByAll(length) ? 0 : 1;

        private bool IsPrefixSharedByAll(int length)
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
