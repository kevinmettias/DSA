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

                while (i + len < first.Length && j + len < second.Length && first[i + len] == second[j + len])
                {
                    len++;
                }

                best = Math.Max(best, len);
            }
        }

        return best;
    }

    public static int FindLengthByMemoizedSuffixPairDp(int[] first, int[] second)
    {
        var (_, best) = Memoizer.Memoize<(int First, int Second), (int MatchLen, int Best)>((0, 0), Explore);
        return best;

        (int MatchLen, int Best) Explore(
            (int First, int Second) state, Func<(int First, int Second), (int MatchLen, int Best)> explore)
        {
            var (i, j) = state;

            if (i == first.Length || j == second.Length)
            {
                return (0, 0);
            }

            var matchLen = 0;

            if (first[i] == second[j])
            {
                var (nextMatch, _) = explore((i + 1, j + 1));
                matchLen = 1 + nextMatch;
            }

            var (_, bestRight) = explore((i + 1, j));
            var (_, bestDown) = explore((i, j + 1));
            var bestOfRightAndDown = Math.Max(bestRight, bestDown);

            return (matchLen, Math.Max(matchLen, bestOfRightAndDown));
        }
    }
}
