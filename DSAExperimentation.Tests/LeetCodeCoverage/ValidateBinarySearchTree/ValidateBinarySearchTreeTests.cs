using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinarySearchTree;

public sealed partial class ValidateBinarySearchTreeTests
{
    [Fact] public void IsValidBST_ValidTree_ReturnsTrue() => Assert.True(IsValid(new BinaryTreeNode<int>(2) { Left = new(1), Right = new(3) }));
    [Fact] public void IsValidBST_InvalidTree_ReturnsFalse() => Assert.False(IsValid(new BinaryTreeNode<int>(5) { Left = new(1), Right = new(4) { Left = new(3), Right = new(6) } }));
    private static bool IsValid(BinaryTreeNode<int>? root) => Validate(root, null, null);
    private static bool Validate(BinaryTreeNode<int>? node, int? min, int? max) => node is null || ((min is null || node.Value > min) && (max is null || node.Value < max) && Validate(node.Left, min, node.Value) && Validate(node.Right, node.Value, max));
}
