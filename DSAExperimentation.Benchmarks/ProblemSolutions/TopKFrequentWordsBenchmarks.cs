using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TopKFrequentWords;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TopKFrequentWordsSolution's, the same methods
// TopKFrequentWordsTests proves correct - a full sort with a tie-break comparer
// (O(d log d) over d distinct words) vs. counting into this repo's own
// HashMap<string,int> and keeping only the k "best" words in a size-k min-heap
// (O(d log k)).
[MemoryDiagnoser]
public class TopKFrequentWordsBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 7;
    private const string WordPrefix = "word";
    private const int WordPoolSize = 500;

    private string[] _words = [];

    [Params(1_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Bounded to a pool far smaller than Length so words repeat and real
        // frequency skew (with genuine ties) emerges.
        _words = Enumerable.Range(0, Length).Select(_ => WordPrefix + random.Next(0, WordPoolSize)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string[] ByFullSort() => TopKFrequentWordsSolution.TopKFrequentByFullSort(_words, K);

    [Benchmark]
    public string[] ByMinHeap() => TopKFrequentWordsSolution.TopKFrequentByMinHeap(_words, K);
}
