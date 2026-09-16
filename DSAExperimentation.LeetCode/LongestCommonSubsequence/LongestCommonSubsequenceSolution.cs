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
                var charactersMatch = text1[i] == text2[j];
                lcs[i, j] = charactersMatch
                    ? MatchedLcs(lcs, i, j)
                    : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
            }
        }

        return lcs[0, 0];
    }

    // The characters match, so both cursors advance and the shared subsequence is the
    // next suffix pair's, one character longer.
    private static int MatchedLcs(int[,] lcs, int firstIndex, int secondIndex) =>
        1 + lcs[firstIndex + 1, secondIndex + 1];

    // The same recurrence run top-down, with this repo's own Memoizer<TState,TResult>
    // caching each (i, j) suffix pair - so only the states actually reachable from
    // (0, 0) are ever evaluated, rather than the whole table.
    public static int LengthByMemoizedSuffixPairDp(string text1, string text2)
    {
        var recurrence = new SharedSubsequenceLength(text1, text2);

        return Memoizer.Memoize<(int First, int Second), int>((0, 0), recurrence);
    }

    // The recurrence, named: matching leading characters advance both cursors, and
    // otherwise the better of dropping a character from either string wins.
    private sealed class SharedSubsequenceLength(string text1, string text2)
        : IRecurrence<(int First, int Second), int>
    {
        /// <inheritdoc/>
        public int Replay((int First, int Second) state, IRecurrence<(int First, int Second), int> rest)
        {
            var (i, j) = state;

            if (i == text1.Length || j == text2.Length)
            {
                return 0;
            }

            if (text1[i] == text2[j])
            {
                return 1 + rest.Replay((i + 1, j + 1), rest);
            }

            var skipFromFirst = rest.Replay((i + 1, j), rest);
            var skipFromSecond = rest.Replay((i, j + 1), rest);

            return Math.Max(skipFromFirst, skipFromSecond);
        }
    }
}
