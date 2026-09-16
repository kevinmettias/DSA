using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DiameterOfBinaryTree;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DiameterOfBinaryTree;

// LeetCode 543. Diameter of Binary Tree. See DiameterOfBinaryTreeSolution for the
// two strategies: a naive recompute-height-per-node baseline, and this repo's own
// TreeMetrics.Diameter fold. Examples are stated as LeetCode's own level-order
// arrays - BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData member; LeetCodeWireFormat.ToBinaryTree reconstructs it inside each test method instead.
public sealed partial class DiameterOfBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples()
    {
        var examples = new TheoryData<int?[], int>
        {
            { [1, 2, 3, 4, 5], 3 },
            { [1, 2], 1 },
            { [1], 0 },
        };

        return examples;
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DiameterByRecomputedHeightPerNode_ReturnsLongestPathEdgeCount(int?[] levelOrder, int expected)
        => Assert.Equal(expected, DiameterOfBinaryTreeSolution.DiameterByRecomputedHeightPerNode(LeetCodeWireFormat.ToBinaryTree(levelOrder)!));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DiameterByTreeMetricsFold_ReturnsLongestPathEdgeCount(int?[] levelOrder, int expected)
        => Assert.Equal(expected, DiameterOfBinaryTreeSolution.DiameterByTreeMetricsFold(LeetCodeWireFormat.ToBinaryTree(levelOrder)!));
}
