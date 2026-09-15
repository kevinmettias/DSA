using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SortCharactersByFrequency;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortCharactersByFrequencySolution's, the same
// methods SortCharactersByFrequencyTests proves correct.
[MemoryDiagnoser]
public class SortCharactersByFrequencyBenchmarks
{
    private const int RandomSeed = 7;
    private const int AlphabetSize = 26;

    private string _text = "";

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Bounded to a 26-letter alphabet so real frequency skew emerges, the same
        // reasoning TopKFrequentElementsBenchmarks' bounded random range documents.
        _text = new string([.. Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(0, AlphabetSize)))]);
    }

    [Benchmark(Baseline = true)]
    public string DictionaryThenOrderByDescending() => SortCharactersByFrequencySolution.FrequencySortByDictionaryOrderBy(_text);

    [Benchmark]
    public string HashMapThenHeapDescendingPop() => SortCharactersByFrequencySolution.FrequencySortByHashMapHeap(_text);
}
