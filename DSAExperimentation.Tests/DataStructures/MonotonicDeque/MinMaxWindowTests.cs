using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.Tests.DataStructures.MonotonicDeque;

// The seeded sweep is the window's whole contract - push, evict what slid out, read the range - held to
// a brute-force minimum and maximum of what is still in the window after every step, so the cases above
// it only pin the members' edges.
public sealed partial class MinMaxWindowTests
{
    [Fact]
    public void TryPeekRange_Empty_ReturnsFalse() =>
        Assert.False(new MinMaxWindow<int>().TryPeekRange(out _, out _));

    [Fact]
    public void Push_OneKey_IsBothTheMinimumAndTheMaximum()
    {
        var window = new MinMaxWindow<int>();

        window.Push(0, 7);

        Assert.True(window.TryPeekRange(out var minimum, out var maximum));
        Assert.Equal((7, 7), (minimum, maximum));
    }

    [Fact]
    public void EvictBefore_DropsOnlyPositionsBelowTheWindow()
    {
        var window = new MinMaxWindow<int>();

        window.Push(0, 1);
        window.Push(1, 9);
        window.Push(2, 5);
        window.EvictBefore(1);

        Assert.True(window.TryPeekRange(out var minimum, out var maximum));
        Assert.Equal((5, 9), (minimum, maximum));
    }

    // Both deques hold the newest position, so evicting past it empties them together, and the range
    // reads as empty rather than as one side's stale front.
    [Fact]
    public void EvictBefore_PastTheNewestPosition_EmptiesTheWindow()
    {
        var window = new MinMaxWindow<int>();

        window.Push(0, 3);
        window.Push(1, 8);
        window.EvictBefore(2);

        Assert.False(window.TryPeekRange(out _, out _));
    }

    [Fact]
    public void TryPeekRange_SeededPushesAndEvictions_AgreesWithABruteForceRangeAtEveryStep()
    {
        var random = new Random(Seeded.Seed);
        var window = new MinMaxWindow<int>();
        var model = new List<(int Position, int Key)>();
        var firstPosition = 0;

        for (var position = 0; position < Seeded.ScriptLength; position++)
        {
            var key = random.Next(Seeded.KeyRange);
            window.Push(position, key);
            model.Add((position, key));

            firstPosition = Math.Min(position + 1, firstPosition + random.Next(Seeded.StrideRange));
            window.EvictBefore(firstPosition);

            Assert.Equal(RangeOf(model, firstPosition), RangeOf(window));
        }
    }

    private static (int Minimum, int Maximum)? RangeOf(List<(int Position, int Key)> model, int firstPosition)
    {
        var keys = model.Where(entry => entry.Position >= firstPosition).Select(entry => entry.Key).ToList();

        if (keys.Count == 0)
        {
            return null;
        }

        return (keys.Min(), keys.Max());
    }

    private static (int Minimum, int Maximum)? RangeOf(MinMaxWindow<int> window)
    {
        if (!window.TryPeekRange(out var minimum, out var maximum))
        {
            return null;
        }

        return (minimum, maximum);
    }

    // Keys come from a small range, so equal keys - which each deque evicts as dominated - are common.
    // The left edge moves 0, 1 or 2 positions per push, one on average, and never past the newest
    // position plus one, so the window empties, refills and grows over the script.
    private static class Seeded
    {
        public const int Seed = 1_438; // LC 1438, the first solution to keep this pair
        public const int ScriptLength = 3_000;
        public const int KeyRange = 20;
        public const int StrideRange = 3;
    }
}
