using DSAExperimentation.LeetCode.CountValidPathsInATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountValidPathsInATree;

// Harness only. Both strategies live in CountValidPathsInATreeSolution - including
// the per-pair path walk, which used to exist only as an unasserted benchmark
// baseline - and this file pins them to the same examples so a failure names the
// strategy that broke.
public sealed partial class CountValidPathsInATreeTests
{
    public static TheoryData<int, int[][], long> Examples =>
        new()
        {
            // LeetCode example 1. Primes among labels 1..5: 2, 3, 5. Valid pairs
            // (hand-verified against LeetCode's own explanation): (1,2), (1,3),
            // (1,4), (2,4).
            { 5, [[1, 2], [1, 3], [2, 4], [2, 5]], 4L },

            // LeetCode example 2. Primes among labels 1..6: 2, 3, 5. Valid pairs:
            // (1,2), (1,3), (1,4), (1,6), (2,4), (3,6).
            { 6, [[1, 2], [1, 3], [2, 4], [3, 5], [3, 6]], 6L },

            // A single node: no pair exists, so nothing can be valid.
            { 1, [], 0L },

            // The smallest valid path there is - label 1 is not prime, 2 is.
            { 2, [[1, 2]], 1L },

            // A star centred on prime 2 with a second prime, 3, hanging off it:
            // (1,2), (1,4) and (2,4) are valid; every pair spanning both primes is
            // not.
            { 4, [[1, 2], [2, 3], [2, 4]], 3L },

            // A path 1-4-2-3 whose two non-prime labels are adjacent, so they form
            // one blob of size 2 hanging off prime 2: (1,2) and (4,2) are valid.
            { 4, [[1, 4], [4, 2], [2, 3]], 2L },

            // A star centred on non-prime 1 with all six other labels as arms: the
            // centre plus arms 4 and 6 form one blob of size 3, so each of the four
            // primes 2, 3, 5, 7 pairs with all three of them - 4 * 3 = 12.
            { 7, [[1, 2], [1, 3], [1, 5], [1, 7], [1, 4], [1, 6]], 12L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountValidPathsByPerPairPathWalk_LeetCodeExamples_ReturnsExpectedCount(
        int nodeCount, int[][] edges, long expected)
    {
        var paths = CountValidPathsInATreeSolution.CountValidPathsByPerPairPathWalk(nodeCount, edges);
        Assert.Equal(expected, paths);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountValidPathsByDisjointSetBlobs_LeetCodeExamples_ReturnsExpectedCount(
        int nodeCount, int[][] edges, long expected)
    {
        var paths = CountValidPathsInATreeSolution.CountValidPathsByDisjointSetBlobs(nodeCount, edges);
        Assert.Equal(expected, paths);
    }
}
