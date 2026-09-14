using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WordsWithinTwoEditsOfDictionary;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordsWithinTwoEditsOfDictionarySolution's, the same
// methods WordsWithinTwoEditsOfDictionaryTests proves correct. [GlobalSetup] builds
// the dictionary and the query batch - LeetCode's own input shape, handed straight
// to each strategy, so no prepared-input overload is needed - leaving each arm to
// measure only the matching.
//
// Scanning the whole dictionary and counting mismatched characters for every query
// is O(queries * dictionarySize * wordLength); building the LowercaseTrie<bool>
// once is O(dictionarySize * wordLength) plus a budgeted walk per query. Each query
// is exactly one dictionary word with 0, 1, or 2 characters changed (so every query
// really does match), and neither strategy short-circuits on an early "definitely
// no match" bail-out.
//
// Section 17.8 note: both arms previously COUNTED matches; they now return
// LeetCode's actual answer, the matching queries themselves, so the measurement
// includes materializing that list. Every query matches by construction, so the
// list is the full query batch in both arms and the comparison is still about
// matching cost.
[MemoryDiagnoser]
public class WordsWithinTwoEditsOfDictionaryBenchmarks
{
    private const int WordLength = 8;

    private const int RandomSeed = 2452; // LeetCode problem number

    private const int AlphabetSize = 26;

    private const int MaxEdits = 2;

    [Params(2_000, 6_000)]
    public int DictionarySize;

    private string[] _dictionary = null!;
    private string[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _dictionary = Enumerable.Range(0, DictionarySize).Select(_ => RandomWord(random)).Distinct().ToArray();
        _queries = _dictionary.Select(word => WithinEditBudget(word, random)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string[] BruteForce() =>
        WordsWithinTwoEditsOfDictionarySolution.FindMatchingQueriesByBruteForce(_queries, _dictionary);

    [Benchmark]
    public string[] TrieSearch() =>
        WordsWithinTwoEditsOfDictionarySolution.FindMatchingQueriesByEditBudgetTrie(_queries, _dictionary);

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    private static string WithinEditBudget(string word, Random random)
    {
        var characters = word.ToCharArray();
        var editCount = random.Next(MaxEdits + 1); // 0, 1, or 2 edits

        for (var i = 0; i < editCount; i++)
        {
            var position = random.Next(word.Length);
            characters[position] = (char)('a' + ((characters[position] - 'a' + 1) % AlphabetSize));
        }

        return new string(characters);
    }
}
