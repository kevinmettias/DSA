using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.DataStructures.Heap;

public sealed partial class HeapTests
{
    [Fact]
    public void Push_Pop_MinHeap_ReturnsElementsInAscendingOrder()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();
        int[] values = [5, 3, 8, 1, 9, 2, 7];

        foreach (var value in values)
        {
            heap.Push(value);
        }

        var popped = PopAll(heap);

        Assert.Equal(values.OrderBy(value => value), popped);
    }

    [Fact]
    public void Push_Pop_MaxHeap_ReturnsElementsInDescendingOrder()
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();
        int[] values = [5, 3, 8, 1, 9, 2, 7];

        foreach (var value in values)
        {
            heap.Push(value);
        }

        var popped = PopAll(heap);

        Assert.Equal(values.OrderByDescending(value => value), popped);
    }

    [Fact]
    public void Push_Pop_WithDuplicatePriorities_ReturnsAllOccurrencesInHeapOrder()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();
        int[] values = [3, 1, 3, 1, 3];

        foreach (var value in values)
        {
            heap.Push(value);
        }

        var popped = PopAll(heap);

        Assert.Equal(values.OrderBy(value => value), popped);
    }

    [Fact]
    public void TryPeek_EmptyHeap_ReturnsFalse()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        Assert.False(heap.TryPeek(out _));
    }

    [Fact]
    public void TryPop_EmptyHeap_ReturnsFalse()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        Assert.False(heap.TryPop(out _));
    }

    [Fact]
    public void TryPeek_NonEmptyHeap_ReturnsTrueAndDoesNotRemoveElement()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();
        heap.Push(5);
        heap.Push(1);

        var first = heap.TryPeek(out var firstValue);
        var second = heap.TryPeek(out var secondValue);

        Assert.True(first);
        Assert.True(second);
        Assert.Equal(1, firstValue);
        Assert.Equal(1, secondValue);
        Assert.Equal(2, heap.Count);
    }

    [Fact]
    public void Count_ReflectsPushesAndPops()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        Assert.Equal(0, heap.Count);

        heap.Push(1);
        heap.Push(2);

        Assert.Equal(2, heap.Count);

        heap.TryPop(out _);

        Assert.Equal(1, heap.Count);
    }

    [Fact]
    public void Push_Pop_InterleavedOperations_MaintainsHeapOrder()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        heap.Push(5);
        heap.Push(3);
        heap.Push(8);

        heap.TryPop(out var first);

        Assert.Equal(3, first);

        heap.Push(1);
        heap.Push(9);

        var popped = PopAll(heap);

        Assert.Equal(new[] { 1, 5, 8, 9 }, popped);
    }

    private static List<Element> PopAll<Element, TOrder>(Heap<Element, TOrder> heap)
        where TOrder : struct, IHeapOrder<Element>
    {
        var popped = new List<Element>();

        while (heap.TryPop(out var item))
        {
            popped.Add(item);
        }

        return popped;
    }
}
