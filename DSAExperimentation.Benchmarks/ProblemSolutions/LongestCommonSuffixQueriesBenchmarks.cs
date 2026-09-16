using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Trie;
using DSAExperimentation.LeetCode.LongestCommonSuffixQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonSuffixQueriesSolution's, the same
// methods LongestCommonSuffixQueriesTests proves correct. The trie arm is handed an
// already-built Trie<int>, so container insertion is charged to [GlobalSetup] and
// only query answering is measured.
[MemoryDiagnoser]
public class LongestCommonSuffixQueriesBenchmarks
{
    private const int Seed = 3093;

    private string[] _wordsContainer = [];

    private string[] _wordsQuery = [];
    private Trie<int> _trie = new();
    [Params(30, 200)]
    public int ContainerSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _wordsContainer = LongestCommonSuffixQueriesWorkloads.BuildWords(ContainerSize, random);
        _wordsQuery = LongestCommonSuffixQueriesWorkloads.BuildWords(ContainerSize, random);
        _trie = LongestCommonSuffixQueriesSolution.BuildSuffixTrie(_wordsContainer);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        LongestCommonSuffixQueriesSolution.FindIndicesByBruteForce(_wordsContainer, _wordsQuery);

    [Benchmark]
    public int[] Trie() =>
        LongestCommonSuffixQueriesSolution.FindIndicesByTrie(_trie, _wordsQuery);
}
