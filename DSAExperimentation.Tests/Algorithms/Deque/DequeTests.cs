using DsaDeque = DSAExperimentation.Algorithms.Deque.Deque<int>;

namespace DSAExperimentation.Tests.Algorithms.Deque;

public sealed partial class DequeTests
{
    [Fact]
    public void PushBack_PopFront_ReturnsValuesInFifoOrder()
    {
        var deque = new DsaDeque();
        deque.PushBack(1);
        deque.PushBack(2);
        deque.PushBack(3);

        Assert.Equal(1, deque.PopFront());
        Assert.Equal(2, deque.PopFront());
        Assert.Equal(3, deque.PopFront());
    }

    [Fact]
    public void PushFront_PopBack_ReturnsValuesInLifoOrderFromTheOppositeEnd()
    {
        var deque = new DsaDeque();
        deque.PushFront(1);
        deque.PushFront(2);
        deque.PushFront(3);

        Assert.Equal(1, deque.PopBack());
        Assert.Equal(2, deque.PopBack());
        Assert.Equal(3, deque.PopBack());
    }

    [Fact]
    public void PeekFront_PeekBack_DoNotRemoveElements()
    {
        var deque = new DsaDeque();
        deque.PushBack(1);
        deque.PushBack(2);

        Assert.Equal(1, deque.PeekFront());
        Assert.Equal(2, deque.PeekBack());
        Assert.Equal(2, deque.Count);
    }

    [Fact]
    public void PeekFront_EmptyDeque_ThrowsInvalidOperationException()
    {
        var deque = new DsaDeque();

        Assert.Throws<InvalidOperationException>(() => deque.PeekFront());
    }

    [Fact]
    public void PeekBack_EmptyDeque_ThrowsInvalidOperationException()
    {
        var deque = new DsaDeque();

        Assert.Throws<InvalidOperationException>(() => deque.PeekBack());
    }

    [Fact]
    public void TryPeekFront_EmptyDeque_ReturnsFalse()
    {
        var deque = new DsaDeque();

        Assert.False(deque.TryPeekFront(out _));
    }

    [Fact]
    public void TryPeekBack_EmptyDeque_ReturnsFalse()
    {
        var deque = new DsaDeque();

        Assert.False(deque.TryPeekBack(out _));
    }

    [Fact]
    public void TryPopFront_EmptyDeque_ReturnsFalse()
    {
        var deque = new DsaDeque();

        Assert.False(deque.TryPopFront(out _));
    }

    [Fact]
    public void TryPopBack_EmptyDeque_ReturnsFalse()
    {
        var deque = new DsaDeque();

        Assert.False(deque.TryPopBack(out _));
    }

    [Fact]
    public void Count_ReflectsMixedEndOperations()
    {
        var deque = new DsaDeque();

        deque.PushBack(1);
        deque.PushFront(2);
        deque.PushBack(3);

        Assert.Equal(3, deque.Count);

        deque.PopFront();

        Assert.Equal(2, deque.Count);
    }
}
