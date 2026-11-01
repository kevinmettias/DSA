using DsaBoundedDeque = DSAExperimentation.DataStructures.Deque.BoundedDeque<int>;

namespace DSAExperimentation.Tests.DataStructures.Deque;

public sealed partial class BoundedDequeTests
{
    private const int Capacity = 2;

    [Fact]
    public void Constructor_CapacityBelowOne_ThrowsArgumentOutOfRangeException() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new DsaBoundedDeque(0));

    [Fact]
    public void TryPushFront_Full_RefusesAndKeepsBothEnds()
    {
        var deque = FullDeque();

        Assert.False(deque.TryPushFront(9));
        Assert.True(deque.IsFull);
        AssertEnds(deque, front: 1, back: 2);
    }

    [Fact]
    public void TryPushBack_Full_RefusesAndKeepsBothEnds()
    {
        var deque = FullDeque();

        Assert.False(deque.TryPushBack(9));
        AssertEnds(deque, front: 1, back: 2);
    }

    [Fact]
    public void TryPopBack_Full_MakesRoomAtEitherEnd()
    {
        var deque = FullDeque();

        Assert.True(deque.TryPopBack(out var back));
        Assert.Equal(2, back);
        Assert.False(deque.IsFull);
        Assert.True(deque.TryPushFront(0));
        AssertEnds(deque, front: 0, back: 1);
    }

    [Fact]
    public void TryPopFront_Full_RemovesTheFront()
    {
        var deque = FullDeque();

        Assert.True(deque.TryPopFront(out var front));
        Assert.Equal(1, front);
        Assert.Equal(1, deque.Count);
    }

    [Fact]
    public void TryPeekFront_Empty_ReturnsFalse() => Assert.False(new DsaBoundedDeque(Capacity).TryPeekFront(out _));

    [Fact]
    public void TryPeekBack_Empty_ReturnsFalse() => Assert.False(new DsaBoundedDeque(Capacity).TryPeekBack(out _));

    // Holds 1 at the front and 2 at the back, pushed one at each end.
    private static DsaBoundedDeque FullDeque()
    {
        var deque = new DsaBoundedDeque(Capacity);
        deque.TryPushBack(2);
        deque.TryPushFront(1);
        return deque;
    }

    private static void AssertEnds(DsaBoundedDeque deque, int front, int back)
    {
        Assert.True(deque.TryPeekFront(out var actualFront));
        Assert.True(deque.TryPeekBack(out var actualBack));
        Assert.Equal((front, back), (actualFront, actualBack));
    }
}
