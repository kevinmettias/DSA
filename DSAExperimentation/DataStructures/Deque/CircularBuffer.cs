using System.Numerics;

namespace DSAExperimentation.DataStructures.Deque;

// A wraparound array: elements occupy [_head, _head+Count) modulo the buffer's
// length, so both ends can grow without shifting the other end's elements. Growing
// re-linearizes into a fresh array starting at index 0. Callers (Deque) are trusted
// to check Count before calling GetFront/GetBack/RemoveFront/RemoveBack, the same
// split of responsibility Heap has with HeapArray.
//
// The length is always a power of two, so wrapping an index is a mask rather than a
// %. Every index operation used to divide, and a division costs tens of cycles where
// a mask costs one: twelve of them sat on each element's path through a two-deque
// sliding window (ARCHITECTURE 19.8). The starting length and every grown length are
// rounded up to a power of two here, so the mask stays right whatever ArrayGrowth's
// numbers are.
internal sealed class CircularBuffer<Element>
{
    private Element[] _items = new Element[BitOperations.RoundUpToPowerOf2((uint)ArrayGrowth.InitialCapacity)];
    private int _head;

    public int Count { get; private set; }

    public Element GetFront() => _items[_head];

    public Element GetBack() => _items[BackIndex()];

    public void AddFront(Element value)
    {
        EnsureCapacity(Count + 1);
        _head = Wrap(_head - 1);
        _items[_head] = value;
        Count++;
    }

    public void AddBack(Element value)
    {
        EnsureCapacity(Count + 1);
        _items[Wrap(_head + Count)] = value;
        Count++;
    }

    public void RemoveFront()
    {
        // presumption: allow -- the vacated slot is unreachable once _head moves past
        // it; this only drops a stale reference for the GC, not a correctness need.
        _items[_head] = default!;
        _head = Wrap(_head + 1);
        Count--;
    }

    public void RemoveBack()
    {
        // presumption: allow -- the vacated slot is unreachable once Count shrinks
        // past it; this only drops a stale reference for the GC, not a correctness need.
        _items[BackIndex()] = default!;
        Count--;
    }

    private int BackIndex() => Wrap(_head + Count - 1);

    private void EnsureCapacity(int required)
    {
        if (required <= _items.Length)
        {
            return;
        }

        var grown = Math.Max(_items.Length * ArrayGrowth.GrowthFactor, required);
        var resized = new Element[BitOperations.RoundUpToPowerOf2((uint)grown)];

        for (var i = 0; i < Count; i++)
        {
            resized[i] = _items[Wrap(_head + i)];
        }

        _items = resized;
        _head = 0;
    }

    // The buffer's length is a power of two, so the low bits are the index modulo it - and
    // two's complement makes -1 land on the last slot, so stepping back from index 0 wraps
    // the same way stepping forward past the end does.
    private int Wrap(int index) => index & (_items.Length - 1);
}
