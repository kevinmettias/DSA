using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TopKFrequentWords;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TopKFrequentWordsSolution's, the same methods
// TopKFrequentWordsSolutionTests proves correct - a full sort with a tie-break comparer
// (O(d log d) over d distinct words) vs. counting into this repo's own
// HashMap<string,int> and keeping only the k "best" words in a size-k min-heap
// (O(d log k)). Length stops at LC 692's 500 words, each "word" plus a LetterNames
// suffix, so every word is lowercase letters only and at most 10 of them.
public class TopKFrequentWordsBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 7;
    private const string WordPrefix = "word";
    private const int WordPoolSize = 25;

    private string[] _words = [];

    [Params(50, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Bounded to a pool far smaller than Length so words repeat and real
        // frequency skew (with genuine ties) emerges.
        _words = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, WordPoolSize))
            .Select(index => WordPrefix + LetterNames.Of(index))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public string[] ByFullSort() => TopKFrequentWordsSolution.TopKFrequentByFullSort(_words, K);

    [Benchmark]
    public string[] ByMinHeap() => TopKFrequentWordsSolution.TopKFrequentByMinHeap(_words, K);
}
