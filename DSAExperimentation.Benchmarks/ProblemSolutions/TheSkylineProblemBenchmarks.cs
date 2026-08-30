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
            var left = random.Next(0, BuildingCount * 2);
            var width = random.Next(1, 50);
            var height = random.Next(1, 1_000);
            _buildings[i] = [left, left + width, height];
        }
    }

    [Benchmark(Baseline = true)]
    public List<int[]> BruteForceCriticalPoints()
    {
        var criticalX = new SortedSet<int>();
        foreach (var building in _buildings)
        {
            criticalX.Add(building[0]);
            criticalX.Add(building[1]);
        }

        var result = new List<int[]>();
        var previousHeight = 0;

        foreach (var x in criticalX)
        {
            var currentHeight = 0;
            foreach (var building in _buildings)
            {
                if (building[0] <= x && x < building[1] && building[2] > currentHeight)
                {
                    currentHeight = building[2];
                }
            }

            if (currentHeight != previousHeight)
            {
                result.Add([x, currentHeight]);
                previousHeight = currentHeight;
            }
        }

        return result;
    }

    [Benchmark]
    public List<int[]> SweepLineHeap()
    {
        var events = new List<(int X, int Height)>();
        foreach (var building in _buildings)
        {
            events.Add((building[0], building[2]));
            events.Add((building[1], -building[2]));
        }

        events.Sort((a, b) => a.X.CompareTo(b.X));

        var heap = new Heap<int, MaxHeapOrder<int>>();
        var pendingRemovals = new HashMap<int, int>();
        var result = new List<int[]>();
        var previousHeight = 0;
        var i = 0;

        while (i < events.Count)
        {
            var x = events[i].X;

            while (i < events.Count && events[i].X == x)
            {
                var height = events[i].Height;

                if (height > 0)
                {
                    heap.Push(height);
                }
                else
                {
                    pendingRemovals.TryGetValue(-height, out var count);
                    pendingRemovals.Set(-height, count + 1);
                }

                i++;
            }

            while (heap.TryPeek(out var top) && pendingRemovals.TryGetValue(top, out var removedCount) && removedCount > 0)
            {
                heap.TryPop(out _);
                pendingRemovals.Set(top, removedCount - 1);
            }

            var currentHeight = heap.TryPeek(out var peek) ? peek : 0;

            if (currentHeight != previousHeight)
            {
                result.Add([x, currentHeight]);
                previousHeight = currentHeight;
            }
        }

        return result;
    }
}
