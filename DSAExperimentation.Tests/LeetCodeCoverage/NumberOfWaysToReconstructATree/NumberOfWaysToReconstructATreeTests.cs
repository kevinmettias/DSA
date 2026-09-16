using DSAExperimentation.LeetCode.NumberOfWaysToReconstructATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToReconstructATree;

// Harness only. Both strategies live in NumberOfWaysToReconstructATreeSolution -
// this file pins them to LeetCode's three published examples plus four cases that
// separate the two ways an answer of 0 can arise (no node is related to everyone,
// versus a node whose neighbours are not all shared with its only possible parent)
// and the two ways an answer of 2 can arise (a tie at the minimal qualifying
// degree, versus a node tied with its own parent).
public sealed partial class NumberOfWaysToReconstructATreeTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode's example 1: 1 -> 2 -> 3 is the only tree with this relation.
            { [[1, 2], [2, 3]], 1 },

            // LeetCode's example 2: every node is related to every other, so any of
            // the three can be the root of a chain.
            { [[1, 2], [2, 3], [1, 3]], 2 },

            // LeetCode's example 3: no node is related to all four others, so no
            // node can be the root.
            { [[1, 2], [2, 3], [2, 4], [1, 5]], 0 },

            // Two nodes both end up related to everyone: 1 -> 4 -> {2, 3} reproduces
            // the same relation as 4 -> 1 -> {2, 3}, despite only one of them being
            // the actual root of either tree.
            { [[1, 2], [1, 3], [1, 4], [2, 4], [3, 4]], 2 },

            // The smallest ambiguous input: either node can be the other's parent.
            { [[1, 2]], 2 },

            // A forced three-level tree - 1 -> {5, 3} and 3 -> {2, 4} - where every
            // non-root node's parent is the unique lowest qualifying degree.
            { [[1, 2], [1, 3], [1, 4], [1, 5], [2, 3], [3, 4]], 1 },

            // Node 1 is still related to everyone, so the degree check passes; but
            // node 2's only possible parent is 3, and node 2 is related to 5 while
            // node 3 is not - which transitivity forbids.
            { [[1, 2], [1, 3], [1, 4], [1, 5], [2, 3], [3, 4], [2, 5]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckWaysByDictionaryAndLinqSort_LeetCodeExamples_ReturnsReconstructionCount(
        int[][] pairs, int expected) =>
        Assert.Equal(expected, NumberOfWaysToReconstructATreeSolution.CheckWaysByDictionaryAndLinqSort(pairs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckWaysByHashMapAndMergeSort_LeetCodeExamples_ReturnsReconstructionCount(
        int[][] pairs, int expected) =>
        Assert.Equal(expected, NumberOfWaysToReconstructATreeSolution.CheckWaysByHashMapAndMergeSort(pairs));
}
