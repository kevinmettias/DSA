using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.LeetCode.LastSubstringInLexicographicalOrder;

// LeetCode 1163. Last Substring in Lexicographical Order: return the
// lexicographically largest substring of s.
//
// Both strategies rest on the same observation: the largest substring is always a
// SUFFIX. Any substring that stops early is a proper prefix of the suffix starting at
// the same index, and a proper prefix always compares smaller, so extending it to the
// end of the string can only improve the answer. The problem is therefore "find the
// maximal suffix", and the two strategies differ only in how they find it.
internal static class LastSubstringInLexicographicalOrderSolution
{
    // Baseline: hold the best suffix seen so far and compare each remaining suffix
    // against it with an ordinal span comparison - O(n) comparisons, each up to O(n)
    // characters. Deliberately plain BCL; this is what you would write without this
    // repo.
    public static string LastSubstringByPairwiseComparison(string s)
    {
        var bestStart = 0;

        for (var candidate = 1; candidate < s.Length; candidate++)
        {
            var candidateSpan = s.AsSpan(candidate);
            var bestSpan = s.AsSpan(bestStart);

            if (candidateSpan.CompareTo(bestSpan, StringComparison.Ordinal) > 0)
            {
                bestStart = candidate;
            }
        }

        return s[bestStart..];
    }

    // Composed: this repo's own SuffixArray already sorts every suffix ascending in
    // O(n log^2 n), so the maximal suffix is simply its last entry and the whole
    // problem reduces to reading that one index back out - no separate maximal-suffix
    // scan of any kind.
    public static string LastSubstringBySuffixArray(string s)
    {
        var suffixArray = new SuffixArrayStructure(s);
        var maxSuffixStart = suffixArray.Suffixes[^1];

        return s[maxSuffixStart..];
    }
}
