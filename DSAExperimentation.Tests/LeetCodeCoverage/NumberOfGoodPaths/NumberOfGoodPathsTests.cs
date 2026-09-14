using DSAExperimentation.LeetCode.NumberOfGoodPaths;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfGoodPaths;

// Harness only. Both strategies are NumberOfGoodPathsSolution's - this file pins
// them to LeetCode's published examples plus the shapes those never reach: a lone
// node, a chain where a taller node blocks the only path between two equal ones,
// and a chain where a shorter node does not.
//
// The pairwise path walk is asserted here too, which is the point of hoisting it
// into the solution class: before this migration it lived only in the benchmark's
// baseline arm and nothing checked that the arm the DisjointSet sweep is measured
// against was even right.
public sealed class NumberOfGoodPathsTests
{
    public static TheoryData<int[], int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: the two 1s and the two 3s each connect through a
            // 2, so only the 1s pair up on top of the five trivial paths.
            { [1, 3, 2, 1, 3], [[0, 1], [0, 2], [2, 3], [2, 4]], 6 },

            // LeetCode example 2: every value equal, so all four trivial paths plus
            // all six pairs count.
            { [1, 1, 1, 1], [[0, 1], [1, 2], [2, 3]], 10 },

            // LeetCode example 3: a single node with no edges.
            { [1], [], 1 },

            // The three-node version of example 2, which the pre-section-17 test
            // asserted.
            { [1, 1, 1], [[0, 1], [1, 2]], 6 },

            // The pre-section-17 test's single-node case, with a value that is not 1.
            { [5], [], 1 },

            // A shorter node between two equal ones does not block them: 3 trivial
            // paths plus the 2-1-2 pair.
            { [2, 1, 2], [[0, 1], [1, 2]], 4 },

            // A taller node between two equal ones does block them, so only the
            // trivial paths remain.
            { [1, 3, 1], [[0, 1], [1, 2]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodPathsByPairwisePathWalk_LeetCodeExamples_CountsEveryGoodPath(
        int[] vals, int[][] edges, int expected) =>
        Assert.Equal(expected, NumberOfGoodPathsSolution.CountGoodPathsByPairwisePathWalk(vals, edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodPathsByDisjointSetSweep_LeetCodeExamples_CountsEveryGoodPath(
        int[] vals, int[][] edges, int expected) =>
        Assert.Equal(expected, NumberOfGoodPathsSolution.CountGoodPathsByDisjointSetSweep(vals, edges));
}
