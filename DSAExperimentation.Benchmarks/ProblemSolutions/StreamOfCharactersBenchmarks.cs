using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.StreamOfCharacters.StreamOfCharactersSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StreamOfCharactersSolution's, the same classes
// StreamOfCharactersTests proves correct - re-testing every suffix of the whole
// stream so far against a HashSet<string> on every query (O(stream length * max word
// length) per query, since each candidate substring itself costs O(length) to
// materialize and hash) against this repo's own LowercaseTrie<TValue> built from
// reversed words and walked backward through DynamicArray<char>'s O(1) indexed Get
// (O(max word length) per query). [GlobalSetup] generates the streamed characters,
// so stream generation is charged to setup rather than to the replay each arm
// measures. Words are chosen with no shared suffixes so neither approach gets an
// early exit from an early match, forcing both through their full per-query cost on
// nearly every character.
[MemoryDiagnoser]
public class StreamOfCharactersBenchmarks
{
    private static readonly string[] Words =
    [
        "characters", "algorithm", "benchmark", "primitive", "structure",
        "traversal", "reference", "composed", "sequence", "children",
    ];

    private const int RandomSeed = 1032; // LC problem number
    private const int AlphabetSize = 26;

    [Params(200, 3_000)]
    public int StreamLength;

    private char[] _stream = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stream = Enumerable.Range(0, StreamLength).Select(_ => (char)('a' + random.Next(0, AlphabetSize))).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanEverySuffixAgainstHashSet() => CountMatches(new StreamCheckerBySuffixRescan(Words));

    [Benchmark]
    public int ReversedTrieBackwardWalk() => CountMatches(new StreamCheckerByReversedTrie(Words));

    // Counts matches rather than discarding each Query result, so the JIT can't
    // eliminate the replay as dead code.
    private int CountMatches(IStreamCheckerStrategy checker)
    {
        var matches = 0;

        foreach (var letter in _stream)
        {
            if (checker.Query(letter))
            {
                matches++;
            }
        }

        return matches;
    }
}
