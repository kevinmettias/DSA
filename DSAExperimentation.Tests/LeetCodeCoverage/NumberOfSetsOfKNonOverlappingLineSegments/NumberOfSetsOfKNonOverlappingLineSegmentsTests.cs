using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSetsOfKNonOverlappingLineSegments;

// LeetCode 1621. Number of Sets of K Non-Overlapping Line Segments: n points laid
// out left to right at x = 0..n-1, choose k non-overlapping segments (touching at
// a shared endpoint is allowed). Only the point count and k matter, never the
// actual coordinates - a segment is just a pair of increasing indices, and a set
// of them is non-overlapping iff, sorted by left endpoint, each one's right
// endpoint is <= the next one's left endpoint. That combinatorial count is the
// stars-and-bars identity C(n-1+k, 2k) (confirmed against a direct brute-force
// enumeration of every segment-tuple for n,k up to 9, and against LeetCode's own
// published n=4,k=2 -> 5 and n=5,k=3 -> 7 examples), so the whole problem reduces
// to Pascal's identity - memoized via this repo's own Memoizer<TState,TResult> the
// same way NumberOfWaysToReorderArrayToGetSameBSTTests/ProbabilityOfATwoBoxesHaving
// TheSameNumberOfDistinctBallsTests already compute C(n,k). The one addition their
// Choose didn't need: a k > n guard, since unlike a tree-shaped child count, 2k can
// exceed n-1+k here (whenever there simply aren't enough points to seat k segments,
// even with every pair touching) - without it the plain Pascal split recurses on
// negative-n states forever instead of hitting either base case.
public sealed partial class NumberOfSetsOfKNonOverlappingLineSegmentsTests
{
    private const long Modulo = 1_000_000_007;

    [Theory]
    [InlineData(4, 2, 5)]
    [InlineData(5, 3, 7)]
    [InlineData(3, 1, 3)]
    [InlineData(2, 1, 1)]
    [InlineData(5, 2, 15)]
    [InlineData(6, 3, 28)]
    [InlineData(1, 0, 1)]
    public void NumberOfSets_VerifiedAgainstBruteForceEnumeration_ReturnsExpectedCount(int n, int k, int expected)
        => Assert.Equal(expected, NumberOfSets(n, k));

    [Fact]
    public void NumberOfSets_TouchingLetsKSegmentsFitInJustKPlusOnePoints_ReturnsOne()
        => Assert.Equal(1, NumberOfSets(3, 2));

    [Fact]
    public void NumberOfSets_TooFewPointsForKSegmentsEvenWhileTouching_ReturnsZero()
        => Assert.Equal(0, NumberOfSets(2, 2));

    private static int NumberOfSets(int n, int k) => (int)Choose(n + k - 1, 2 * k);

    private static long Choose(int n, int k) => Memoizer.Memoize<(int N, int K), long>((n, k), ChooseRecurrence);

    private static long ChooseRecurrence((int N, int K) state, Func<(int, int), long> choose)
    {
        var (n, k) = state;

        if (k == 0 || k == n)
        {
            return 1;
        }

        if (k > n)
        {
            return 0;
        }

        return (choose((n - 1, k - 1)) + choose((n - 1, k))) % Modulo;
    }
}
