using DSAExperimentation.LeetCode.Solutions.SmallestMissingGeneticValueInEachSubtree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestMissingGeneticValueInEachSubtree;

// Harness only. Both strategies are SmallestMissingGeneticValueInEachSubtreeSolution's
// and the tree they walk is DataStructures' ParentArrayTree - this file just pins them
// to the two the pre-migration test carried. The
// subtree-rescan baseline was previously untested scaffolding inlined in the benchmark;
// it gets the same coverage as the composed walk here for the first time.
public sealed partial class SmallestMissingGeneticValueInEachSubtreeSolutionTests
{
    public static TheoryData<int[], int[], int[]> Cases =>
        new()
        {
            // Branching tree with value 1 part-way down a branch: 0(5) -> {1(1), 2(3)},
            // 1 -> 3(2).
            { [-1, 0, 0, 1], [5, 1, 3, 2], [4, 3, 1, 1] },

            // Chain 0 -> 1 -> 2 with no node holding value 1.
            { [-1, 0, 1], [10, 20, 30], [1, 1, 1] },

            // Single node, holding value 1 itself: the walk starts and ends on the root.
            { [-1], [1], [2] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestMissingValuesBySubtreeRescan_Cases_ReturnsEachSubtreesMissingValue(
        int[] parents, int[] nums, int[] expected)
    {
        var actual = SmallestMissingGeneticValueInEachSubtreeSolution.SmallestMissingValuesBySubtreeRescan(
            parents, nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestMissingValuesByAncestorChain_Cases_ReturnsEachSubtreesMissingValue(
        int[] parents, int[] nums, int[] expected)
    {
        var actual = SmallestMissingGeneticValueInEachSubtreeSolution.SmallestMissingValuesByAncestorChain(
            parents, nums);

        Assert.Equal(expected, actual);
    }
}
