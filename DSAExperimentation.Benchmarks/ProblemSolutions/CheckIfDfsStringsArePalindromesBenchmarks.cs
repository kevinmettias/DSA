using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.CheckIfDfsStringsArePalindromes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfDfsStringsArePalindromesSolution's, the same
// methods CheckIfDfsStringsArePalindromesTests proves correct.
//
// The workload is a straight chain (CountWaysToBuildRoomsInAnAntColonyBenchmarks'
// own precedent for "worst subtree-size shape", built the same way via
// ParentArrayTree.Build over a hand-built chain parent[]): every node's own
// subtree is nearly the whole tree, so the brute-force arm's per-node
// O(subtree size) rebuild is genuine O(n^2) total, exactly what the composed arm's
// single O(n) tour plus O(1)-per-node RollingHash query is for.
[MemoryDiagnoser]
public class CheckIfDfsStringsArePalindromesBenchmarks
{
    private const int AlphabetSize = 4;
    private const int Seed = 3327;

    private RootedTreeNode[] _nodes = [];

    private string _s = "";
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nodes = BuildChainTree(NodeCount);
        _s = BuildRandomString(NodeCount);
    }

    // The chain's parent array: parent[i] = i - 1, so every node's subtree is
    // nearly the whole tree.
    private static RootedTreeNode[] BuildChainTree(int nodeCount)
    {
        var parent = new int[nodeCount];
        parent[0] = -1;

        for (var i = 1; i < nodeCount; i++)
        {
            parent[i] = i - 1;
        }

        return ParentArrayTree.Build(parent);
    }

    // The string the workloads are judged on: one seeded draw per node, in node
    // order, over an alphabet small enough that palindromic subtrees are common.
    private static string BuildRandomString(int nodeCount)
    {
        var random = new Random(Seed);
        var chars = new char[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool[] BruteForce() =>
        CheckIfDfsStringsArePalindromesSolution.IsPalindromeByBruteForce(_nodes, _s);

    [Benchmark]
    public bool[] EulerTourRollingHash() =>
        CheckIfDfsStringsArePalindromesSolution.IsPalindromeByEulerTourRollingHash(_nodes, _s);
}
