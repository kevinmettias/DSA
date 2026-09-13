using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.LeetCode.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution's, the same methods
// CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceTests proves correct. Each is
// handed the already-split words its hoisted overload takes, so splitting the
// sentence is charged to [GlobalSetup] rather than to the scan being measured. The
// search word never matches, so both strategies walk every word.
[MemoryDiagnoser]
public class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks
{
    // LC problem number, reused as the deterministic sentence seed.
    private const int SentenceSeed = 1455;

    [Params(200, 5_000)]
    public int WordCount;

    private DynamicArray<string> _words = null!;

    [GlobalSetup]
    public void Setup() =>
        _words = CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SplitWords(
            PrefixSentenceWorkloads.BuildSentence(WordCount, seed: SentenceSeed));

    [Benchmark(Baseline = true)]
    public int StartsWithScan() =>
        CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByStartsWithScan(
            _words, PrefixSentenceWorkloads.UnmatchedSearchWord);

    [Benchmark]
    public int TriePerWordHasPrefix() =>
        CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByTriePerWord(
            _words, PrefixSentenceWorkloads.UnmatchedSearchWord);
}
