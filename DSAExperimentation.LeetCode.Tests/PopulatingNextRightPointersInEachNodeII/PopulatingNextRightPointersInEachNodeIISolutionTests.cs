using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.LeetCode.Tests.PopulatingNextRightPointersInEachNodeII;

// Harness only. Both strategies are PopulatingNextRightPointersInEachNodeIISolution's. Each row is a
// tree in LeetCode's level order and LeetCode's readout of it once connected - each level along its
// next pointers, '#' (null here) closing it. The first two rows are LeetCode's published examples,
// where 5's next pointer reaches across 3's missing left child to 7. The rest are read by hand:
// a single node; a tree whose second level starts its children at 3, not 2, so the readout must
// find the next level past a childless node; and a right-leaning chain, one node per level.
public sealed partial class PopulatingNextRightPointersInEachNodeIISolutionTests
{
    public static TheoryData<int?[], int?[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, null, 7], [1, null, 2, 3, null, 4, 5, 7, null] },
            { [], [] },
            { [1], [1, null] },
            { [1, 2, 3, null, null, 4, 5], [1, null, 2, 3, null, 4, 5, null] },
            { [1, null, 2, null, 3], [1, null, 2, null, 3, null] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByManualQueueBfs_LeetCodeExamples_ReadsEachLevelAlongItsNextPointers(
        int?[] levelOrder, int?[] expected) =>
        Assert.Equal(
            expected,
            PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(LeetCodeWireFormat.ToBinaryTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodeExamples_ReadsEachLevelAlongItsNextPointers(
        int?[] levelOrder, int?[] expected) =>
        Assert.Equal(
            expected,
            PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(levelOrder)));
}
