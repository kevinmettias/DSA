using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The Skyline Problem (LC 218): the O(n^2) "scan every critical x-coordinate against every
// building" brute force vs. the O(n log n) sweep-line approach using this repo's own
// Heap<int,MaxHeapOrder<int>> for the active-height frontier, with lazy deletion via a
// HashMap<int,int> pending-removal count. Buildings are randomly overlapping so both
// strategies pay their full worst-case cost rather than degenerating to disjoint ranges.
[MemoryDiagnoser]
public class TheSkylineProblemBenchmarks
{
    private const int LeftCoordinateSpreadMultiplier = 2;
    private const int MaxBuildingWidth = 50;
    private const int MaxBuildingHeight = 1_000;
    private const int HeightIndex = 2;

    [Params(100, 1_000)]
    public int BuildingCount;

    private int[][] _buildings = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _buildings = new int[BuildingCount][];

        for (var i = 0; i < BuildingCount; i++)
        {
            var left = random.Next(0, BuildingCount * LeftCoordinateSpreadMultiplier);
            var width = random.Next(1, MaxBuildingWidth);
            var height = random.Next(1, MaxBuildingHeight);
            _buildings[i] = [left, left + width, height];
        }
    }

    [Benchmark(Baseline = true)]
    public List<int[]> BruteForceCriticalPoints()
    {
        var criticalX = CollectCriticalXCoordinates();
        var result = new List<int[]>();
        var previousHeight = 0;

        foreach (var x in criticalX)
        {
            var currentHeight = MaxHeightAt(x);
            previousHeight = AppendIfChanged(result, x, currentHeight, previousHeight);
        }

        return result;
    }

    private SortedSet<int> CollectCriticalXCoordinates()
    {
        var criticalX = new SortedSet<int>();

        foreach (var building in _buildings)
        {
            criticalX.Add(building[0]);
            criticalX.Add(building[1]);
        }

        return criticalX;
    }

    private int MaxHeightAt(int x)
    {
        var currentHeight = 0;

        foreach (var building in _buildings)
        {
            if (building[0] <= x && x < building[1] && building[HeightIndex] > currentHeight)
            {
                currentHeight = building[HeightIndex];
            }
        }

        return currentHeight;
    }

    private static int AppendIfChanged(List<int[]> result, int x, int currentHeight, int previousHeight)
    {
        if (currentHeight == previousHeight)
        {
            return previousHeight;
        }

        result.Add([x, currentHeight]);
        return currentHeight;
    }

    [Benchmark]
    public List<int[]> SweepLineHeap()
    {
        var events = BuildHeightEvents();
        var frontier = new ActiveHeightFrontier();
        var result = new List<int[]>();
        var cursor = (Index: 0, PreviousHeight: 0);

        while (cursor.Index < events.Count)
        {
            cursor = ProcessEventGroup(events, frontier, result, cursor);
        }

        return result;
    }

    private List<(int X, int Height)> BuildHeightEvents()
    {
        var events = new List<(int X, int Height)>();

        foreach (var building in _buildings)
        {
            events.Add((building[0], building[HeightIndex]));
            events.Add((building[1], -building[HeightIndex]));
        }

        events.Sort((a, b) => a.X.CompareTo(b.X));

        return events;
    }

    private static (int Index, int PreviousHeight) ProcessEventGroup(
        List<(int X, int Height)> events,
        ActiveHeightFrontier frontier,
        List<int[]> result,
        (int Index, int PreviousHeight) cursor)
    {
        var x = events[cursor.Index].X;
        var index = ConsumeEventsAtX(events, cursor.Index, x, frontier);

        DrainLazyDeletions(frontier);

        var currentHeight = frontier.Heap.TryPeek(out var peek) ? peek : 0;
        var previousHeight = cursor.PreviousHeight;

        if (currentHeight != previousHeight)
        {
            result.Add([x, currentHeight]);
            previousHeight = currentHeight;
        }

        return (index, previousHeight);
    }

    private static int ConsumeEventsAtX(List<(int X, int Height)> events, int index, int x, ActiveHeightFrontier frontier)
    {
        while (index < events.Count && events[index].X == x)
        {
            var height = events[index].Height;

            if (height > 0)
            {
                frontier.Heap.Push(height);
            }
            else
            {
                frontier.PendingRemovals.TryGetValue(-height, out var count);
                frontier.PendingRemovals.Set(-height, count + 1);
            }

            index++;
        }

        return index;
    }

    private static void DrainLazyDeletions(ActiveHeightFrontier frontier)
    {
        while (frontier.Heap.TryPeek(out var top) && frontier.PendingRemovals.TryGetValue(top, out var removedCount) && removedCount > 0)
        {
            frontier.Heap.TryPop(out _);
            frontier.PendingRemovals.Set(top, removedCount - 1);
        }
    }

    private sealed class ActiveHeightFrontier
    {
        public Heap<int, MaxHeapOrder<int>> Heap { get; } = new();
        public HashMap<int, int> PendingRemovals { get; } = new();
    }
}
