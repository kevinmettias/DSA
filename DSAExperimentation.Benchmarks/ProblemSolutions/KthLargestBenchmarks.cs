using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Kth Largest Element in an Array (LC 215): full O(n log n) sort-then-index vs. an
// O(n log k) size-k min-heap (this repo's own Heap<Element,MinHeapOrder<Element>>) -
// the heap only ever holds K, discarding the smaller root whenever a bigger
// candidate arrives, so its own log factor is on K, not N.
[MemoryDiagnoser]
public class KthLargestBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 7;

    [Params(1_000, 50_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullSort()
    {
        var copy = (int[])_values.Clone();
        Array.Sort(copy);
        return copy[^K];
    }

    [Benchmark]
    public int SizeKMinHeap()
    {
        var heap = new Heap<int, MinHeapOrder<int>>();

        foreach (var value in _values)
        {
            heap.Push(value);

            if (heap.Count > K)
            {
                heap.TryPop(out _);
            }
        }

        heap.TryPeek(out var kthLargest);
        return kthLargest;
    }
}
