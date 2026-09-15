using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.EncryptAndDecryptStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EncryptAndDecryptStringsSolution's, the same
// strategies EncryptAndDecryptStringsTests proves correct - re-encrypting the
// whole dictionary on every decrypt call against precomputing each word's
// encryption once into this repo's own HashMap<string, int> frequency table.
//
// The encrypter is built INSIDE each measured method on purpose: for a design
// problem the construction cost is half of what the two strategies differ in, so
// hoisting it would hide the precomputed arm's one-time table build. Only the
// inputs LeetCode itself hands the constructor - keys, values, the dictionary and
// the query words - are built in [GlobalSetup].
//
// DictionarySize drives both the per-call cost of the rescan arm and the one-time
// build cost of the precomputed table; DecryptCalls stays fixed so the gap widens
// purely with dictionary size, the same "force the real worst case" shape
// TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class EncryptAndDecryptStringsBenchmarks
{
    private const int DecryptCalls = 100;
    private const int WordLength = 20;
    private const int AlphabetSize = 26;
    private const int RandomSeed = 1;

    private char[] _keys = [];

    private string[] _values = [];
    private string[] _dictionary = [];
    private string[] _queries = [];
    [Params(50, 2_000)]
    public int DictionarySize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _keys = [.. Enumerable.Range(0, AlphabetSize).Select(index => (char)('a' + index))];
        _values = [.. Enumerable.Range(0, AlphabetSize).Select(NextLetterPair)];
        _dictionary = [.. Enumerable.Range(0, DictionarySize).Select(_ => RandomWord(random))];

        // Every query is the true encryption of some dictionary word, so both
        // strategies do genuine matching work instead of an always-empty lookup.
        var encrypter = EncryptAndDecryptStringsSolution.CreateByPrecomputedFrequency(_keys, _values, _dictionary);

        _queries =
        [
            .. Enumerable.Range(0, DecryptCalls)
                .Select(_ => encrypter.Encrypt(_dictionary[random.Next(_dictionary.Length)])),
        ];
    }

    private static string RandomWord(Random random)
        => new([.. Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize)))]);

    [Benchmark(Baseline = true)]
    public int RecomputeEveryDecrypt()
    {
        var encrypter = EncryptAndDecryptStringsSolution.CreateByDictionaryRescan(_keys, _values, _dictionary);

        return TotalMatches(encrypter);
    }

    [Benchmark]
    public int PrecomputedFrequencyMap()
    {
        var encrypter = EncryptAndDecryptStringsSolution.CreateByPrecomputedFrequency(_keys, _values, _dictionary);

        return TotalMatches(encrypter);
    }

    private static string NextLetterPair(int index)
        => new([(char)('a' + index), (char)('a' + ((index + 1) % AlphabetSize))]);

    private int TotalMatches(EncryptAndDecryptStringsSolution.IEncrypter encrypter)
    {
        var matches = 0;

        foreach (var query in _queries)
        {
            matches += encrypter.Decrypt(query);
        }

        return matches;
    }
}
