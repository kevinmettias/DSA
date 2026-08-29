namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A plain mutable reference type, not a record: reference identity is exactly what
// LowestCommonAncestor's node.Equals(first) needs (two distinct leaves holding the
// same Value must never compare equal). Value and Left/Right are all settable -
// Left/Right for mutate-in-place operations (invert, flatten), Value for problems
// that rewrite a node's own value in place (e.g. Convert BST to Greater Tree,
// Trim a Binary Search Tree) - the same "genuinely mutable reference type...
// auto-properties, not bare fields" precedent DoublyLinkedListNode/
// SinglyLinkedListNode already establish.
internal sealed class BinaryTreeNode<TValue>(TValue value)
{
    public TValue Value { get; set; } = value;

    public BinaryTreeNode<TValue>? Left { get; set; }

    public BinaryTreeNode<TValue>? Right { get; set; }
}
