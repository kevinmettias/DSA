using DSAExperimentation.LeetCode.ConstructBinaryTreeFromInorderAndPostorderTraversal;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructBinaryTreeFromInorderAndPostorderTraversal;

// Harness only: BuildByPostorderIndexMap lives in
// ConstructBinaryTreeFromInorderAndPostorderTraversalSolution. Each example's expected
// tree is LeetCode's published output in its own level-order notation, and the built
// tree is printed back into that notation to compare.
public sealed partial class ConstructBinaryTreeFromInorderAndPostorderTraversalTests
{
    public static TheoryData<TraversalExample> Examples =>
        new()
        {
            { new TraversalExample([9, 3, 15, 20, 7], [9, 15, 7, 20, 3], [3, 9, 20, null, null, 15, 7]) },
            { new TraversalExample([-1], [-1], [-1]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByPostorderIndexMap_LeetCodeExamples_ReconstructsBinaryTree(TraversalExample example)
    {
        var root = ConstructBinaryTreeFromInorderAndPostorderTraversalSolution.BuildByPostorderIndexMap(
            example.Inorder, example.Postorder);

        Assert.Equal(example.Expected, LeetCodeWireFormat.FromBinaryTree(root));
    }

    // One LeetCode example: the two traversals given, and the tree they determine in
    // LeetCode's level-order notation. The two traversals share a type, so each is named.
    public readonly record struct TraversalExample(int[] Inorder, int[] Postorder, int?[] Expected);
}
