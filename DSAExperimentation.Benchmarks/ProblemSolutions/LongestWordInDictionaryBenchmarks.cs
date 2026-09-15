using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestWordInDictionary;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestWordInDictionarySolution's, the same methods
// LongestWordInDictionaryTests proves correct. Setup grows each word from the
// previous one character at a time (occasionally starting a fresh chain), the same
// "build real matches, not coincidental collisions" intent ReplaceWordsBenchmarks'
// half-real-root generator uses - which guarantees most words are genuinely
// buildable, forcing both strategies through their full prefix-chain walk instead
// of an early mismatch.
[MemoryDiagnoser]
public class LongestWordInDictionaryBenchmarks
{
    // LC problem number, used as the deterministic setup seed.
    private const int RandomSeed = 720;
    private const int AlphabetSize = 26;
    private const int FreshChainChance = 4;
    private const string EmptyPrefix = "";

    private string[] _words = [];

    [Params(50, 500)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var words = new List<string>(WordCount);
        var current = EmptyPrefix;

        while (words.Count < WordCount)
        {
            var startsFreshChain = current.Length == 0 || random.Next(FreshChainChance) == 0;

            current = startsFreshChain
                ? ((char)('a' + random.Next(AlphabetSize))).ToString()
                : AppendedRandomLetter(current, random);

            words.Add(current);
        }

        _words = words.ToArray();
    }

    private static string AppendedRandomLetter(string current, Random random)
        => current + (char)('a' + random.Next(AlphabetSize));

    [Benchmark(Baseline = true)]
    public string DictionaryScanPerPrefix() =>
        LongestWordInDictionarySolution.LongestWordByDictionaryScan(_words);

    [Benchmark]
    public string LowercaseTrieWalk() =>
        LongestWordInDictionarySolution.LongestWordByLowercaseTrieWalk(_words);
}
