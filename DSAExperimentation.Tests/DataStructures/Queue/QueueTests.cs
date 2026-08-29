using DsaQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.DataStructures.Queue;

public sealed partial class QueueTests
{
    [Fact]
    public void Enqueue_TryDequeue_ReturnsValuesInFifoOrder()
    {
        var queue = new DsaQueue();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        queue.TryDequeue(out var first);
        queue.TryDequeue(out var second);
        queue.TryDequeue(out var third);

        Assert.Equal(1, first);
        Assert.Equal(2, second);
        Assert.Equal(3, third);
    }

    [Fact]
    public void TryPeek_NonEmptyQueue_ReturnsTrueAndDoesNotRemoveElement()
    {
        var queue = new DsaQueue();
        queue.Enqueue(1);
        queue.Enqueue(2);

        var first = queue.TryPeek(out var firstValue);
        var second = queue.TryPeek(out var secondValue);

        Assert.True(first);
        Assert.True(second);
        Assert.Equal(1, firstValue);
        Assert.Equal(1, secondValue);
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void TryPeek_EmptyQueue_ReturnsFalse()
    {
        var queue = new DsaQueue();

        Assert.False(queue.TryPeek(out _));
    }

    [Fact]
    public void TryDequeue_EmptyQueue_ReturnsFalse()
    {
        var queue = new DsaQueue();

        Assert.False(queue.TryDequeue(out _));
    }

    [Fact]
    public void Count_ReflectsEnqueuesAndDequeues()
    {
        var queue = new DsaQueue();

        Assert.Equal(0, queue.Count);

        queue.Enqueue(1);
        queue.Enqueue(2);

        Assert.Equal(2, queue.Count);

        queue.TryDequeue(out _);

        Assert.Equal(1, queue.Count);
    }
}
