using DsaBoundedQueue = DSAExperimentation.DataStructures.Queue.BoundedQueue<int>;

namespace DSAExperimentation.Tests.DataStructures.Queue;

public sealed partial class BoundedQueueTests
{
    private const int Capacity = 2;

    [Fact]
    public void Constructor_CapacityBelowOne_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new DsaBoundedQueue(0));

    [Fact]
    public void TryEnqueue_Full_RefusesAndKeepsTheRear()
    {
        var queue = FullQueue();

        Assert.True(queue.IsFull);
        Assert.False(queue.TryEnqueue(9));
        Assert.True(queue.TryPeekRear(out var rear));
        Assert.Equal(2, rear);
    }

    [Fact]
    public void TryDequeue_Full_ReturnsTheOldestAndMakesRoom()
    {
        var queue = FullQueue();

        Assert.True(queue.TryDequeue(out var front));
        Assert.Equal(1, front);
        Assert.False(queue.IsFull);
        Assert.True(queue.TryEnqueue(3));
        Assert.True(queue.TryPeek(out var oldest));
        Assert.Equal(2, oldest);
    }

    [Fact]
    public void TryPeek_Empty_ReturnsFalse() => Assert.False(new DsaBoundedQueue(Capacity).TryPeek(out _));

    [Fact]
    public void TryPeekRear_Empty_ReturnsFalse() => Assert.False(new DsaBoundedQueue(Capacity).TryPeekRear(out _));

    [Fact]
    public void TryDequeue_Empty_ReturnsFalse() => Assert.False(new DsaBoundedQueue(Capacity).TryDequeue(out _));

    [Fact]
    public void Count_ReflectsEnqueuesAndDequeues()
    {
        var queue = FullQueue();
        queue.TryDequeue(out _);

        Assert.Equal(1, queue.Count);
    }

    // Holds 1 then 2.
    private static DsaBoundedQueue FullQueue()
    {
        var queue = new DsaBoundedQueue(Capacity);
        queue.TryEnqueue(1);
        queue.TryEnqueue(2);
        return queue;
    }
}
