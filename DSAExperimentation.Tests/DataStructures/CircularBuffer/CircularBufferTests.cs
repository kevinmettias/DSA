using DSAExperimentation.DataStructures.CircularBuffer;

namespace DSAExperimentation.Tests.DataStructures.CircularBuffer;

public sealed partial class CircularBufferTests
{
    // Not a power of two, so the bound and the storage's length differ.
    private const int Capacity = 3;

    // Past the storage's first growth, so an unbounded buffer has to grow to keep accepting.
    private const int ManyElements = 20;

    [Fact]
    public void TryPushBack_Unbounded_AcceptsEveryElement()
    {
        var buffer = new CircularBuffer<int, UnboundedCapacity>(default);

        for (var i = 0; i < ManyElements; i++)
        {
            Assert.True(buffer.TryPushBack(i));
        }

        Assert.Equal(ManyElements, buffer.Count);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public void TryPushBack_BoundedAndFull_RefusesAndChangesNothing()
    {
        var buffer = FullBuffer();

        Assert.False(buffer.TryPushBack(9));
        Assert.Equal(Capacity, buffer.Count);
        Assert.True(buffer.TryPeekBack(out var back));
        Assert.Equal(Capacity - 1, back);
    }

    [Fact]
    public void TryPushFront_BoundedAndFull_RefusesAndChangesNothing()
    {
        var buffer = FullBuffer();

        Assert.False(buffer.TryPushFront(9));
        Assert.Equal(Capacity, buffer.Count);
        Assert.True(buffer.TryPeekFront(out var front));
        Assert.Equal(0, front);
    }

    [Fact]
    public void IsFull_Bounded_TurnsTrueAtTheCapacity()
    {
        var buffer = new CircularBuffer<int, BoundedCapacity>(new BoundedCapacity(Capacity));

        for (var i = 0; i < Capacity; i++)
        {
            Assert.False(buffer.IsFull);
            buffer.TryPushBack(i);
        }

        Assert.True(buffer.IsFull);
    }

    // Popping makes room, and the wrapped buffer keeps its order across the storage's end.
    [Fact]
    public void TryPushBack_AfterAPopFromAFullBuffer_AcceptsAndKeepsOrder()
    {
        var buffer = FullBuffer();
        buffer.TryPopFront(out _);

        Assert.True(buffer.TryPushBack(Capacity));
        Assert.Equal([1, 2, Capacity], Drain(buffer));
    }

    [Fact]
    public void TryPopBack_NonEmpty_RemovesTheNewest()
    {
        var buffer = FullBuffer();

        Assert.True(buffer.TryPopBack(out var back));
        Assert.Equal(Capacity - 1, back);
        Assert.Equal(Capacity - 1, buffer.Count);
    }

    [Fact]
    public void TryPeekFront_Empty_ReturnsFalse() =>
        Assert.False(new CircularBuffer<int, UnboundedCapacity>(default).TryPeekFront(out _));

    [Fact]
    public void TryPeekBack_Empty_ReturnsFalse() =>
        Assert.False(new CircularBuffer<int, UnboundedCapacity>(default).TryPeekBack(out _));

    [Fact]
    public void TryPopFront_Empty_ReturnsFalse() =>
        Assert.False(new CircularBuffer<int, UnboundedCapacity>(default).TryPopFront(out _));

    [Fact]
    public void TryPopBack_Empty_ReturnsFalse() =>
        Assert.False(new CircularBuffer<int, UnboundedCapacity>(default).TryPopBack(out _));

    [Fact]
    public void TryPushBack_ThenPeek_FrontIsTheFirstPushedAndBackTheLast()
    {
        var buffer = new CircularBuffer<int, UnboundedCapacity>(default);
        buffer.TryPushBack(1);
        buffer.TryPushBack(2);

        Assert.Equal((1, 2), EndsOf(buffer));
    }

    [Fact]
    public void TryPushFront_ThenPeek_FrontIsTheNewest()
    {
        var buffer = new CircularBuffer<int, UnboundedCapacity>(default);
        buffer.TryPushBack(1);
        buffer.TryPushFront(2);

        Assert.Equal((2, 1), EndsOf(buffer));
    }

    // Each pop moves the head on and each push lands past the end of the array, so the contents
    // straddle the array's end and every index has to wrap.
    [Fact]
    public void TryPopFrontThenTryPushBack_WrapsAroundTheArray()
    {
        var buffer = new CircularBuffer<int, UnboundedCapacity>(default);

        for (var i = 0; i < Wraparound.Filled; i++)
        {
            buffer.TryPushBack(i);
        }

        for (var i = 0; i < Wraparound.Cycled; i++)
        {
            Assert.True(buffer.TryPopFront(out var front));
            Assert.Equal(i, front);
            buffer.TryPushBack(i + Wraparound.Offset);
        }

        Assert.Equal(Wraparound.Filled, buffer.Count);
        Assert.Equal((Wraparound.Cycled, Wraparound.Cycled - 1 + Wraparound.Offset), EndsOf(buffer));
    }

    [Fact]
    public void TryPushBack_PastTheFirstGrowth_KeepsEveryElementInOrder()
    {
        var buffer = new CircularBuffer<int, UnboundedCapacity>(default);

        for (var i = 0; i < ManyElements; i++)
        {
            buffer.TryPushBack(i);
        }

        for (var i = 0; i < ManyElements; i++)
        {
            Assert.True(buffer.TryPopFront(out var front));
            Assert.Equal(i, front);
        }

        Assert.Equal(0, buffer.Count);
    }

    // Every path a wrapped index can take, held to a List model after each step: pushes at both
    // ends from a head that has already wrapped, growth while the contents straddle the end of the
    // array, and pops back down through empty. A wrong wrap shows as a wrong front or back long
    // before it shows as a wrong count. Pushes outweigh pops, so the script grows through several
    // resizes rather than hovering near empty.
    [Fact]
    public void SeededMixOfEndOperations_AgreesWithAListModelAtEveryStep()
    {
        var random = new Random(Seeded.Seed);
        var buffer = new CircularBuffer<int, UnboundedCapacity>(default);
        var model = new List<int>();

        for (var step = 0; step < Seeded.ScriptLength; step++)
        {
            ApplySeededStep(buffer, model, random, step);

            Assert.Equal(model.Count, buffer.Count);
            Assert.Equal(EndsOf(model), EndsOf(buffer));
        }
    }

    // Holds 0, 1, 2 from front to back.
    private static CircularBuffer<int, BoundedCapacity> FullBuffer()
    {
        var buffer = new CircularBuffer<int, BoundedCapacity>(new BoundedCapacity(Capacity));

        for (var i = 0; i < Capacity; i++)
        {
            buffer.TryPushBack(i);
        }

        return buffer;
    }

    private static List<int> Drain(CircularBuffer<int, BoundedCapacity> buffer)
    {
        var drained = new List<int>();

        while (buffer.TryPopFront(out var front))
        {
            drained.Add(front);
        }

        return drained;
    }

    private static void ApplySeededStep(CircularBuffer<int, UnboundedCapacity> buffer, List<int> model, Random random, int value)
    {
        var roll = random.Next(Seeded.RollRange);

        if (roll < Seeded.PushFrontBelow)
        {
            buffer.TryPushFront(value);
            model.Insert(0, value);
            return;
        }

        if (roll < Seeded.PushBackBelow || model.Count == 0)
        {
            buffer.TryPushBack(value);
            model.Add(value);
            return;
        }

        PopFromEither(buffer, model, roll);
    }

    private static void PopFromEither(CircularBuffer<int, UnboundedCapacity> buffer, List<int> model, int roll)
    {
        if (roll < Seeded.PopFrontBelow)
        {
            buffer.TryPopFront(out _);
            model.RemoveAt(0);
            return;
        }

        buffer.TryPopBack(out _);
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

    private static (int Front, int Back)? EndsOf(CircularBuffer<int, UnboundedCapacity> buffer)
    {
        if (!buffer.TryPeekFront(out var front) || !buffer.TryPeekBack(out var back))
        {
            return null;
        }

        return (front, back);
    }

    // Four elements fill the first array exactly; three pop-and-push cycles leave the head at the
    // last slot and the contents wrapped past the end. Pushed values are offset so a stale slot
    // never reads as a right answer.
    private static class Wraparound
    {
        public const int Filled = 4;
        public const int Cycled = 3;
        public const int Offset = 10;
    }

    // A roll in [0, RollRange): below PushFrontBelow pushes at the front, below PushBackBelow at
    // the back, below PopFrontBelow pops the front, and the rest pop the back - 60% pushes, 40%
    // pops.
    private static class Seeded
    {
        public const int Seed = 4_096;
        public const int ScriptLength = 3_000;
        public const int RollRange = 10;
        public const int PushFrontBelow = 3;
        public const int PushBackBelow = 6;
        public const int PopFrontBelow = 8;
    }
}
