using DSAExperimentation.LeetCode.PalindromePairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is PalindromePairsSolution's, the same methods
// PalindromePairsSolutionTests proves correct - the O(n^2*k) brute force that concatenates
// and checks every ordered word pair directly, the O(n*k^2) approach using this repo's own
// HashMap<TKey,TValue> as a reversed-complement lookup for every prefix/suffix split, and
// the O(sum of lengths) walk down this repo's LowercaseTrie of the words reversed that
// LC 336 requires.
//
// Words are drawn unique over three letters, at most LongestWordLength long, so k is set
// apart from n: the short words keep every arm's cost in its count of words, and words
// up to LC 336's 300 letters are where the complement lookup's k^2 shows against the
// trie's k. Both counts and lengths stay inside LC 336's 5,000 words of 300 letters.
public class PalindromePairsBenchmarks
{
    private const int AlphabetSize = 3;

    private string[] _words = [];

    [Params(80, 400)]
    public int WordCount { get; set; }

    [Params(8, 300)]
    public int LongestWordLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var unique = new HashSet<string>();

        while (unique.Count < WordCount)
        {
            var length = random.Next(1, LongestWordLength + 1);
            var chars = new char[length];

            for (var i = 0; i < length; i++)
            {
                chars[i] = (char)('a' + random.Next(AlphabetSize));
            }

            unique.Add(new string(chars));
        }

        _words = [.. unique];
    }

    [Benchmark(Baseline = true)]
    public List<(int First, int Second)> BruteForce() => PalindromePairsSolution.FindPairsByBruteForce(_words);

    [Benchmark]
    public List<(int First, int Second)> HashMapComplementLookup() =>
        PalindromePairsSolution.FindPairsByHashMapComplementLookup(_words);

    [Benchmark]
    public List<(int First, int Second)> ReversedWordTrie() =>
        PalindromePairsSolution.FindPairsByReversedWordTrie(_words);
}
