using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Trie;
using DSAExperimentation.LeetCode.FindTheLengthOfTheLongestCommonPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheLengthOfTheLongestCommonPrefixSolution's,
// the same methods FindTheLengthOfTheLongestCommonPrefixTests proves correct.
// The digit trie is built once in [GlobalSetup] via the solution's own
// BuildDigitTrie, so the trie arm is only ever charged for arr2's walk.
[MemoryDiagnoser]
public class FindTheLengthOfTheLongestCommonPrefixBenchmarks
{
    private const int MaxValueExclusive = 100_000_000;
    private const int Seed = 3043;

    private int[] _arr1 = [];

    private int[] _arr2 = [];
    private Trie<bool> _trie = new();
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _arr1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _arr2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _trie = FindTheLengthOfTheLongestCommonPrefixSolution.BuildDigitTrie(_arr1);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        FindTheLengthOfTheLongestCommonPrefixSolution.LongestPrefixLengthByBruteForce(_arr1, _arr2);

    [Benchmark]
    public int Trie() => FindTheLengthOfTheLongestCommonPrefixSolution.LongestPrefixLengthByTrie(_trie, _arr2);
}
