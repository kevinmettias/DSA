using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromicPathQueriesInATreeSolution's, the same
// methods PalindromicPathQueriesInATreeTests proves correct. LcaBitmask is handed
// the pre-built RootedTreeNode[] its hoisted overload takes, so building the tree
// (never mutated by either query strategy, so safe to share across iterations) is
// charged to [GlobalSetup] rather than to the queries being measured.
[MemoryDiagnoser]
public class PalindromicPathQueriesInATreeBenchmarks
{
    private const int RandomSeed = 3841;
    private const int QueryCount = 2_000;
    private const int AlphabetSize = 4; private int[] _parent = [];

    private string _labels = "";
    private int[][] _queries = [];
    private RootedTreeNode[] _nodes = [];
    // small alphabet so paths actually collide into palindromes sometimes

    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _parent = BuildParentArray(random, NodeCount);
        _labels = BuildLabels(random, NodeCount);
        _queries = BuildQueries(random, QueryCount, NodeCount);
        _nodes = ParentArrayTree.Build(_parent);
    }

    // The three generators draw from the one seeded Random in call order, so the
    // parent, label and query streams stay exactly the streams Setup produced.
    private static int[] BuildParentArray(Random random, int nodeCount)
    {
        var parent = new int[nodeCount];
        parent[0] = -1;

        for (var i = 1; i < nodeCount; i++)
        {
            parent[i] = random.Next(0, i);
        }

        return parent;
    }

    private static string BuildLabels(Random random, int nodeCount)
    {
        var letters = new char[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            letters[i] = (char)('a' + random.Next(0, AlphabetSize));
        }

        return new string(letters);
    }

    private static int[][] BuildQueries(Random random, int queryCount, int nodeCount)
    {
        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            queries[i] = [random.Next(0, nodeCount), random.Next(0, nodeCount)];
        }

        return queries;
    }

    [Benchmark(Baseline = true)]
    public bool[] AncestorWalk() =>
        PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByAncestorWalk(_parent, _labels, _queries);

    [Benchmark]
    public bool[] LcaBitmask() =>
        PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByLcaBitmask(_nodes, _labels, _queries);
}
