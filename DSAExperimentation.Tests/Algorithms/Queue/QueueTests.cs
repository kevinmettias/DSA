using DsaQueue = DSAExperimentation.Algorithms.Queue.Queue<int>;

namespace DSAExperimentation.Tests.Algorithms.Queue;

public sealed partial class QueueTests
{
    [Fact]
    public void Enqueue_Dequeue_ReturnsValuesInFifoOrder()
    {
        var queue = new DsaQueue();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        Assert.Equal(1, queue.Dequeue());
        Assert.Equal(2, queue.Dequeue());
        Assert.Equal(3, queue.Dequeue());
    }

    [Fact]
    public void Peek_DoesNotRemoveElement()
    {
        var queue = new DsaQueue();
        queue.Enqueue(1);
        queue.Enqueue(2);

        Assert.Equal(1, queue.Peek());
        Assert.Equal(1, queue.Peek());
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void Peek_EmptyQueue_ThrowsInvalidOperationException()
    {
        var queue = new DsaQueue();

        Assert.Throws<InvalidOperationException>(() => queue.Peek());
    }

    [Fact]
    public void Dequeue_EmptyQueue_ThrowsInvalidOperationException()
    {
        var queue = new DsaQueue();

        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
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

        queue.Dequeue();

        Assert.Equal(1, queue.Count);
    }
}
