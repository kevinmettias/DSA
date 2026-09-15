using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.TheSkylineProblem;

// LeetCode 218. The Skyline Problem: report the contour formed by a set of
// rectangular buildings as a sequence of [x, height] key points.
//
// Both strategies reduce to the same left-to-right sweep over each building's
// start/end x-coordinate, tracking "the tallest building still active" as
// buildings enter and leave. They differ only in how that height is
// recomputed: the naive scan below re-derives it from every building at each
// critical x-coordinate, while the sweep-line strategy maintains it
// incrementally in a max-heap.
internal static class TheSkylineProblemSolution
{
    private const int HeightIndex = 2;

    // The textbook approach: collect every distinct start/end x-coordinate,
    // then for each one scan every building from scratch to find the tallest
    // one still covering it. O(n^2); written without this repo's primitives,
    // using the BCL SortedSet a caller would reach for on their own.
    public static List<int[]> GetSkylineByBruteForce(int[][] buildings)
    {
        var result = new List<int[]>();
        var previousHeight = 0;

        foreach (var x in CriticalXCoordinates(buildings))
        {
            var currentHeight = MaxHeightAt(buildings, x);

            if (currentHeight != previousHeight)
            {
                result.Add([x, currentHeight]);
                previousHeight = currentHeight;
            }
        }

        return result;
    }

    // Every distinct x-coordinate where the contour can change: a building's start and
    // its own end - the only places the brute-force scan has to re-derive the height.
    private static SortedSet<int> CriticalXCoordinates(int[][] buildings)
    {
        var criticalX = new SortedSet<int>();

        foreach (var building in buildings)
        {
            criticalX.Add(building[0]);
            criticalX.Add(building[1]);
        }

        return criticalX;
    }

    private static int MaxHeightAt(int[][] buildings, int x)
    {
        var currentHeight = 0;

        foreach (var building in buildings)
        {
            if (RaisesSkylineAt(building, x, currentHeight))
            {
                currentHeight = building[HeightIndex];
            }
        }

        return currentHeight;
    }

    // A building only raises the skyline at x while x lies inside its span and it
    // stands taller than the best height found so far.
    private static bool RaisesSkylineAt(int[] building, int x, int currentHeight)
        => building[0] <= x && x < building[1] && building[HeightIndex] > currentHeight;

    // This repo's own Heap<int, MaxHeapOrder<int>> as the active-height
    // frontier during the sweep, with lazy deletion via a
    // HashMap<int,int> pending-removal count - the standard technique since
    // Heap.cs's own doc comment says it offers no arbitrary Remove. Events
    // sharing an x-coordinate are applied as one batch before the height is
    // re-read, so intra-batch ordering never affects the result.
    public static List<int[]> GetSkylineBySweepLineHeap(int[][] buildings)
    {
        var events = BuildHeightEvents(buildings);
        var state = new SweepState();
        var i = 0;

        while (i < events.Count)
        {
            i = ProcessNextXCoordinate(events, i, state);
        }

        return state.Result;
    }

    private static List<(int X, int Height)> BuildHeightEvents(int[][] buildings)
    {
        var events = new List<(int X, int Height)>();

        foreach (var building in buildings)
        {
            events.Add((building[0], building[HeightIndex]));
            events.Add((building[1], -building[HeightIndex]));
        }

        events.Sort((a, b) => a.X.CompareTo(b.X));

        return events;
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
        while (heap.TryPeek(out var top))
        {
            var (isPending, count) = HasPendingRemoval(pendingRemovals, top);

            if (!isPending)
            {
                break;
            }

            heap.TryPop(out _);
            pendingRemovals.Set(top, count - 1);
        }
    }

    // A height still sitting at the top is only lazily dead when it is one of those
    // the sweep owes a removal to.
    private static (bool IsPending, int Count) HasPendingRemoval(HashMap<int, int> pendingRemovals, int top)
    {
        var isPending = pendingRemovals.TryGetValue(top, out var count) && count > 0;
        return (isPending, count);
    }

    private sealed class SweepState
    {
        public Heap<int, MaxHeapOrder<int>> Heap { get; } = new();

        public HashMap<int, int> PendingRemovals { get; } = new();

        public List<int[]> Result { get; } = [];

        public int PreviousHeight { get; set; }
    }
}
