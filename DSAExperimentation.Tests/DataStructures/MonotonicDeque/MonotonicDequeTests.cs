using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.Tests.DataStructures.MonotonicDeque;

// SlidingMaxima is the deque's whole contract in one loop - push, evict what slid out, read the front -
// checked against a brute-force maximum of every window, so the cases below only pin the individual
// members' edges.
public sealed partial class MonotonicDequeTests
{
    public static TheoryData<int[], int> SlidingWindows =>
        new()
        {
            { [1, 3, -1, -3, 5, 3, 6, 7], 3 },
            { [4, 4, 4, 1, 4], 2 },
            { [9, 8, 7, 6, 5, 4], 4 },
        };

    [Fact]
    public void TryPeekFront_Empty_ReturnsFalse() =>
        Assert.False(new MonotonicDeque<int, MaxWindowOrder<int>>().TryPeekFront(out _));

    [Fact]
    public void Push_SmallerKeyAfterLarger_KeepsTheLargerAtTheFront()
    {
        var deque = new MonotonicDeque<int, MaxWindowOrder<int>>();

        deque.Push(0, 9);
        deque.Push(1, 4);

        Assert.True(deque.TryPeekFront(out var front));
        Assert.Equal((0, 9), front);
    }

    [Fact]
    public void Push_LargerKey_EvictsEveryDominatedResident()
    {
        var deque = new MonotonicDeque<int, MaxWindowOrder<int>>();

        deque.Push(0, 3);
        deque.Push(1, 1);
        deque.Push(2, 5);

        Assert.True(deque.TryPopFront(out var front));
        Assert.Equal((2, 5), front);
        Assert.False(deque.TryPeekFront(out _));
    }

    // Ties are dominated: the later of two equal keys stays in the window longer, so it is the one kept.
    [Fact]
    public void Push_EqualKey_EvictsTheOlderEntry()
    {
        var deque = new MonotonicDeque<int, MaxWindowOrder<int>>();

        deque.Push(0, 7);
        deque.Push(1, 7);

        Assert.True(deque.TryPopFront(out var front));
        Assert.Equal(1, front.Position);
        Assert.False(deque.TryPeekFront(out _));
    }

    [Fact]
    public void EvictBefore_DropsOnlyPositionsBelowTheWindow()
    {
        var deque = new MonotonicDeque<int, MinWindowOrder<int>>();

        deque.Push(0, 1);
        deque.Push(1, 2);
        deque.Push(2, 3);
        deque.EvictBefore(1);

        Assert.True(deque.TryPeekFront(out var front));
        Assert.Equal((1, 2), front);
    }

    // A position needn't be an index: with x-coordinates as positions, a window of width 4 ending at
    // x = 10 starts at 6, and the key stored at push time is what the front reports.
    [Fact]
    public void EvictBefore_CoordinatePositions_KeepsTheInWindowExtremum()
    {
        var deque = new MonotonicDeque<long, MaxWindowOrder<long>>();

        deque.Push(1, 50L);
        deque.Push(6, 20L);
        deque.Push(10, 10L);
        deque.EvictBefore(10 - 4);

        Assert.True(deque.TryPeekFront(out var front));
        Assert.Equal((6, 20L), front);
    }

    [Fact]
    public void TryPopFront_Empty_ReturnsFalse() =>
        Assert.False(new MonotonicDeque<int, MinWindowOrder<int>>().TryPopFront(out _));

    [Theory]
    [MemberData(nameof(SlidingWindows))]
    public void Push_SlidingWindow_FrontIsEveryWindowsMaximum(int[] values, int width) =>
        Assert.Equal(BruteForceMaxima(values, width), SlidingMaxima(values, width));

    private static int[] SlidingMaxima(int[] values, int width)
    {
        var deque = new MonotonicDeque<int, MaxWindowOrder<int>>();
        var maxima = new List<int>();

        for (var position = 0; position < values.Length; position++)
        {
            deque.Push(position, values[position]);
            deque.EvictBefore(position - width + 1);

            if (position >= width - 1 && deque.TryPeekFront(out var front))
            {
                maxima.Add(front.Key);
            }
        }

        return [.. maxima];
    }

    private static int[] BruteForceMaxima(int[] values, int width) =>
        [.. Enumerable.Range(0, values.Length - width + 1).Select(start => values.Skip(start).Take(width).Max())];
}
