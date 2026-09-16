using DSAExperimentation.LeetCode.SmallestMissingGeneticValueInEachSubtree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestMissingGeneticValueInEachSubtree;

// Harness only. Both strategies are SmallestMissingGeneticValueInEachSubtreeSolution's
// and the tree they walk is DataStructures' ParentArrayTree - this file just pins them
// to LeetCode's published examples plus the two the pre-migration test carried. The
// subtree-rescan baseline was previously untested scaffolding inlined in the benchmark;
// it gets the same coverage as the composed walk here for the first time.
public sealed class SmallestMissingGeneticValueInEachSubtreeTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            // LeetCode example 1: only the root's subtree holds every value 1..4.
            { [-1, 0, 0, 2], [1, 2, 3, 4], [5, 1, 1, 1] },

            // LeetCode example 2: the ancestor chain from the node holding 1 (node 4)
            // up to the root is the only place a non-trivial answer can appear.
            { [-1, 0, 1, 0, 3, 3], [5, 4, 6, 2, 1, 3], [7, 1, 1, 4, 2, 1] },

            // LeetCode example 3: no node holds value 1, so every answer is 1.
            { [-1, 2, 3, 0, 2, 4, 1], [2, 3, 4, 5, 6, 7, 8], [1, 1, 1, 1, 1, 1, 1] },

            // Branching tree with value 1 part-way down a branch: 0(5) -> {1(1), 2(3)},
            // 1 -> 3(2).
            { [-1, 0, 0, 1], [5, 1, 3, 2], [4, 3, 1, 1] },

            // Chain 0 -> 1 -> 2 with no node holding value 1.
            { [-1, 0, 1], [10, 20, 30], [1, 1, 1] },

            // Single node, holding value 1 itself: the walk starts and ends on the root.
            { [-1], [1], [2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestMissingValuesBySubtreeRescan_LeetCodeExamples_ReturnsEachSubtreesMissingValue(
        int[] parents, int[] nums, int[] expected)
    {
        var actual = SmallestMissingGeneticValueInEachSubtreeSolution.SmallestMissingValuesBySubtreeRescan(
            parents, nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestMissingValuesByAncestorChain_LeetCodeExamples_ReturnsEachSubtreesMissingValue(
        int[] parents, int[] nums, int[] expected)
    {
        var actual = SmallestMissingGeneticValueInEachSubtreeSolution.SmallestMissingValuesByAncestorChain(
            parents, nums);

        Assert.Equal(expected, actual);
    }
}
