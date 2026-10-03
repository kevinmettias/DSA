using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.LeetCode.MostCommonWord;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MostCommonWordSolution's, the same methods
// MostCommonWordSolutionTests proves correct - the textbook Dictionary<string,int> +
// HashSet<string> scan vs. this repo's own HashMap<string,int> plus Set<string>.
// Each arm is handed the prepared token list its hoisted overload takes, so
// tokenization is charged to [GlobalSetup] rather than to the counting scan being
// measured. Banned words are a small, fixed slice of the vocabulary so both
// strategies do real filtering work, not just counting.
//
// Length counts tokens, and stops at 166: MostCommonWordWorkloads' five-letter words
// joined by single spaces then fill 995 characters of LC 819's 1,000-character
// paragraph, and its planted leader keeps the answer unique as LC 819 guarantees.
public class MostCommonWordBenchmarks
{
    // LC problem number, used as the deterministic benchmark input seed.
    private const int RandomSeed = 819;

    // Bounded to a pool far smaller than Length so words repeat and a real
    // "most common" winner emerges, the same reasoning TopKFrequentWords/
    // TopKFrequentElements benchmarks already document for their own pools.
    private const int WordPoolSize = 20;

    // Number of distinct words banned from counting.
    private const int BannedWordCount = 5;

    private DynamicArray<string> _words = new();

    private string[] _banned = [];
    [Params(16, 166)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (tokens, banned) = MostCommonWordWorkloads.Build(Length, WordPoolSize, BannedWordCount, RandomSeed);

        _words = new DynamicArray<string>();

        foreach (var token in tokens)
        {
            _words.Add(token);
        }

        _banned = banned;
    }

    [Benchmark(Baseline = true)]
    public string ByDictionaryScan() =>
        MostCommonWordSolution.MostCommonByDictionaryScan(_words, _banned);

    [Benchmark]
    public string ByHashMapTally() =>
        MostCommonWordSolution.MostCommonByHashMapTally(_words, _banned);
}
