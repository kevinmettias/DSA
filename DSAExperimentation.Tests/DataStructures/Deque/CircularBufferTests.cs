using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.Tests.DataStructures.Deque;

public sealed partial class CircularBufferTests
{
    [Fact]
    public void AddBack_ThenGetFront_ReturnsFirstAddedValue()
    {
        var buffer = new CircularBuffer<int>();
        buffer.AddBack(1);
        buffer.AddBack(2);

        Assert.Equal(1, buffer.GetFront());
        Assert.Equal(2, buffer.GetBack());
    }

    [Fact]
    public void AddFront_ThenGetFront_ReturnsMostRecentlyAddedValue()
    {
        var buffer = new CircularBuffer<int>();
        buffer.AddBack(1);
        buffer.AddFront(2);

        Assert.Equal(2, buffer.GetFront());
        Assert.Equal(1, buffer.GetBack());
    }

    [Fact]
    public void RemoveFrontThenAddBack_WrapsAroundBufferCorrectly()
    {
        var buffer = new CircularBuffer<int>();

        for (var i = 0; i < 4; i++)
        {
            buffer.AddBack(i);
        }

        for (var i = 0; i < 3; i++)
        {
            Assert.Equal(i, buffer.GetFront());
            buffer.RemoveFront();
            buffer.AddBack(i + 10);
        }

        Assert.Equal(4, buffer.Count);
        Assert.Equal(3, buffer.GetFront());
        Assert.Equal(12, buffer.GetBack());
    }

    [Fact]
    public void AddBack_BeyondInitialCapacity_GrowsAndPreservesOrder()
    {
        var buffer = new CircularBuffer<int>();

        for (var i = 0; i < 20; i++)
        {
            buffer.AddBack(i);
        }

        Assert.Equal(20, buffer.Count);

        for (var i = 0; i < 20; i++)
        {
            Assert.Equal(i, buffer.GetFront());
            buffer.RemoveFront();
        }

        Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public void RemoveBack_DecreasesCountAndUpdatesBack()
    {
        var buffer = new CircularBuffer<int>();
        buffer.AddBack(1);
        buffer.AddBack(2);

        buffer.RemoveBack();

        Assert.Equal(1, buffer.Count);
        Assert.Equal(1, buffer.GetBack());
    }
}
