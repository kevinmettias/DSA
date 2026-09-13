using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.LongestCommonSubsequence;

// LeetCode 1143. Longest Common Subsequence: the length of the longest sequence of
// characters appearing, in order but not necessarily contiguously, in both strings.
//
// Both strategies run the same suffix-pair recurrence - match a shared leading
// character and advance both cursors, otherwise keep the better of skipping a
// character from either string - and differ only in the direction it is walked:
// LengthByTabulation fills a bottom-up 2D table with no recursion or caching at all,
// while LengthByMemoizedSuffixPairDp recurses top-down over (i, j) states through
// this repo's Memoizer, the same two-string-DP shape EditDistanceSolution and
// MaximumLengthOfRepeatedSubarraySolution already use.
internal static class LongestCommonSubsequenceSolution
{
    // The textbook baseline: a plain int[,] table filled from the far corner back to
    // (0, 0), every cell touched exactly once. Deliberately written without this
    // repo's primitives - it is the arm the memoized strategy has to justify itself
    // against.
    public static int LengthByTabulation(string text1, string text2)
    {
        var lcs = new int[text1.Length + 1, text2.Length + 1];

        for (var i = text1.Length - 1; i >= 0; i--)
        {
            for (var j = text2.Length - 1; j >= 0; j--)
            {
                lcs[i, j] = text1[i] == text2[j]
                    ? 1 + lcs[i + 1, j + 1]
                    : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
            }
        }

        return lcs[0, 0];
    }

    // The same recurrence run top-down, with this repo's own Memoizer<TState,TResult>
    // caching each (i, j) suffix pair - so only the states actually reachable from
    // (0, 0) are ever evaluated, rather than the whole table.
    public static int LengthByMemoizedSuffixPairDp(string text1, string text2)
    {
        return Memoizer.Memoize<(int First, int Second), int>((0, 0), LcsFrom);

        int LcsFrom((int First, int Second) state, Func<(int First, int Second), int> lcs)
        {
            var (i, j) = state;

            if (i == text1.Length || j == text2.Length)
            {
                return 0;
            }

            if (text1[i] == text2[j])
            {
                return 1 + lcs((i + 1, j + 1));
            }

            return Math.Max(lcs((i + 1, j)), lcs((i, j + 1)));
        }
    }
}
