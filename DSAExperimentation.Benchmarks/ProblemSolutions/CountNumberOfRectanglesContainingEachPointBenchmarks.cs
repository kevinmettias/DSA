using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Number of Rectangles Containing Each Point (LC 2250): a brute-force
// baseline that checks every rectangle against every point directly - O(n*m) - vs.
// grouping rectangle lengths by height in this repo's own HashMap<TKey,TValue>,
// sorting each height's length list (and the distinct-height list) with this
// repo's own MergeSort, then answering each point with two nested
// BinarySearch.LowerBound calls (first to skip straight to the smallest height
// >= y, then within each surviving height bucket to skip straight to the smallest
// length >= x) - the same LowerBound-over-a-sorted-ArraySequence composition
// QueriesOnNumberOfPointsInsideACircleBenchmarks already establishes. The grouped
// approach pays a fixed one-time cost up front (building the HashMap, MergeSort-ing
// every bucket) that brute force never pays, so at RectangleCount=500 it loses to
// brute force's simple O(n*m) scan; it only wins once RectangleCount grows large
// enough that O((n+m) log n) undercuts O(n*m) by more than that fixed setup cost
// covers, which is what RectangleCount=4_000 demonstrates.
[MemoryDiagnoser]
public class CountNumberOfRectanglesContainingEachPointBenchmarks
{
    private const int RandomSeed = 2250; // LC problem number
    private const int MaxHeightExclusive = 101;
    private const int CoordinateBoundExclusive = 1_000_000_000;

    [Params(500, 4_000)]
    public int RectangleCount;

    private int[][] _rectangles = null!;
    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rectangles = Enumerable.Range(0, RectangleCount)
            .Select(_ => new[] { random.Next(1, CoordinateBoundExclusive), random.Next(1, MaxHeightExclusive) })
            .ToArray();
        _points = Enumerable.Range(0, RectangleCount)
            .Select(_ => new[] { random.Next(1, CoordinateBoundExclusive), random.Next(1, MaxHeightExclusive) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var total = 0L;

        foreach (var point in _points)
        {
            var x = point[0];
            var y = point[1];

            foreach (var rectangle in _rectangles)
            {
                if (rectangle[0] >= x && rectangle[1] >= y)
                {
                    total++;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public long GroupedSortedBinarySearch()
    {
        var (heights, lengthsByHeight) = GroupSortedLengthsByHeight();
        var heightSequence = new ArraySequence<int>(heights);
        var total = 0L;

        foreach (var point in _points)
        {
            total += CountForPoint(point, heights, heightSequence, lengthsByHeight);
        }

        return total;
    }

    private (int[] Heights, HashMap<int, int[]> LengthsByHeight) GroupSortedLengthsByHeight()
    {
        var grouped = new HashMap<int, List<int>>();

        foreach (var rectangle in _rectangles)
        {
            AddRectangleToGroup(rectangle, grouped);
        }

        var lengthsByHeight = new HashMap<int, int[]>();
        foreach (var height in grouped.Keys)
        {
            grouped.TryGetValue(height, out var lengths);
            var array = lengths.ToArray();
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(array));
            lengthsByHeight.Set(height, array);
        }

        var heights = lengthsByHeight.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(heights));

        return (heights, lengthsByHeight);
    }

    private static void AddRectangleToGroup(int[] rectangle, HashMap<int, List<int>> grouped)
    {
        var length = rectangle[0];
        var height = rectangle[1];

        if (!grouped.TryGetValue(height, out var lengths))
        {
            lengths = [];
            grouped.Set(height, lengths);
        }

        lengths.Add(length);
    }

    private static int CountForPoint(
        int[] point, int[] heights, ArraySequence<int> heightSequence, HashMap<int, int[]> lengthsByHeight)
    {
        var x = point[0];
        var y = point[1];
        var startIndex = BinarySearch.LowerBound(heightSequence, y);
        var count = 0;

        for (var i = startIndex; i < heights.Length; i++)
        {
            lengthsByHeight.TryGetValue(heights[i], out var lengths);
            var lengthSequence = new ArraySequence<int>(lengths);
            var firstAtLeastX = BinarySearch.LowerBound(lengthSequence, x);
            count += lengths.Length - firstAtLeastX;
        }

        return count;
    }
}
