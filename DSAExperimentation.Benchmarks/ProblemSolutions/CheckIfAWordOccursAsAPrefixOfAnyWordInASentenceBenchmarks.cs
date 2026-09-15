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

    private DynamicArray<string> _words = new();

    [Params(200, 5_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var sentence = PrefixSentenceWorkloads.BuildSentence(WordCount, seed: SentenceSeed);
        _words = CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SplitWords(sentence);
    }

    [Benchmark(Baseline = true)]
    public int StartsWithScan() =>
        CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByStartsWithScan(
            _words, PrefixSentenceScenario.UnmatchedSearchWord);

    [Benchmark]
    public int TriePerWordHasPrefix() =>
        CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByTriePerWord(
            _words, PrefixSentenceScenario.UnmatchedSearchWord);
}
