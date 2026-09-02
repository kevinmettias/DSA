using DsaDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.DataStructures.Deque;

public sealed partial class DequeTests
{
    [Fact]
    public void TryPopFront_NonEmptyDeque_ReturnsValuesInFifoOrder()
    {
        var deque = new DsaDeque();
        deque.PushBack(1);
        deque.PushBack(2);
        deque.PushBack(3);

        deque.TryPopFront(out var first);
        deque.TryPopFront(out var second);
        deque.TryPopFront(out var third);

        Assert.Equal(1, first);
        Assert.Equal(2, second);
        Assert.Equal(3, third);
    }

    [Fact]
    public void TryPopBack_NonEmptyDeque_ReturnsValuesInLifoOrderFromTheOppositeEnd()
    {
        var deque = new DsaDeque();
        deque.PushFront(1);
        deque.PushFront(2);
        deque.PushFront(3);

        deque.TryPopBack(out var first);
        deque.TryPopBack(out var second);
        deque.TryPopBack(out var third);

        Assert.Equal(1, first);
        Assert.Equal(2, second);
        Assert.Equal(3, third);
    }

    [Fact]
    public void TryPeekFront_TryPeekBack_NonEmptyDeque_LeaveElementsInPlace()
    {
        var deque = new DsaDeque();
        deque.PushBack(1);
        deque.PushBack(2);

        var front = deque.TryPeekFront(out var frontValue);
        var back = deque.TryPeekBack(out var backValue);

        Assert.True(front);
        Assert.True(back);
        Assert.Equal(1, frontValue);
        Assert.Equal(2, backValue);
        Assert.Equal(2, deque.Count);
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

        deque.TryPopFront(out _);

        Assert.Equal(2, deque.Count);
    }

    [Fact]
    public void PushFront_MakesTheItemTheNewFront()
    {
        var deque = new DsaDeque();

        deque.PushFront(1);
        deque.PushFront(2);

        Assert.True(deque.TryPeekFront(out var front));
        Assert.Equal(2, front);
    }

    [Fact]
    public void PushBack_MakesTheItemTheNewBack()
    {
        var deque = new DsaDeque();

        deque.PushBack(1);
        deque.PushBack(2);

        Assert.True(deque.TryPeekBack(out var back));
        Assert.Equal(2, back);
    }

    [Fact]
    public void PushFront_And_PushBack_GrowFromOppositeEndsOfTheSameSequence()
    {
        var deque = new DsaDeque();

        deque.PushBack(2);
        deque.PushFront(1);
        deque.PushBack(3);

        Assert.True(deque.TryPeekFront(out var front));
        Assert.True(deque.TryPeekBack(out var back));
        Assert.Equal(1, front);
        Assert.Equal(3, back);
        Assert.Equal(3, deque.Count);
    }
}
