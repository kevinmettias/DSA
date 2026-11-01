namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// Mirrors DoublyLinkedListNode minus Previous: a genuinely mutable reference type
// (reference identity is what CycleDetection.FindCycleStart's ReferenceEquals
// walk needs), settable Value - many list problems rewrite a node's value in
// place (e.g. Delete Node in a Linked List copies the next node's value into the
// current node), not just its Next. Primary constructor for the common "new node
// with this value" call shape, matching BinaryTreeNode's precedent.
/// <summary>A node of a singly linked list, holding a value and a link to the next node.</summary>
/// <param name="value">The value the node starts with.</param>
public sealed class SinglyLinkedListNode<TValue>(TValue value)
{
    /// <summary>The value the node holds.</summary>
    public TValue Value { get; set; } = value;

    /// <summary>The next node in the list, or null when this node is the last.</summary>
    public SinglyLinkedListNode<TValue>? Next { get; set; }
}
