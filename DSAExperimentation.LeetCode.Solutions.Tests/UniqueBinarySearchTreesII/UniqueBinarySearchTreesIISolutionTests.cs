using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.UniqueBinarySearchTreesII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.UniqueBinarySearchTreesII;

// Harness only. Both strategies are UniqueBinarySearchTreesIISolution's. CatalogCaseTests
// holds them to LeetCode's published examples, each tree read in its level-order notation
// and the trees in any order (ARCHITECTURE 17.13). This file keeps the one-node tree's own
// shape, which the recurrence reaches through branches it discards.
public sealed partial class UniqueBinarySearchTreesIISolutionTests
{
    [Fact]
    public void GenerateTreesByPlainRecursion_NOne_ReturnsSingleLeafTree() =>
        AssertSingleLeafTree(UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(1));

    [Fact]
    public void GenerateTreesByMemoizedRange_NOne_ReturnsSingleLeafTree() =>
        AssertSingleLeafTree(UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(1));

    private static void AssertSingleLeafTree(List<BinaryTreeNode<int>?> trees)
    {
        // For n = 1 the recurrence's only non-inverted branch is (1,1): the nulls it
        // lists for the inverted (1,0) and (2,1) become that leaf's absent children
        // rather than a list entry, since AppendCombinations fills the list with the
        // node it builds. IsType asks for that node instead of promising it is there.
        var tree = Assert.IsType<BinaryTreeNode<int>>(Assert.Single(trees));

        Assert.Equal(1, tree.Value);
        Assert.Null(tree.Left);
        Assert.Null(tree.Right);
    }
}
