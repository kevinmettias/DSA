using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.SmallestSubtreeWithAllTheDeepestNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestSubtreeWithAllTheDeepestNodes;

// Harness only. Both strategies are SmallestSubtreeWithAllTheDeepestNodesSolution's -
// the hand-rolled (depth, candidate) recursion and the TreeFold pass over
// DeepestSubtreeAlgebra. Examples are stated in LeetCode's own level-order-with-null
// array shape (BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData signature), and the expected answer is the value of the subtree root
// LeetCode reports - values are distinct in every example, so the value identifies
// exactly one node of the tree.
public sealed partial class SmallestSubtreeWithAllTheDeepestNodesTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [3, 5, 1, 6, 2, 0, 8, null, null, 7, 4], 2 },
            { [1], 1 },
            { [0, 1, 3, null, 2], 2 },
            { [1, 2, 3, 4, 5, 6, 7], 1 },
            { [0, 1, null, null, 3], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubtreeWithAllDeepestByRecursion_LeetCodeExamples_ReturnsSmallestSubtreeRoot(
        int?[] levelOrder, int expected) =>
        Assert.Equal(
            expected,
            SubtreeRoot(
                SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByRecursion(LeetCodeWireFormat.ToBinaryTree(levelOrder))).Value);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SubtreeWithAllDeepestByTreeFold_LeetCodeExamples_ReturnsSmallestSubtreeRoot(
        int?[] levelOrder, int expected) =>
        Assert.Equal(
            expected,
            SubtreeRoot(
                SmallestSubtreeWithAllTheDeepestNodesSolution.SubtreeWithAllDeepestByTreeFold(LeetCodeWireFormat.ToBinaryTree(levelOrder))).Value);

    // Both strategies return the deepest subtree's root, and both return null only
    // for a null root - which no example above has, since a level-order array names
    // its root in slot 0. IsType asks for that node and fails the test if it is
    // absent, rather than promising it to the compiler.
    private static BinaryTreeNode<int> SubtreeRoot(BinaryTreeNode<int>? subtree) =>
        Assert.IsType<BinaryTreeNode<int>>(subtree);
}
