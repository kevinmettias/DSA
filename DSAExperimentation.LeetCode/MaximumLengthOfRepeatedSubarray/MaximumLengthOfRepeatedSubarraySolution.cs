using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumLengthOfRepeatedSubarray;

// LeetCode 718. Maximum Length of Repeated Subarray: the longest run that appears,
// contiguously, in both arrays.
//
// FindLengthByBruteForce is the textbook O(n*m*min(n,m)) approach: from every
// starting pair (i, j), walk forward while the two arrays keep matching. It is the
// baseline FindLengthByMemoizedSuffixPairDp has to justify itself against.
//
// FindLengthByMemoizedSuffixPairDp instead runs a single memoized walk over
// suffix-pair states (i, j) via this repo's Memoizer - the same two-sequence-DP shape
// EditDistanceTests/DistinctSubsequencesTests already use. Each state returns both
// "the exact match run starting here" and "the best run found anywhere from here
// onward," so a distinct (i, j) state's match-run length is computed once and every
// later reference to it (from an earlier diagonal, or a neighboring row/column)
// reuses the cached result instead of rescanning it - O(n*m) total. The max-over-
// all-pairs answer falls out of Best at the start state.
internal static class MaximumLengthOfRepeatedSubarraySolution
{
    public static int FindLengthByBruteForce(int[] first, int[] second)
    {
        var best = 0;

        for (var i = 0; i < first.Length; i++)
        {
            for (var j = 0; j < second.Length; j++)
            {
                var len = 0;

                while (CanExtendRun(first, second, (i, j), len))
                {
                    len++;
                }

                best = Math.Max(best, len);
            }
        }

        return best;
    }

    // Both walks still have an element at this offset, and those elements match -
    // so the run starting at (i, j) can grow by one more.
    private static bool CanExtendRun(int[] first, int[] second, (int First, int Second) start, int len)
        => start.First + len < first.Length && start.Second + len < second.Length
            && first[start.First + len] == second[start.Second + len];

    public static int FindLengthByMemoizedSuffixPairDp(int[] first, int[] second)
    {
        var (_, best) = Memoizer.Memoize<(int First, int Second), (int MatchLen, int Best)>(
            (0, 0), new SuffixPairRuns(first, second));

        return best;
    }

    // The suffix-pair rule, named: what the match run starting at (i, j) is worth, and the
    // best run found anywhere from (i, j) onward. Both arrays are fixed for the whole walk
    // and arrive once through the primary constructor; `rest` is the memo run's own handle
    // on this rule, so each recurrence below is a call on a named type.
    private sealed class SuffixPairRuns(int[] first, int[] second)
        : IRecurrence<(int First, int Second), (int MatchLen, int Best)>
    {
        /// <inheritdoc/>
        public (int MatchLen, int Best) Replay(
            (int First, int Second) state,
            IRecurrence<(int First, int Second), (int MatchLen, int Best)> rest)
        {
            var (i, j) = state;

            if (i == first.Length || j == second.Length)
            {
                return (0, 0);
            }

            var matchLen = 0;

            if (first[i] == second[j])
            {
                var (nextMatch, _) = rest.Replay((i + 1, j + 1), rest);
                matchLen = 1 + nextMatch;
            }

            var (_, bestRight) = rest.Replay((i + 1, j), rest);
            var (_, bestDown) = rest.Replay((i, j + 1), rest);
            var bestOfRightAndDown = Math.Max(bestRight, bestDown);

            return (matchLen, Math.Max(matchLen, bestOfRightAndDown));
        }
    }
}
