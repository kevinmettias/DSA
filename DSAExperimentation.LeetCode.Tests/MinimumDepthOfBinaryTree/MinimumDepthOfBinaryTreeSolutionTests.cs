using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

namespace DSAExperimentation.LeetCode.Tests.MinimumDepthOfBinaryTree;

// Harness only. Both strategies live in MinimumDepthOfBinaryTreeSolution - the
// recursive walk and the level-order walk - and are asserted against LeetCode's
// published examples, the original test's right-skewed case, and the empty-tree
// edge case none of the pre-migration files exercised. BinaryTreeNode<int> is
// internal, so - as in SameTreeTests - it stays out of a public TheoryData
// signature and LeetCodeWireFormat.ToBinaryTree reconstructs it from LeetCode's own
// level-order-with-null array shape.
public sealed partial class MinimumDepthOfBinaryTreeSolutionTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [], 0 },
            // The original test's right-skewed case, root = [1,2,3,null,null,null,4] -> 2:
            // the root's left child is the only shallow leaf.
            { [1, 2, 3, null, null, null, 4], 2 },
            // LeetCode 111's own example: root = [3,9,20,null,null,15,7] -> 2.
            { [3, 9, 20, null, null, 15, 7], 2 },
            // LeetCode 111's own example: root = [2,null,3,null,4,null,5,null,6] -> 5.
            // A leaf is only reachable by descending every right child, so the minimum
            // depth equals the maximum depth here.
            { [2, null, 3, null, 4, null, 5, null, 6], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDepthByRecursion_Examples_ReturnsShortestLeafDepth(int?[] values, int expectedDepth)
    {
        var depth = MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(LeetCodeWireFormat.ToBinaryTree(values));

        Assert.Equal(expectedDepth, depth);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDepthByBreadthFirstSearch_Examples_ReturnsShortestLeafDepth(int?[] values, int expectedDepth)
    {
        var depth = MinimumDepthOfBinaryTreeSolution.MinDepthByBreadthFirstSearch(LeetCodeWireFormat.ToBinaryTree(values));

        Assert.Equal(expectedDepth, depth);
    }
}
