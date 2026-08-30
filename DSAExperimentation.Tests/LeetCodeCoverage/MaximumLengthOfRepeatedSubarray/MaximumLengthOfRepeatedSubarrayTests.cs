using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumLengthOfRepeatedSubarray;

// LeetCode 718. Maximum Length of Repeated Subarray: this repo's Memoizer over
// suffix-pair states (i, j), the same two-sequence-DP shape EditDistance/
// DistinctSubsequences already use. Each state returns both "the exact match run
// starting here" and "the best run found anywhere from here onward," so a single
// memoized walk from (0, 0) covers every starting pair without hand-deriving the
// whole table - the max-over-all-pairs answer falls out of Best at the start state.
public sealed partial class MaximumLengthOfRepeatedSubarrayTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 2, 1 }, new[] { 3, 2, 1, 4, 7 }, 3)]
    [InlineData(new[] { 0, 0, 0, 0, 0 }, new[] { 0, 0, 0, 0, 0 }, 5)]
    [InlineData(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, 0)]
    public void FindLength_LeetCodeExamples_ReturnsLongestRepeatedRun(int[] first, int[] second, int expected)
        => Assert.Equal(expected, FindLength(first, second));

    private static int FindLength(int[] first, int[] second)
    {
        var (_, best) = Memoizer.Memoize<(int First, int Second), (int MatchLen, int Best)>((0, 0), Explore);
        return best;

        (int MatchLen, int Best) Explore((int First, int Second) state, Func<(int First, int Second), (int MatchLen, int Best)> explore)
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

            return (matchLen, Math.Max(matchLen, Math.Max(bestRight, bestDown)));
        }
    }
}
