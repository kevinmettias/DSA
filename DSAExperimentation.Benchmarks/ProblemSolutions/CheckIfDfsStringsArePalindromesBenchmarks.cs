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

    [Params(200, 2_000)]
    public int NodeCount;

    private RootedTreeNode[] _nodes = null!;
    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var parent = new int[NodeCount];
        parent[0] = -1;

        for (var i = 1; i < NodeCount; i++)
        {
            parent[i] = i - 1;
        }

        _nodes = ParentArrayTree.Build(parent);

        var random = new Random(Seed);
        var chars = new char[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _s = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool[] BruteForce() =>
        CheckIfDfsStringsArePalindromesSolution.IsPalindromeByBruteForce(_nodes, _s);

    [Benchmark]
    public bool[] EulerTourRollingHash() =>
        CheckIfDfsStringsArePalindromesSolution.IsPalindromeByEulerTourRollingHash(_nodes, _s);
}
