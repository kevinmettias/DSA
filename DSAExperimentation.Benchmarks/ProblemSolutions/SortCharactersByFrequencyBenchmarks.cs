using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sort Characters By Frequency (LC 451): count with the BCL's Dictionary and sort
// every distinct character via OrderByDescending vs. count with this repo's own
// HashMap<char,int>, then push every (char, frequency) pair into this repo's own
// Heap<T,TOrder> ordered by ByPriorityOrder<TNode,TWeight> - the same
// (node, priority) projection TopKFrequentElementsBenchmarks uses for LC 347 -
// with the frequency negated so the heap's only order (ascending priority) pops
// characters back out in descending-frequency order for free, popped to
// completion instead of capped at k.
[MemoryDiagnoser]
public class SortCharactersByFrequencyBenchmarks
{
    private const int RandomSeed = 7;
    private const int AlphabetSize = 26;

    [Params(1_000, 50_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Bounded to a 26-letter alphabet so real frequency skew emerges, the same
        // reasoning TopKFrequentElementsBenchmarks' bounded random range documents.
        _text = new string([.. Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(0, AlphabetSize)))]);
    }

    [Benchmark(Baseline = true)]
    public string DictionaryThenOrderByDescending()
    {
        var counts = new Dictionary<char, int>();

        foreach (var c in _text)
        {
            counts[c] = counts.GetValueOrDefault(c) + 1;
        }

        var result = new StringBuilder(_text.Length);

        foreach (var entry in counts.OrderByDescending(entry => entry.Value))
        {
            result.Append(entry.Key, entry.Value);
        }

        return result.ToString();
    }

    [Benchmark]
    public string HashMapThenHeapDescendingPop()
    {
        var counts = BuildFrequencyMap();
        var heap = BuildDescendingHeap(counts);

        return DrainHeapToString(heap);
    }

    private HashMap<char, int> BuildFrequencyMap()
    {
        var counts = new HashMap<char, int>();

        foreach (var c in _text)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        return counts;
    }

    private static Heap<(char Node, int Priority), ByPriorityOrder<char, int>> BuildDescendingHeap(HashMap<char, int> counts)
    {
        var heap = new Heap<(char Node, int Priority), ByPriorityOrder<char, int>>();

        foreach (var c in counts.Keys)
        {
            counts.TryGetValue(c, out var frequency);
            heap.Push((c, -frequency));
        }

        return heap;
    }

    private string DrainHeapToString(Heap<(char Node, int Priority), ByPriorityOrder<char, int>> heap)
    {
        var result = new StringBuilder(_text.Length);

        while (heap.TryPop(out var top))
        {
            result.Append(top.Node, -top.Priority);
        }

        return result.ToString();
    }
}
