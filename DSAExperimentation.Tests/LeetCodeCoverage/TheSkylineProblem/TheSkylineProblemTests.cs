using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheSkylineProblem;

// LeetCode 218. The Skyline Problem: a left-to-right sweep over each building's start/end
// x-coordinate, tracking active heights in this repo's own Heap<int,MaxHeapOrder<int>> with
// lazy deletion via a HashMap<int,int> pending-removal count - the standard technique for a
// heap that, per Heap.cs's own doc comment, offers no arbitrary Remove. Events sharing an
// x-coordinate are applied as one batch before the height is re-read, so intra-batch ordering
// never affects the result (verified by the third test below, where an ending and a starting
// building of equal height share an x-coordinate).
public sealed partial class TheSkylineProblemTests
{
    [Fact]
    public void GetSkyline_FiveOverlappingBuildings_ReturnsClassicKeyPoints()
    {
        int[][] buildings =
        [
            [2, 9, 10],
            [3, 7, 15],
            [5, 12, 12],
            [15, 20, 10],
            [19, 24, 8],
        ];

        var skyline = GetSkyline(buildings);

        Assert.Equal(
            [[2, 10], [3, 15], [7, 12], [12, 0], [15, 10], [20, 8], [24, 0]],
            skyline);
    }

    [Fact]
    public void GetSkyline_SingleBuilding_ReturnsRiseThenFall()
    {
        int[][] buildings = [[0, 2, 3]];

        var skyline = GetSkyline(buildings);

        Assert.Equal([[0, 3], [2, 0]], skyline);
    }

    [Fact]
    public void GetSkyline_TwoAdjacentBuildingsOfEqualHeight_MergesIntoOneRun()
    {
        int[][] buildings = [[1, 5, 4], [5, 10, 4]];

        var skyline = GetSkyline(buildings);

        Assert.Equal([[1, 4], [10, 0]], skyline);
    }

    private static List<int[]> GetSkyline(int[][] buildings)
    {
        var events = new List<(int X, int Height)>();
        foreach (var building in buildings)
        {
            events.Add((building[0], building[2]));
            events.Add((building[1], -building[2]));
        }

        events.Sort((a, b) => a.X.CompareTo(b.X));

        var state = new SweepState();
        var i = 0;

        while (i < events.Count)
        {
            i = ProcessNextXCoordinate(events, i, state);
        }

        return state.Result;
    }

    private static int ProcessNextXCoordinate(List<(int X, int Height)> events, int i, SweepState state)
    {
        var x = events[i].X;

        while (i < events.Count && events[i].X == x)
        {
            ApplyEvent(events[i].Height, state.Heap, state.PendingRemovals);
            i++;
        }

        DiscardRemovedTops(state.Heap, state.PendingRemovals);
        var currentHeight = state.Heap.TryPeek(out var top) ? top : 0;

        if (currentHeight != state.PreviousHeight)
        {
            state.Result.Add([x, currentHeight]);
            state.PreviousHeight = currentHeight;
        }

        return i;
    }

    private static void ApplyEvent(int height, Heap<int, MaxHeapOrder<int>> heap, HashMap<int, int> pendingRemovals)
    {
        if (height > 0)
        {
            heap.Push(height);
            return;
        }

        var endingHeight = -height;
        pendingRemovals.TryGetValue(endingHeight, out var count);
        pendingRemovals.Set(endingHeight, count + 1);
    }

    private static void DiscardRemovedTops(Heap<int, MaxHeapOrder<int>> heap, HashMap<int, int> pendingRemovals)
    {
        while (heap.TryPeek(out var top) && pendingRemovals.TryGetValue(top, out var count) && count > 0)
        {
            heap.TryPop(out _);
            pendingRemovals.Set(top, count - 1);
        }
    }

    private sealed class SweepState
    {
        public Heap<int, MaxHeapOrder<int>> Heap { get; } = new();

        public HashMap<int, int> PendingRemovals { get; } = new();

        public List<int[]> Result { get; } = [];

        public int PreviousHeight { get; set; }
    }
}
