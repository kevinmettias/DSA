using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PalindromePairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePairsSolution's, the same methods
// PalindromePairsTests proves correct - the O(n^2*k) brute force that concatenates
// and checks every ordered word pair directly vs. the O(n*k^2) approach using this
// repo's own HashMap<TKey,TValue> as a reversed-complement lookup for every
// prefix/suffix split.
[MemoryDiagnoser]
public class PalindromePairsBenchmarks
{
    private const int WordLengthBound = 9;
    private const int AlphabetSize = 3;

    [Params(80, 400)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var unique = new HashSet<string>();

        while (unique.Count < WordCount)
        {
            var length = random.Next(1, WordLengthBound);
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
    public int BruteForce() => PalindromePairsSolution.FindPairsByBruteForce(_words).Count;

    [Benchmark]
    public int HashMapComplementLookup() => PalindromePairsSolution.FindPairsByHashMapComplementLookup(_words).Count;
}
