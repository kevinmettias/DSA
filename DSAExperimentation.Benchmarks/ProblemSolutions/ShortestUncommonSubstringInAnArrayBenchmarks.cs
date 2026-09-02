using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortestUncommonSubstringInAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestUncommonSubstringInAnArraySolution's, the
// same methods ShortestUncommonSubstringInAnArrayTests proves correct. Words
// are drawn from a small 4-letter alphabet so words share a lot of substrings
// with each other - the case that forces both strategies through most of their
// candidate lists instead of resolving at length 1.
[MemoryDiagnoser]
public class ShortestUncommonSubstringInAnArrayBenchmarks
{
    private const int WordLength = 20;
    private const int AlphabetSize = 4;
    private const int Seed = 3076;

    [Params(10, 100)]
    public int WordCount;

    private string[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _arr = Enumerable.Range(0, WordCount).Select(_ => RandomWord(random)).ToArray();
    }

    private static string RandomWord(Random random)
    {
        var chars = new char[WordLength];

        for (var i = 0; i < WordLength; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string[] BruteForce() =>
        ShortestUncommonSubstringInAnArraySolution.FindShortestUncommonSubstringsByBruteForce(_arr);

    [Benchmark]
    public string[] AhoCorasick() =>
        ShortestUncommonSubstringInAnArraySolution.FindShortestUncommonSubstringsByAhoCorasick(_arr);
}
