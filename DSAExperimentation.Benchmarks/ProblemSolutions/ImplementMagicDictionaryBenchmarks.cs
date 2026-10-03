using DSAExperimentation.LeetCode.ImplementMagicDictionary;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementMagicDictionarySolution's, the same
// factories ImplementMagicDictionarySolutionTests proves correct. [GlobalSetup] builds
// the dictionary and, for each dictionary word, a search word that is exactly
// one character away from it (guaranteed to be a real match) - build+scan cost
// has to be paid every call either way, so Replay processes a full batch of
// search words per call rather than just one, the same "script construction
// charged to setup, replay is what gets measured" shape LRUCacheBenchmarks
// already uses for its own instance-API problem. O(words * dictionarySize *
// wordLength) vs. O(dictionarySize * wordLength) to build the trie once plus
// O(words * wordLength * alphabetSize) to search it. DictionarySize stops at LC 676's
// 100-word cap, which also keeps the one-search-per-word batch inside its 100 search
// calls.
public class ImplementMagicDictionaryBenchmarks
{
    private const int WordLength = 8;

    private const int RandomSeed = 676; // LeetCode problem number

    private const int AlphabetSize = 26;

    private string[] _dictionary = [];

    private string[] _searchWords = [];

    // Every Search verdict, in search-word order; sized in setup so the replay allocates nothing.
    private bool[] _verdicts = [];
    [Params(10, 100)]
    public int DictionarySize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _dictionary = Enumerable.Range(0, DictionarySize).Select(_ => RandomWord(random)).Distinct().ToArray();
        _searchWords = _dictionary.Select(word => OneCharacterAway(word, random)).ToArray();
        _verdicts = new bool[_searchWords.Length];
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    private static string OneCharacterAway(string word, Random random)
    {
        var characters = word.ToCharArray();
        var position = random.Next(word.Length);
        characters[position] = (char)('a' + ((characters[position] - 'a' + 1) % AlphabetSize));
        return new string(characters);
    }

    [Benchmark(Baseline = true)]
    public bool[] BruteForce() => Replay(ImplementMagicDictionarySolution.CreateByBruteForce());

    [Benchmark]
    public bool[] TrieSearch() => Replay(ImplementMagicDictionarySolution.CreateByTrieSearch());

    private bool[] Replay(ImplementMagicDictionarySolution.IMagicDictionary magicDictionary)
    {
        magicDictionary.BuildDict(_dictionary);

        for (var i = 0; i < _searchWords.Length; i++)
        {
            _verdicts[i] = magicDictionary.Search(_searchWords[i]);
        }

        return _verdicts;
    }
}
