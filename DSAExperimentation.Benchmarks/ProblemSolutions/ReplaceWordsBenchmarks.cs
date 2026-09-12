using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReplaceWords;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReplaceWordsSolution's, the same methods
// ReplaceWordsTests proves correct. Half the sentence words are built by prepending
// a real dictionary root (forcing genuine prefix-match work in both strategies), the
// other half are fully random (forcing a full, unmatched dictionary scan in the
// baseline) - the same "don't let either strategy short-circuit trivially" intent
// TwoSumBenchmarks' unreachable target uses.
[MemoryDiagnoser]
public class ReplaceWordsBenchmarks
{
    private const int RootLength = 4;
    private const int WordLength = 9;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 648;

    // Alternates sentence words between "prefixed with a real dictionary root" and
    // "fully random" so half the words exercise each strategy.
    private const int AlternationModulus = 2;

    private const int LowercaseAlphabetSize = 26;

    [Params(50, 1_000)]
    public int DictionarySize;

    private string[] _dictionary = null!;
    private string _sentence = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _dictionary = Enumerable.Range(0, DictionarySize)
            .Select(_ => RandomWord(random, RootLength))
            .Distinct()
            .ToArray();
        _sentence = string.Join(' ', Enumerable.Range(0, DictionarySize)
            .Select(i => i % AlternationModulus == 0
                ? _dictionary[random.Next(_dictionary.Length)] + RandomWord(random, WordLength - RootLength)
                : RandomWord(random, WordLength)));
    }

    [Benchmark(Baseline = true)]
    public string DictionaryScanPerWord() => ReplaceWordsSolution.ReplaceByDictionaryScan(_dictionary, _sentence);

    [Benchmark]
    public string LowercaseTrieWalk() => ReplaceWordsSolution.ReplaceByTrieWalk(_dictionary, _sentence);

    private static string RandomWord(Random random, int length)
        => new(Enumerable.Range(0, length).Select(_ => (char)('a' + random.Next(LowercaseAlphabetSize))).ToArray());
}
