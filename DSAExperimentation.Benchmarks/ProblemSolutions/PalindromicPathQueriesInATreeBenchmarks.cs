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
    private const int AlphabetSize = 4; // small alphabet so paths actually collide into palindromes sometimes

    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _parent = null!;
    private string _labels = null!;
    private int[][] _queries = null!;
    private RootedTreeNode[] _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _parent = new int[NodeCount];
        _parent[0] = -1;

        for (var i = 1; i < NodeCount; i++)
        {
            _parent[i] = random.Next(0, i);
        }

        var letters = new char[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            letters[i] = (char)('a' + random.Next(0, AlphabetSize));
        }

        _labels = new string(letters);

        _queries = new int[QueryCount][];

        for (var i = 0; i < QueryCount; i++)
        {
            _queries[i] = [random.Next(0, NodeCount), random.Next(0, NodeCount)];
        }

        _nodes = ParentArrayTree.Build(_parent);
    }

    [Benchmark(Baseline = true)]
    public bool[] AncestorWalk() =>
        PalindromicPathQueriesInATreeSolution.IsPalindromePathByAncestorWalk(_parent, _labels, _queries);

    [Benchmark]
    public bool[] LcaBitmask() =>
        PalindromicPathQueriesInATreeSolution.IsPalindromePathByLcaBitmask(_nodes, _labels, _queries);
}
