using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.LeetCode.MostCommonWord;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MostCommonWordSolution's, the same methods
// MostCommonWordTests proves correct - the textbook Dictionary<string,int> +
// HashSet<string> scan vs. this repo's own HashMap<string,int> plus Set<string>.
// Each arm is handed the prepared token list its hoisted overload takes, so
// tokenization is charged to [GlobalSetup] rather than to the counting scan being
// measured. Banned words are a small, fixed slice of the vocabulary so both
// strategies do real filtering work, not just counting.
[MemoryDiagnoser]
public class MostCommonWordBenchmarks
{
    // LC problem number, used as the deterministic benchmark input seed.
    private const int RandomSeed = 819;

    // Prefix shared by every generated word, e.g. "word0", "word1", ...
    private const string WordPoolPrefix = "word";

    // Bounded to a pool far smaller than Length so words repeat and a real
    // "most common" winner emerges, the same reasoning TopKFrequentWords/
    // TopKFrequentElements benchmarks already document for their own pools.
    private const int WordPoolSize = 500;

    // Number of distinct words banned from counting.
    private const int BannedWordCount = 5;

    private DynamicArray<string> _words = new();

    private string[] _banned = [];
    [Params(1_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _words = new DynamicArray<string>();

        for (var i = 0; i < Length; i++)
        {
            _words.Add(WordPoolPrefix + random.Next(0, WordPoolSize));
        }

        _banned = Enumerable.Range(0, BannedWordCount).Select(i => WordPoolPrefix + i).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string ByDictionaryScan() =>
        MostCommonWordSolution.MostCommonByDictionaryScan(_words, _banned);

    [Benchmark]
    public string ByHashMapTally() =>
        MostCommonWordSolution.MostCommonByHashMapTally(_words, _banned);
}
