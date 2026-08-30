using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Median from Data Stream (LC 295): sort-on-every-query baseline (a growable
// List<int>, re-sorted from scratch on every FindMedian call, O(n log n) per query)
// vs. this repo's own two-heap approach - a MaxHeapOrder<int> heap for the smaller
// half, a MinHeapOrder<int> heap for the larger half, rebalanced after every insert
// so their roots always straddle the median (O(log n) per insert, O(1) per query).
// Both process the same interleaved AddNum/FindMedian stream.
[MemoryDiagnoser]
public class FindMedianFromDataStreamBenchmarks
{
    [Params(100, 1_000)]
    public int StreamLength;

    private int[] _stream = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(295);
        _stream = Enumerable.Range(0, StreamLength).Select(_ => random.Next(1, 1_000_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double SortOnEveryQuery()
    {
        var values = new List<int>();
        var lastMedian = 0.0;

        foreach (var value in _stream)
        {
            values.Add(value);
            var sorted = values.ToArray();
            Array.Sort(sorted);

            var mid = sorted.Length / 2;
            lastMedian = sorted.Length % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2.0 : sorted[mid];
        }

        return lastMedian;
    }

    [Benchmark]
    public double TwoHeaps()
    {
        var lowerHalf = new Heap<int, MaxHeapOrder<int>>();
        var upperHalf = new Heap<int, MinHeapOrder<int>>();
        var lastMedian = 0.0;

        foreach (var value in _stream)
        {
            if (lowerHalf.Count == 0 || value <= Peek(lowerHalf))
            {
                lowerHalf.Push(value);
            }
            else
            {
                upperHalf.Push(value);
            }

            if (lowerHalf.Count > upperHalf.Count + 1)
            {
                lowerHalf.TryPop(out var moved);
                upperHalf.Push(moved);
            }
            else if (upperHalf.Count > lowerHalf.Count)
            {
                upperHalf.TryPop(out var moved);
                lowerHalf.Push(moved);
            }

            lastMedian = lowerHalf.Count > upperHalf.Count
                ? Peek(lowerHalf)
                : (Peek(lowerHalf) + Peek(upperHalf)) / 2.0;
        }

        return lastMedian;
    }

    private static int Peek<TOrder>(Heap<int, TOrder> heap)
        where TOrder : struct, IHeapOrder<int>
    {
        heap.TryPeek(out var value);
        return value;
    }
}
