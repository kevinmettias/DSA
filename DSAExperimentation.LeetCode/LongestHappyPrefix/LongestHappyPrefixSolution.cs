using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.LongestHappyPrefix;

// LeetCode 1392. Longest Happy Prefix: the longest non-empty proper prefix of s
// that is also a suffix of s - returned as the prefix itself, not its length.
//
// That is precisely what KMP's prefix/failure function measures, so the composed
// strategy has no separate algorithm at all: it runs
// PrefixFunctionSearch.ComputeFailureFunction over s and reads its last entry.
internal static class LongestHappyPrefixSolution
{
    // The textbook answer: try every candidate length from n-1 down to 1 and
    // compare that many characters each time - O(n^2) in the worst case.
    // Deliberately written with nothing but BCL spans; it is the arm the composed
    // strategy below has to justify itself against.
    public static string LongestPrefixByShrinkAndCompare(string s)
    {
        for (var length = s.Length - 1; length >= 1; length--)
        {
            if (s.AsSpan(0, length).SequenceEqual(s.AsSpan(s.Length - length, length)))
            {
                return s[..length];
            }
        }

        return string.Empty;
    }

    // This repo's own KMP prefix/failure function: failure[^1] is by definition the
    // length of the longest proper prefix of s that is also a suffix of s, so one
    // O(n) pass answers the problem outright and the only remaining work is the
    // slice.
    public static string LongestPrefixByPrefixFunction(string s)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(s);
        var length = failure.Length == 0 ? 0 : failure[^1];

        return s[..length];
    }
}
