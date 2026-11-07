using DSAExperimentation.DataStructures.LinkedList;

namespace DSAExperimentation.DataStructures.SinglyLinkedList;

// The one linked node with nothing beside its forward link: the doubly linked node without its
// back link, so this node has no Previous field, member or write (ARCHITECTURE.md §13.11). A
// genuinely mutable reference type (reference identity is what CycleDetection.FindCycleStart's
// ReferenceEquals walk needs), with a settable Value - many list problems rewrite a node's value in
// place (e.g. Delete Node in a Linked List copies the next node's value into the current node), not
// just its Next. Primary constructor for the common "new node with this value" call shape, matching
// BinaryTreeNode's precedent.
/// <summary>A node of a singly linked list, holding a value and a link to the next node.</summary>
/// <param name="value">The value the node starts with.</param>
public sealed class SinglyLinkedListNode<TValue>(TValue value)
    : LinkedNode<SinglyLinkedListNode<TValue>, TValue>(value);
