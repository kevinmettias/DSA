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
internal sealed class DoublyLinkedList<TValue>
{
    private readonly DoublyLinkedListNode<TValue> _head = new();
    private readonly DoublyLinkedListNode<TValue> _tail = new();

    public int Count { get; private set; }

    public DoublyLinkedList()
    {
        _head.Next = _tail;
        _tail.Previous = _head;
    }

    public void AddFront(DoublyLinkedListNode<TValue> node)
    {
        var previousFront = _head.Next!;

        node.Previous = _head;
        node.Next = previousFront;
        previousFront.Previous = node;
        _head.Next = node;

        Count++;
    }

    public void Remove(DoublyLinkedListNode<TValue> node)
    {
        node.Previous!.Next = node.Next;
        node.Next!.Previous = node.Previous;

        Count--;
    }

    public DoublyLinkedListNode<TValue> PopBack()
    {
        var node = _tail.Previous!;
        Remove(node);
        return node;
    }
}
