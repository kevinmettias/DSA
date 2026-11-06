using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Tests;

public sealed partial class LeetCodeQuadTreesTests
{
    // Construct Quad Tree's first example, and Logical OR's second input: a leaf's four nulls before a sibling's
    // children, and the trailing nulls dropped.
    public static TheoryData<int[]?[]> LevelOrders =>
        new()
        {
            { [[0, 1], [1, 0], [1, 1], [1, 1], [1, 0]] },
            { [[0, 1], [1, 1], [0, 1], [1, 1], [1, 0], null, null, null, null, [1, 0], [1, 0], [1, 1], [1, 1]] },
            { [[1, 1]] },
            { [] },
        };

    [Theory]
    [MemberData(nameof(LevelOrders))]
    public void LevelOrderOf_TreeBuiltFromTheLevelOrder_ReturnsLeetCodesOwnLevelOrder(int[]?[] levelOrder)
    {
        var root = LeetCodeQuadTrees.From(levelOrder);

        Assert.Equal(levelOrder, LeetCodeQuadTrees.LevelOrderOf(root));
    }

    [Fact]
    public void IsSameTree_InnerValuesDiffering_IsTrue()
    {
        var recorded = LeetCodeQuadTrees.From([[0, 0], [1, 1], [1, 1], [1, 1], [1, 0]]);
        var answered = LeetCodeQuadTrees.From([[0, 1], [1, 1], [1, 1], [1, 1], [1, 0]]);
        var isSame = LeetCodeQuadTrees.IsSameTree(answered, recorded);

        Assert.True(isSame);
    }

    [Fact]
    public void IsSameTree_LeafValuesDiffering_IsFalse()
    {
        var recorded = LeetCodeQuadTrees.From([[0, 0], [1, 1], [1, 1], [1, 1], [1, 0]]);
        var answered = LeetCodeQuadTrees.From([[0, 0], [1, 1], [1, 1], [1, 1], [1, 1]]);
        var isSame = LeetCodeQuadTrees.IsSameTree(answered, recorded);

        Assert.False(isSame);
    }

    [Fact]
    public void From_LeafEntry_IsALeafOfItsValue()
    {
        var root = Assert.IsType<QuadTreeNode>(LeetCodeQuadTrees.From([[1, 0]]));

        Assert.Equal((true, false), (root.IsLeaf, root.Val));
    }
}
