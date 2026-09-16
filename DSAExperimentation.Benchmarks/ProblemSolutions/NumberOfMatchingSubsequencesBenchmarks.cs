using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfMatchingSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfMatchingSubsequencesSolution's, the same
// methods NumberOfMatchingSubsequencesTests proves correct - the natural
// two-pointer subsequence scan through searchedText, once per word, against this
// repo's HashMap<TKey,TValue> + Queue<T> bucket pass, which advances every word in a
// single left-to-right pass over searchedText. The workload is built so no word ever
// matches, forcing the per-word arm through its full worst case (see
// MatchingSubsequenceWorkloads). Both arms take LeetCode's own
// (searchedText, words) shape, so [GlobalSetup] only has to generate the input, not
// prepare a structure.
[MemoryDiagnoser]
public class NumberOfMatchingSubsequencesBenchmarks
{
    // Arbitrary fixed seed for reproducible benchmark input.
    private const int RandomSeed = 1;

    private string _searchedText = "";

    private string[] _words = [];
    [Params(200, 2_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _searchedText = MatchingSubsequenceWorkloads.BuildText(random, MatchingSubsequenceScenario.TextLength);
        _words = MatchingSubsequenceWorkloads.BuildUnmatchableWords(random, WordCount);
    }

    [Benchmark(Baseline = true)]
    public int TwoPointerPerWord() =>
        NumberOfMatchingSubsequencesSolution.CountMatchingSubseqByTwoPointerPerWord(_searchedText, _words);

    [Benchmark]
    public int HashMapQueueBuckets() =>
        NumberOfMatchingSubsequencesSolution.CountMatchingSubseqByWait