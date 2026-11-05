using DSAExperimentation.LeetCode.Solutions.CountValidPathsInATree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountValidPathsInATree;

// Harness only. Both strategies live in CountValidPathsInATreeSolution - including
// the per-pair path walk, which used to exist only as an unasserted benchmark
// baseline - and this file pins them to the same cases so a failure names the
// strategy that broke.
public sealed partial class CountValidPathsInATreeSolutionTests
{
    public static TheoryData<int, int[][], long> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void CountValidPathsByPerPairPathWalk_Cases_ReturnsExpectedCount(
        int nodeCount, int[][] edges, long expected)
    {
        var paths = CountValidPathsInATreeSolution.CountValidPathsByPerPairPathWalk(nodeCount, edges);
        Assert.Equal(expected, paths);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountValidPathsByDisjointSetBlobs_Cases_ReturnsExpectedCount(
        int nodeCount, int[][] edges, long expected)
    {
        var paths = CountValidPathsInATreeSolution.CountValidPathsByDisjointSetBlobs(nodeCount, edges);
        Assert.Equal(expected, paths);
    }
}
