using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Most Common Word (LC 819): the textbook Dictionary<string,int> + HashSet<string>
// scan vs. this repo's own HashMap<string,int> (word counts) plus Set<string>
// (banned words) - the same count-with-HashMap shape TopKFrequentWordsBenchmarks
// already exercises for LC 692, with Set<string> standing in for "words to skip."
// Banned words are a small, fixed slice of the vocabulary so both strategies do
// real filtering work, not just counting.
[MemoryDiagnoser]
public class MostCommonWordBenchmarks
{
    // LC problem number, used as the deterministic benchmark input seed.
    private const int RandomSeed = 819;

    // Prefix shared by every generated word, e.g. "word0", "word1", ...
    private const string WordPoolPrefix = "word";

    // Bounded to a pool far smaller than Length so words repeat and a real
    // "most common" winner emerges, the same reasoning TopKFrequentWords/
    // TopKFrequentElements benchmarks already document for their own pools.
    private const int WordPoolSize = 500;

    // Number of distinct words banned from counting.
    private const int BannedWordCount = 5;

    [Params(1_000, 20_000)]
    public int Length;

    private string[] _words = null!;
    private string[] _banned = null!;

    private readonly record struct BestWordState(string Word, int Count);

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _words = Enumerable.Range(0, Length).Select(_ => WordPoolPrefix + random.Next(0, WordPoolSize)).ToArray();
        _banned = Enumerable.Range(0, BannedWordCount).Select(i => WordPoolPrefix + i).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string DictionaryAndHashSet()
    {
        var bannedWords = new HashSet<string>(_banned);
        var counts = new Dictionary<string, int>();
        var state = new BestWordState(string.Empty, 0);

        foreach (var word in _words)
        {
            state = UpdateWordCount(bannedWords, counts, word, state);
        }

        return state.Word;
    }

    [Benchmark]
    public string HashMapAndSet()
    {
        var bannedWords = new Set<string>();

        foreach (var word in _banned)
        {
            bannedWords.TryAdd(word);
        }

        var counts = new HashMap<string, int>();
        var state = new BestWordState(string.Empty, 0);

        foreach (var word in _words)
        {
            state = UpdateWordCount(bannedWords, counts, word, state);
        }

        return state.Word;
    }

    private static BestWordState UpdateWordCount(HashSet<string> bannedWords, Dictionary<string, int> counts, string word, BestWordState state)
    {
        if (bannedWords.Contains(word))
        {
            return state;
        }

        var count = counts.GetValueOrDefault(word) + 1;
        counts[word] = count;

        return count > state.Count ? new BestWordState(word, count) : state;
    }

    private static BestWordState UpdateWordCount(Set<string> bannedWords, HashMap<string, int> counts, string word, BestWordState state)
    {
        if (bannedWords.Has(word))
        {
            return state;
        }

        counts.TryGetValue(word, out var count);
        count++;
        counts.Set(word, count);

        return count > state.Count ? new BestWordState(word, count) : state;
    }
}
