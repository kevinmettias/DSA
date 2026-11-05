using DSAExperimentation.LeetCode.Solutions.NumberOfGoodPaths;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfGoodPaths;

// Harness only. Both strategies are NumberOfGoodPathsSolution's - this file pins
// them to shapes LeetCode's examples never reach: all-equal chains, where every pair
// counts, a lone node, a chain where a taller node blocks the only path between two
// equal ones, and a chain where a shorter node does not. LeetCode's published
// examples run in CatalogCaseTests.
//
// The pairwise path walk is asserted here too, which is the point of hoisting it
// into the solution class: before this migration it lived only in the benchmark's
// baseline arm and nothing checked that the arm the DisjointSet sweep is measured
// against was even right.
public sealed partial class NumberOfGoodPathsSolutionTests
{
    public static TheoryData<int[], int[][], int> Cases =>
        new()
        {
            // Every value equal, so all four trivial paths plus all six pairs count.
            { [1, 1, 1, 1], [[0, 1], [1, 2], [2, 3]], 10 },

            // The three-node version of the row above, which the pre-section-17 test
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
    [MemberData(nameof(Cases))]
    public void CountGoodPathsByPairwisePathWalk_Cases_CountsEveryGoodPath(
        int[] vals, int[][] edges, int expected)
    {
        var actual = NumberOfGoodPathsSolution.CountGoodPathsByPairwisePathWalk(vals, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGoodPathsByDisjointSetSweep_Cases_CountsEveryGoodPath(
        int[] vals, int[][] edges, int expected)
    {
        var actual = NumberOfGoodPathsSolution.CountGoodPathsByDisjointSetSweep(vals, edges);

        Assert.Equal(expected, actual);
    }
}
