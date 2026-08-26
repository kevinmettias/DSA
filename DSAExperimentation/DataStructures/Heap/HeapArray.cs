namespace DSAExperimentation.DataStructures.Heap;

// Concrete, not behind an interface - there is exactly one heap representation today. See
// ARCHITECTURE.md §5: an interface here would be the same speculative abstraction IChildren's
// own doc comment warns against, since no second implementation exists yet to justify one.
//
// Get/Set/Swap are O(1) by construction (List<Element>'s indexer) - Heap<Element,TOrder>'s O(log n) push/pop
// claim depends on this. See ARCHITECTURE.md §8: swapping this backing store for anything with
// O(n) indexed access would silently degrade that claim to O(n log n) with no compiler error and
// no failing test to catch it.
internal sealed class HeapArray<Element>
{
    private readonly List<Element> _items = [];

    public int Count => _items.Count;

    public Element Get(int index) => _items[index];

    public void Set(int index, Element value) => _items[index] = value;

    public void Add(Element value) => _items.Add(value);

    public void RemoveLast() => _items.RemoveAt(_items.Count - 1);

    public void Swap(int first, int second)
        => (_items[first], _items[second]) = (_items[second], _items[first]);
}
