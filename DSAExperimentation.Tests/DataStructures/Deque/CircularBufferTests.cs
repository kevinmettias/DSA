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

    [Fact]
    public void GetFront_ReturnsTheItemAtTheHead()
    {
        var buffer = new CircularBuffer<int>();

        buffer.AddBack(1);
        buffer.AddBack(2);

        Assert.Equal(1, buffer.GetFront());
    }

    [Fact]
    public void GetBack_ReturnsTheItemAtTheTail()
    {
        var buffer = new CircularBuffer<int>();

        buffer.AddBack(1);
        buffer.AddBack(2);

        Assert.Equal(2, buffer.GetBack());
    }

    [Fact]
    public void GetFront_And_GetBack_AgreeOnASingleItem()
    {
        var buffer = new CircularBuffer<int>();

        buffer.AddBack(42);

        Assert.Equal(buffer.GetFront(), buffer.GetBack());
    }

    [Fact]
    public void GetFront_AfterAddFront_ReturnsTheNewestItem()
    {
        var buffer = new CircularBuffer<int>();

        buffer.AddBack(1);
        buffer.AddFront(0);

        Assert.Equal(0, buffer.GetFront());
        Assert.Equal(1, buffer.GetBack());
    }

    // Every path a wrapped index can take, held to a List model after each step: adds at both
    // ends from a head that has already wrapped, growth while the contents straddle the end of
    // the array, and removals back down through empty. A wrong wrap shows as a wrong front or
    // back long before it shows as a wrong count. Adds outweigh removals, so the script grows
    // through several resizes rather than hovering near empty.
    [Fact]
    public void SeededMixOfEndOperations_AgreesWithAListModelAtEveryStep()
    {
        var random = new Random(Fixtures.Seed);
        var buffer = new CircularBuffer<int>();
        var model = new List<int>();

        for (var step = 0; step < Fixtures.ScriptLength; step++)
        {
            ApplySeededStep(buffer, model, random, step);

            Assert.Equal(model.Count, buffer.Count);
            Assert.Equal(EndsOf(model), EndsOf(buffer));
        }
    }

    private static void ApplySeededStep(CircularBuffer<int> buffer, List<int> model, Random random, int value)
    {
        var roll = random.Next(Fixtures.RollRange);

        if (roll < Fixtures.AddFrontBelow)
        {
            buffer.AddFront(value);
            model.Insert(0, value);
            return;
        }

        if (roll < Fixtures.AddBackBelow || model.Count == 0)
        {
            buffer.AddBack(value);
            model.Add(value);
            return;
        }

        RemoveFromEither(buffer, model, roll);
    }

    private static void RemoveFromEither(CircularBuffer<int> buffer, List<int> model, int roll)
    {
        if (roll < Fixtures.RemoveFrontBelow)
        {
            buffer.RemoveFront();
            model.RemoveAt(0);
            return;
        }

        buffer.RemoveBack();
        model.RemoveAt(model.Count - 1);
    }

    private static (int Front, int Back)? EndsOf(List<int> model)
    {
        if (model.Count == 0)
        {
            return null;
        }

        return (model[0], model[^1]);
    }

    private static (int Front, int Back)? EndsOf(CircularBuffer<int> buffer)
    {
        if (buffer.Count == 0)
        {
            return null;
        }

        return (buffer.GetFront(), buffer.GetBack());
    }

    // A roll in [0, RollRange): below AddFrontBelow adds at the front, below AddBackBelow at
    // the back, below RemoveFrontBelow removes the front, and the rest remove the back - 60%
    // adds, 40% removals.
    private static class Fixtures
    {
        public const int Seed = 4_096;
        public const int ScriptLength = 3_000;
        public const int RollRange = 10;
        public const int AddFrontBelow = 3;
        public const int AddBackBelow = 6;
        public const int RemoveFrontBelow = 8;
    }
}
