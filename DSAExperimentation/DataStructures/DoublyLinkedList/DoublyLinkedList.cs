namespace DSAExperimentation.DataStructures.DoublyLinkedList;

// Concrete, not behind an interface - there is exactly one doubly-linked-list
// representation today. See ARCHITECTURE.md §5: an interface here would be a speculative
// abstraction with no second implementation to justify it. Shared by LruCache and
// LfuCache (composing this already-public concrete Representation the way Stack composes
// DynamicArray, Queue composes Deque, and Set composes HashMap<T,bool> - §5 step 5 /
// §4.1) rather than each duplicating intrusive splice logic, a classically bug-prone
// area (dangling Previous/Next, sentinel mishandling).
//
// Sentinel-based: permanent dummy head/tail nodes, never removed, so every splice is
// branch-free - no "is this the first/last real node" special case anywhere below.
// Raw and unchecked like HeapArray/CircularBuffer, not Heap/Deque: AddFront/Remove/
// PopBack assume a valid node reference and a non-empty list respectively; callers
// (LruCache, LfuCache) are trusted to check Count/track membership before calling, the
// same split of responsibility Heap has with HeapArray. AddFront/Remove/PopBack are
// O(1) by construction (pointer reassignment, no traversal) - LruCache's and LfuCache's
// own O(1) get/set claims depend on this (§8).
/// <summary>
/// A doubly linked list of caller-owned nodes, linked and unlinked in O(1) with no precondition checks.
/// </summary>
public sealed class DoublyLinkedList<TValue>
{
    private readonly DoublyLinkedListNode<TValue> _head = new();
    private readonly DoublyLinkedListNode<TValue> _tail = new();

    /// <summary>The number of nodes linked into the list.</summary>
    public int Count { get; private set; }

    public DoublyLinkedList()
    {
        _head.Next = _tail;
        _tail.Previous = _head;
    }

    /// <summary>Links a node in at the front of the list.</summary>
    /// <param name="node">A node not currently linked into any list.</param>
    public void AddFront(DoublyLinkedListNode<TValue> node)
    {
        var previousFront = _head.Next!;

        node.Previous = _head;
        node.Next = previousFront;
        previousFront.Previous = node;
        _head.Next = node;

        Count++;
    }

    /// <summary>Unlinks a node from the list without clearing the node's own links.</summary>
    /// <param name="node">A node currently linked into this list.</param>
    public void Remove(DoublyLinkedListNode<TValue> node)
    {
        node.Previous!.Next = node.Next;
        node.Next!.Previous = node.Previous;

        Count--;
    }

    /// <summary>Unlinks and returns the node at the back of the list, which must not be empty.</summary>
    /// <returns>The back node: of the nodes still linked, the one added longest ago.</returns>
    public DoublyLinkedListNode<TValue> PopBack()
    {
        var node = _tail.Previous!;
        Remove(node);
        return node;
    }
}
