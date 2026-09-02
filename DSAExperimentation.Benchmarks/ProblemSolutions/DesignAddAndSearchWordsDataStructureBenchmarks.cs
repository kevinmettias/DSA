using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignAddAndSearchWordsDataStructure.DesignAddAndSearchWordsDataStructureSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAddAndSearchWordsDataStructureSolution's, the
// same classes DesignAddAndSearchWordsDataStructureTests proves correct.
// [GlobalSetup] builds one fixed word list and search-pattern list - half exact
// lookups, half single-position wildcards - so word/pattern generation is
// charged to setup rather than to the add-then-search replay each [Benchmark]
// arm measures.
[MemoryDiagnoser]
public class DesignAddAndSearchWordsDataStructureBenchmarks
{
    private const int Seed = 211;
    private const int WordLength = 8;

    [Params(200, 2_000)]
    public int WordCount;

    private string[] _wordsToAdd = null!;
    private string[] _searches = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _wordsToAdd = Enumerable.Range(0, WordCount).Select(_ => RandomWord(random)).ToArray();
        _searches = BuildSearches(_wordsToAdd, random);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => Replay(new WordDictionaryByLinearScan());

    [Benchmark]
    public int Trie() => Replay(new WordDictionaryByTrie());

    // Counts matches rather than discarding each Search result, so the JIT
    // can't eliminate the replay as dead code - the same "return the real
    // answer, not a weaker proxy" shape DesignSpreadsheetBenchmarks follows.
    private int Replay(IWordDictionaryStrategy dictionary)
    {
        foreach (var word in _wordsToAdd)
        {
            dictionary.AddWord(word);
        }

        var matches = 0;

        foreach (var pattern in _searches)
        {
            if (dictionary.Search(pattern))
            {
                matches++;
            }
        }

        return matches;
    }

    private static string RandomWord(Random random) =>
        new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(26))).ToArray());

    // Half exact lookups against words actually added (a mix of hits and
    // misses depending on duplicates), half single-position wildcards over
    // those same words - the two query shapes each strategy's Search has to
    // answer differently, since a Trie fast path only helps the former.
    private static string[] BuildSearches(string[] words, Random random)
    {
        var searches = new string[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            var word = words[random.Next(words.Length)];
            searches[i] = i % 2 == 0 ? word : WithWildcard(word, random);
        }

        return searches;
    }

    private static string WithWildcard(string word, Random random)
    {
        var chars = word.ToCharArray();
        chars[random.Next(chars.Length)] = '.';
        return new string(chars);
    }
}
