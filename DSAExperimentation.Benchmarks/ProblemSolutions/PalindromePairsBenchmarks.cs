using DSAExperimentation.LeetCode.Solutions.PalindromePairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is PalindromePairsSolution's, the same methods
// PalindromePairsSolutionTests proves correct - the O(n^2*k) brute force that concatenates
// and checks every ordered word pair directly, the O(n*k^2) approach using this repo's own
// HashMap<TKey,TValue> as a reversed-complement lookup for every prefix/suffix split, and
// the O(sum of lengths) walk down this repo's trie of the words reversed that
// LC 336 requires.
//
// Words are drawn unique over three letters, at most LongestWordLength long, so k is set
// apart from n: the short words keep every arm's cost in its count of words, and words
// up to LC 336's 300 letters are where the complement lookup's k^2 shows against the
// trie's k.
//
// Sizes are per arm. The brute force's n^2 pairs stop it at 2,000 words, where a call on
// words of up to 300 letters took 0.3 s, against 2.2 s at LC 336's 5,000 (2026-11-05,
// Release, the best of three calls, on efficiency cores). The complement
// lookup and the trie, linear in the word count, run on to 5,000, at 0.4 s and 0.1 s,
// and all three are compared at the counts they share. Every count and length stays
// inside LC 336's 5,000 words of 300 letters.
public class PalindromePairsBenchmarks
{
    private const int AlphabetSize = 3;
    private const int RandomSeed = 1;

    private Dictionary<int, string[]> _wordsByCount = [];

    [Params(8, 300)]
    public int LongestWordLength { get; set; }

    public static IEnumerable<int> BruteForceWordCounts => [80, 400, 2_000];

    public static IEnumerable<int> LinearWordCounts => [.. BruteForceWordCounts, 5_000];

    // Every count any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _wordsByCount = LinearWordCounts.ToDictionary(wordCount => wordCount, BuildWords);

    private string[] BuildWords(int wordCount)
    {
        var random = new Random(RandomSeed);
        var unique = new HashSet<string>();

        while (unique.Count < wordCount)
        {
            var length = random.Next(1, LongestWordLength + 1);
            var chars = new char[length];

            for (var i = 0; i < length; i++)
            {
                chars[i] = (char)('a' + random.Next(AlphabetSize));
            }

            unique.Add(new string(chars));
        }

        return [.. unique];
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceWordCounts))]
    public List<(int First, int Second)> BruteForce(int wordCount) =>
        PalindromePairsSolution.FindPairsByBruteForce(_wordsByCount[wordCount]);

    [Benchmark]
    [ArgumentsSource(nameof(LinearWordCounts))]
    public List<(int First, int Second)> HashMapComplementLookup(int wordCount) =>
        PalindromePairsSolution.FindPairsByHashMapComplementLookup(_wordsByCount[wordCount]);

    [Benchmark]
    [ArgumentsSource(nameof(LinearWordCounts))]
    public List<(int First, int Second)> ReversedWordTrie(int wordCount) =>
        PalindromePairsSolution.FindPairsByReversedWordTrie(_wordsByCount[wordCount]);
}
