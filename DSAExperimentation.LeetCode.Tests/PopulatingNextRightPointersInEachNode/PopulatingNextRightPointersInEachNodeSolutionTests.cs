using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

namespace DSAExperimentation.LeetCode.Tests.PopulatingNextRightPointersInEachNode;

// Harness only. Both strategies are PopulatingNextRightPointersInEachNodeSolution's. Each row is a
// perfect tree in LeetCode's level order and LeetCode's readout of it once connected - each level
// along its next pointers, '#' (null here) closing it. The first two rows are LeetCode's published
// examples; the other two are perfect trees of one and two levels, read the same way by hand.
public sealed partial class PopulatingNextRightPointersInEachNodeSolutionTests
{
    public static TheoryData<int?[], int?[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6, 7], [1, null, 2, 3, null, 4, 5, 6, 7, null] },
            { [], [] },
            { [1], [1, null] },
            { [1, 2, 3], [1, null, 2, 3, null] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByManualQueueBfs_LeetCodePerfectTrees_ReadsEachLevelAlongItsNextPointers(
        int?[] levelOrder, int?[] expected) =>
        Assert.Equal(
            expected,
            PopulatingNextRightPointersInEachNodeSolution.ConnectByManualQueueBfs(LeetCodeWireFormat.ToBinaryTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodePerfectTrees_ReadsEachLevelAlongItsNextPointers(
        int?[] levelOrder, int?[] expected) =>
        Assert.Equal(
            expected,
            PopulatingNextRightPointersInEachNodeSolution.ConnectByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(levelOrder)));
}
