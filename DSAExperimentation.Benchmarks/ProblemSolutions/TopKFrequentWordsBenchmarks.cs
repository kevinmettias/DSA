using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Top K Frequent Words (LC 692): count + full sort with a tie-break comparer
// (Dictionary + OrderByDescending/ThenBy, O(d log d) over d distinct words) vs.
// count with this repo's own HashMap<string,int>, then keep only the k "best"
// words in a size-k min-heap (O(d log k)) - the same size-k-heap shape
// TopKFrequentElementsBenchmarks already exercises for LeetCode 347, closed over
// MinHeapOrder<WordPriority> instead of ByPriorityOrder<TNode,TWeight> so a single
// IComparable key can carry both the frequency-descending primary order and the
// word-ascending tie-break LeetCode 692 requires.
[MemoryDiagnoser]
public class TopKFrequentWordsBenchmarks
{
    private const int K = 10;

    [Params(1_000, 20_000)]
    public int Length;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);

        // Bounded to a pool far smaller than Length so words repeat and real
        // frequency skew (with genuine ties) emerges, the same reasoning
        // TopKFrequentElementsBenchmarks' bounded random range documents.
        _words = Enumerable.Range(0, Length).Select(_ => "word" + random.Next(0, 500)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string[] DictionaryThenFullSort()
    {
        var counts = new Dictionary<string, int>();

        foreach (var word in _words)
        {
            counts[word] = counts.GetValueOrDefault(word) + 1;
        }

        return counts
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => entry.Key, StringComparer.Ordinal)
            .Take(K)
            .Select(entry => entry.Key)
            .ToArray();
    }

    [Benchmark]
    public string[] HashMapThenSizeKMinHeap()
    {
        var counts = new HashMap<string, int>();

        foreach (var word in _words)
        {
            counts.TryGetValue(word, out var count);
            counts.Set(word, count + 1);
        }

        var heap = new Heap<WordPriority, MinHeapOrder<WordPriority>>();

        foreach (var word in counts.Keys)
        {
            counts.TryGetValue(word, out var frequency);
            heap.Push(new WordPriority(frequency, word));

            if (heap.Count > K)
            {
                heap.TryPop(out _);
            }
        }

        var result = new string[heap.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            heap.TryPop(out var top);
            result[i] = top.Word;
        }

        return result;
    }

    private readonly record struct WordPriority(int Frequency, string Word) : IComparable<WordPriority>
    {
        public int CompareTo(WordPriority other) => Frequency != other.Frequency
            ? Frequency.CompareTo(other.Frequency)
            : string.CompareOrdinal(other.Word, Word);
    }
}
