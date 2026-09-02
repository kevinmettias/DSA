using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// K Closest Points to Origin (LC 973): a full O(n log n) sort of every point's
// squared distance vs. an O(n log k) size-k max-heap (this repo's own
// Heap<Element,MaxHeapOrder<Element>> - KthLargestBenchmarks precedent) that only
// ever holds k candidates, discarding the farthest whenever a closer point arrives.
[MemoryDiagnoser]
public class KClosestPointsToOriginBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 973; // LC problem number
    private const int CoordinateBound = 10_000;

    [Params(1_000, 50_000)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long FullSort()
    {
        var withDistance = _points
            .Select(p => ((long)(p[0] * p[0]) + (p[1] * p[1]), p))
            .ToArray();

        Array.Sort(withDistance, (a, b) => a.Item1.CompareTo(b.Item1));

        var sum = 0L;
        for (var i = 0; i < K; i++)
        {
            sum += withDistance[i].Item1;
        }

        return sum;
    }

    [Benchmark]
    public long SizeKMaxHeap()
    {
        var heap = new Heap<(int Distance, int X, int Y), MaxHeapOrder<(int, int, int)>>();

        foreach (var point in _points)
        {
            var distance = (point[0] * point[0]) + (point[1] * point[1]);
            heap.Push((distance, point[0], point[1]));

            if (heap.Count > K)
            {
                heap.TryPop(out _);
            }
        }

        var sum = 0L;
        while (heap.TryPop(out var top))
        {
            sum += top.Distance;
        }

        return sum;
    }
}
