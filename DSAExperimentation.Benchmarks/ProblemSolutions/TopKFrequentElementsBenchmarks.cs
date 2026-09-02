using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Top K Frequent Elements (LC 347): count with the BCL's Dictionary and sort every
// distinct value by frequency (O(d log d) over d distinct values) vs. count with
// this repo's own HashMap<int,int>, then keep only the k most frequent in a size-k
// min-heap ordered by ByPriorityOrder<TNode,TWeight> (O(d log k)) - the same
// size-k-heap shape KthLargestBenchmarks already exercises for LeetCode 215,
// applied to (value, frequency) pairs instead of bare values.
[MemoryDiagnoser]
public class TopKFrequentElementsBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 5;
    private const int ValueUpperBoundExclusive = 2_000;

    [Params(1_000, 50_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Bounded to a range far smaller than Length so values repeat and real
        // frequency skew emerges, the same reasoning TwoSumBenchmarks' bounded
        // random range documents.
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] DictionaryThenFullSort()
    {
        var counts = new Dictionary<int, int>();

        foreach (var value in _values)
        {
            counts[value] = counts.GetValueOrDefault(value) + 1;
        }

        return counts
            .OrderByDescending(entry => entry.Value)
            .Take(K)
            .Select(entry => entry.Key)
            .ToArray();
    }

    [Benchmark]
    public int[] HashMapThenSizeKMinHeap()
    {
        var counts = BuildFrequencyMap();
        var heap = BuildSizeKMinHeap(counts);

        return DrainHeapDescending(heap);
    }

    private HashMap<int, int> BuildFrequencyMap()
    {
        var counts = new HashMap<int, int>();

        foreach (var value in _values)
        {
            counts.TryGetValue(value, out var count);
            counts.Set(value, count + 1);
        }

        return counts;
    }

    private static Heap<(int Node, int Priority), ByPriorityOrder<int, int>> BuildSizeKMinHeap(HashMap<int, int> counts)
    {
        var heap = new Heap<(int Node, int Priority), ByPriorityOrder<int, int>>();

        foreach (var value in counts.Keys)
        {
            counts.TryGetValue(value, out var frequency);
            heap.Push((value, frequency));

            if (heap.Count > K)
            {
                heap.TryPop(out _);
            }
        }

        return heap;
    }

    private static int[] DrainHeapDescending(Heap<(int Node, int Priority), ByPriorityOrder<int, int>> heap)
    {
        var result = new int[heap.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            heap.TryPop(out var top);
            result[i] = top.Node;
        }

        return result;
    }
}
