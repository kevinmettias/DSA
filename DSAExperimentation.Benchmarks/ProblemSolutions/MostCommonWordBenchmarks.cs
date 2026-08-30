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
    [Params(1_000, 20_000)]
    public int Length;

    private string[] _words = null!;
    private string[] _banned = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(819);

        // Bounded to a pool far smaller than Length so words repeat and a real
        // "most common" winner emerges, the same reasoning TopKFrequentWords/
        // TopKFrequentElements benchmarks already document for their own pools.
        _words = Enumerable.Range(0, Length).Select(_ => "word" + random.Next(0, 500)).ToArray();
        _banned = ["word0", "word1", "word2", "word3", "word4"];
    }

    [Benchmark(Baseline = true)]
    public string DictionaryAndHashSet()
    {
        var bannedWords = new HashSet<string>(_banned);
        var counts = new Dictionary<string, int>();
        var best = string.Empty;
        var bestCount = 0;

        foreach (var word in _words)
        {
            if (bannedWords.Contains(word))
            {
                continue;
            }

            var count = counts.GetValueOrDefault(word) + 1;
            counts[word] = count;

            if (count > bestCount)
            {
                bestCount = count;
                best = word;
            }
        }

        return best;
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
        var best = string.Empty;
        var bestCount = 0;

        foreach (var word in _words)
        {
            if (bannedWords.Has(word))
            {
                continue;
            }

            counts.TryGetValue(word, out var count);
            count++;
            counts.Set(word, count);

            if (count > bestCount)
            {
                bestCount = count;
                best = word;
            }
        }

        return best;
    }
}
