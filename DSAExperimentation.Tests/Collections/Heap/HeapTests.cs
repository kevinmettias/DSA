using DSAExperimentation.Collections.Heap;

namespace DSAExperimentation.Tests.Collections.Heap;

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
    public void Peek_EmptyHeap_ThrowsInvalidOperationException()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        Assert.Throws<InvalidOperationException>(() => heap.Peek());
    }

    [Fact]
    public void Pop_EmptyHeap_ThrowsInvalidOperationException()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        Assert.Throws<InvalidOperationException>(() => heap.Pop());
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

        heap.Pop();

        Assert.Equal(1, heap.Count);
    }

    [Fact]
    public void Push_Pop_InterleavedOperations_MaintainsHeapOrder()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        heap.Push(5);
        heap.Push(3);
        heap.Push(8);

        Assert.Equal(3, heap.Pop());

        heap.Push(1);
        heap.Push(9);

        var popped = PopAll(heap);

        Assert.Equal(new[] { 1, 5, 8, 9 }, popped);
    }

    private static List<T> PopAll<T, TOrder>(Heap<T, TOrder> heap)
        where TOrder : struct, IHeapOrder<T>
    {
        var popped = new List<T>();

        while (heap.Count > 0)
        {
            popped.Add(heap.Pop());
        }

        return popped;
    }
}
