namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A plain mutable reference type, not a record: reference identity is exactly what
// LowestCommonAncestor's node.Equals(first) needs (two distinct leaves holding the
// same Value must never compare equal). Value and Left/Right are all settable -
// Left/Right for mutate-in-place operations (invert, flatten), Value for problems
// that rewrite a node's own value in place (e.g. Convert BST to Greater Tree,
// Trim a Binary Search Tree) - the same "genuinely mutable reference type...
// auto-properties, not bare fields" precedent DoublyLinkedListNode/
// SinglyLinkedListNode already establish.
/// <summary>A mutable binary tree node holding a value and an optional left and right child.</summary>
/// <param name="value">The node's initial value.</param>
public sealed class BinaryTreeNode<TValue>(TValue value)
{
    /// <summary>The value this node holds.</summary>
    public TValue Value { get; set; } = value;

    /// <summary>The left child, or null when there is none.</summary>
    public BinaryTreeNode<TValue>? Left { get; set; }

    /// <summary>The right child, or null when there is none.</summary>
    public BinaryTreeNode<TValue>? Right { get; set; }
}
