using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Kth Largest Element in a Stream (LC 703): a sort-on-every-add baseline (a growable
// List<int>, re-sorted from scratch on every Add call, O(n log n) per call) vs this
// repo's own size-k min-heap (Heap<int,MinHeapOrder<int>>, discarding the smallest
// root whenever the heap grows past k, O(log k) per call) - the
// FindMedianFromDataStream two-heap benchmark's "process the same interleaved
// stream, compare per-call cost" shape, specialized to LC703's single running order
// statistic.
[MemoryDiagnoser]
public class KthLargestElementInAStreamBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 703; // LC problem number
    private const int StreamValueExclusiveBound = 1_000_000;

    [Params(100, 1_000)]
    public int StreamLength;

    private int[] _stream = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stream = Enumerable.Range(0, StreamLength).Select(_ => random.Next(1, StreamValueExclusiveBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SortOnEveryAdd()
    {
        var values = new List<int>();
        var lastKthLargest = 0;

        foreach (var value in _stream)
        {
            values.Add(value);
            var sorted = values.ToArray();
            Array.Sort(sorted);
            lastKthLargest = sorted[Math.Max(0, sorted.Length - K)];
        }

        return lastKthLargest;
    }

    [Benchmark]
    public int SizeKMinHeap()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();
        var lastKthLargest = 0;

        foreach (var value in _stream)
        {
            heap.Push(value);

            if (heap.Count > K)
            {
                heap.TryPop(out _);
            }

            heap.TryPeek(out lastKthLargest);
        }

        return lastKthLargest;
    }
}
