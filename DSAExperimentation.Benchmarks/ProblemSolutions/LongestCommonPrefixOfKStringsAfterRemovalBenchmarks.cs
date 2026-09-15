using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LongestCommonPrefixOfKStringsAfterRemoval;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonPrefixOfKStringsAfterRemovalSolution's,
// the same methods LongestCommonPrefixOfKStringsAfterRemovalTests proves correct.
// [GlobalSetup] builds the trie once, so the composed arm's measured call is only
// the two Reduce.Tree passes plus the per-word answer walk - trie construction is
// charged to setup exactly like OpenTheLockBenchmarks charges LockGraph.Build
// there. Word count stays modest: the brute-force arm is O(n * maxLength^2).
[MemoryDiagnoser]
public class LongestCommonPrefixOfKStringsAfterRemovalBenchmarks
{
    private const int Seed = 3485; // LC problem number
    private const int MaxLength = 12;
    private const int K = 3;

    private string[] _words = [];

    private LowercaseTrie<int> _trie = new();
    [Params(20, 100)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _words = LongestCommonPrefixOfKStringsWorkloads.BuildWords(WordCount, MaxLength, Seed);
        _trie = LongestCommonPrefixOfKStringsAfterRemovalSolution.BuildTrie(_words);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByBruteForce(_words, K);

    [Benchmark]
    public int[] ReduceTrie() =>
        LongestCommonPrefixOfKStringsAfterRemovalSolution.AnswerByReduceTrie(_trie, _words, K);
}
