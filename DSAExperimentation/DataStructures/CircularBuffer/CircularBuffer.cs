using System.Runtime.CompilerServices;

namespace DSAExperimentation.DataStructures.CircularBuffer;

// The general wraparound structure the queue family specializes, the way Graph narrows to DAG and
// then to Tree (ARCHITECTURE 3, 20): elements added, read and removed at either end, as many as the
// injected capacity allows. Two kinds of constraint make the specialized structures, and they enter
// differently. A constraint that changes how an operation behaves is a type argument: TCapacity
// decides whether a push is refused, and Deque and BoundedDeque are this type with it unbounded or
// bounded. They derive from it with the argument fixed, so each is the buffer itself rather than an
// object wrapping one, which is why this class is not sealed. A constraint that changes which
// operations exist is a facade: Queue and BoundedQueue keep only a push at the back and a pop at the
// front, which a type argument cannot express, since it cannot remove a member.
//
// A wraparound array: elements occupy [_head, _head+Count) modulo the array's length, so both ends
// can grow without shifting the other end's elements, and growing re-linearizes into a fresh array
// starting at index 0. The length is always a power of two, so wrapping an index is a mask rather
// than a %: twelve divisions once sat on each element's path through a two-deque sliding window
// (ARCHITECTURE 19.8). A bound does not change that: it is checked against Count, and the array grows
// to the power of two that covers it.
//
// The array is this class's own, not a Storage half's (ARCHITECTURE 14). Every object between a
// configuration and its array is a load on every operation, and one the JIT cannot hoist out of a
// loop that makes a call, so the queues, which reach the buffer through a facade, are already one
// object from it (ARCHITECTURE 20).
//
// No throwing PeekFront/PopFront convenience: whether the buffer is empty, or full, is state an
// external caller can't always know in advance, so the TryX forms are the only public surface for
// it, forcing callers onto the return-value form instead of a try/catch.
/// <summary>
/// A wraparound buffer that adds, reads and removes elements at either end, refusing a push its capacity has no room for.
/// </summary>
/// <param name="capacity">How many elements the buffer may hold.</param>
public class CircularBuffer<Element, TCapacity>(TCapacity capacity)
    where TCapacity : struct, IBufferCapacity
{
    private readonly TCapacity _capacity = capacity;

    // ArrayGrowth starts at a power of two and grows by one, so the length stays a power of two with
    // no rounding (CircularBufferTests pins both). Rounding a constant cost nothing at run time, but
    // the inliner charged for it, at construction and inside every push (ARCHITECTURE 20).
    private Element[] _items = new Element[ArrayGrowth.InitialCapacity];
    private int _head;

    /// <summary>The number of elements in the buffer.</summary>
    public int Count { get; private set; }

    /// <summary>Whether the buffer holds as many elements as its capacity allows, so that a push is refused.</summary>
    public bool IsFull => _capacity.IsFullAt(Count);

    // Both pushes ask the witness directly rather than through IsFull, and are always inlined.
    // MonotonicDeque.Push read in the JIT's output: with the pop path inlined, the JIT's budget left
    // IsFull, and then the whole push, as a call on every element, where its hand-written deque had
    // inlined the push (ARCHITECTURE 20). A push is two compares and a store, and the attribute is
    // for a forward that small.
    /// <summary>Adds an element at the front, unless the buffer is full.</summary>
    /// <param name="item">The element to add.</param>
    /// <returns>true when the element was added; false, changing nothing, when the buffer is full.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPushFront(Element item)
    {
        if (_capacity.IsFullAt(Count))
        {
            return false;
        }

        if (Count == _items.Length)
        {
            Grow();
        }

        _head = Wrap(_head - 1);
        _items[_head] = item;
        Count++;
        return true;
    }

    /// <summary>Adds an element at the back, unless the buffer is full.</summary>
    /// <param name="item">The element to add.</param>
    /// <returns>true when the element was added; false, changing nothing, when the buffer is full.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPushBack(Element item)
    {
        if (_capacity.IsFullAt(Count))
        {
            return false;
        }

        if (Count == _items.Length)
        {
            Grow();
        }

        _items[Wrap(_head + Count)] = item;
        Count++;
        return true;
    }

    /// <summary>Reads the element at the front without removing it.</summary>
    /// <param name="item">The front element when the buffer is not empty; otherwise the default value.</param>
    /// <returns>true when the buffer holds an element; false when it is empty.</returns>
    public bool TryPeekFront(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items[_head];
        return true;
    }

    /// <summary>Reads the element at the back without removing it.</summary>
    /// <param name="item">The back element when the buffer is not empty; otherwise the default value.</param>
    /// <returns>true when the buffer holds an element; false when it is empty.</returns>
    public bool TryPeekBack(out Element item)
    {
        if (Count == 0)
        {
            // presumption: allow -- item is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            item = default!;
            return false;
        }

        item = _items[BackIndex()];
        return true;
    }

    /// <summary>Removes the element at the front.</summary>
    /// <param name="item">The removed element when the buffer was not empty; otherwise the default value.</param>
    /// <returns>true when an element was removed; false, changing nothing, when the buffer is empty.</returns>
    public bool TryPopFront(out Element item)
    {
        if (!TryPeekFront(out item))
        {
            return false;
        }

        // presumption: allow -- the vacated slot is unreachable once _head moves past
        // it; this only drops a stale reference for the GC, not a correctness need.
        _items[_head] = default!;
        _head = Wrap(_head + 1);
        Count--;
        return true;
    }

    /// <summary>Removes the element at the back.</summary>
    /// <param name="item">The removed element when the buffer was not empty; otherwise the default value.</param>
    /// <returns>true when an element was removed; false, changing nothing, when the buffer is empty.</returns>
    public bool TryPopBack(out Element item)
    {
        if (!TryPeekBack(out item))
        {
            return false;
        }

        // presumption: allow -- the vacated slot is unreachable once Count shrinks
        // past it; this only drops a stale reference for the GC, not a correctness need.
        _items[BackIndex()] = default!;
        Count--;
        return true;
    }

    private int BackIndex() => Wrap(_head + Count - 1);

    // Only ever reached with every slot taken, and always inlined, as the growth it replaced was. As a
    // call it sat inside every loop that pushes, and the JIT keeps no field in a register across a
    // call, so a breadth-first search re-read its queue's fields on every enqueue and dequeue
    // (ARCHITECTURE 20).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow()
    {
        var resized = new Element[_items.Length * ArrayGrowth.GrowthFactor];

        for (var i = 0; i < Count; i++)
        {
            resized[i] = _items[Wrap(_head + i)];
        }

        _items = resized;
        _head = 0;
    }

    // The array's length is a power of two, so the low bits are the index modulo it - and two's
    // complement makes -1 land on the last slot, so stepping back from index 0 wraps the same way
    // stepping forward past the end does.
    private int Wrap(int index) => index & (_items.Length - 1);
}
