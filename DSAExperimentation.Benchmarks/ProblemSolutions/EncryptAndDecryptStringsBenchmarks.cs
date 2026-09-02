using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Encrypt and Decrypt Strings (LC 2227): re-encrypting the whole dictionary on
// every decrypt call (the naive reading of the problem) vs. precomputing each
// dictionary word's encryption once into this repo's own HashMap<string,int>
// frequency table (EncryptAndDecryptStringsTests precedent), turning every
// subsequent decrypt into an O(1) average lookup. DictionarySize drives both the
// per-call cost of the naive approach and the one-time build cost of the
// precomputed table; DecryptCalls stays fixed so the gap widens purely with
// dictionary size, the same "force the real worst case" shape TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class EncryptAndDecryptStringsBenchmarks
{
    private const int DecryptCalls = 100;
    private const int WordLength = 20;
    private const int AlphabetSize = 26;
    private const int EncryptedCharLength = 2;

    [Params(50, 2_000)]
    public int DictionarySize;

    private HashMap<char, string> _valueByKey = null!;
    private string[] _dictionary = null!;
    private string[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _valueByKey = BuildKeyMap();
        _dictionary = Enumerable.Range(0, DictionarySize).Select(_ => RandomWord(random)).ToArray();

        // Every query is the true encryption of some dictionary word, so both
        // strategies do genuine matching work instead of an always-empty lookup.
        _queries = Enumerable.Range(0, DecryptCalls)
            .Select(_ => Encrypt(_dictionary[random.Next(_dictionary.Length)], _valueByKey))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RecomputeEveryDecrypt()
    {
        var matches = 0;

        foreach (var query in _queries)
        {
            foreach (var word in _dictionary)
            {
                if (Encrypt(word, _valueByKey) == query)
                {
                    matches++;
                }
            }
        }

        return matches;
    }

    [Benchmark]
    public int PrecomputedFrequencyMap()
    {
        var encryptedCounts = new HashMap<string, int>();

        foreach (var word in _dictionary)
        {
            var encrypted = Encrypt(word, _valueByKey);
            encryptedCounts.TryGetValue(encrypted, out var count);
            encryptedCounts.Set(encrypted, count + 1);
        }

        var matches = 0;

        foreach (var query in _queries)
        {
            encryptedCounts.TryGetValue(query, out var count);
            matches += count;
        }

        return matches;
    }

    private static HashMap<char, string> BuildKeyMap()
    {
        var map = new HashMap<char, string>();

        for (var i = 0; i < AlphabetSize; i++)
        {
            var key = (char)('a' + i);
            var value = new string([(char)('a' + (i % AlphabetSize)), (char)('a' + ((i + 1) % AlphabetSize))]);
            map.Set(key, value);
        }

        return map;
    }

    private static string RandomWord(Random random)
        => new([.. Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize)))]);

    private static string Encrypt(string word, HashMap<char, string> valueByKey)
    {
        var result = new System.Text.StringBuilder(word.Length * EncryptedCharLength);

        foreach (var c in word)
        {
            valueByKey.TryGetValue(c, out var mapped);
            result.Append(mapped);
        }

        return result.ToString();
    }
}
