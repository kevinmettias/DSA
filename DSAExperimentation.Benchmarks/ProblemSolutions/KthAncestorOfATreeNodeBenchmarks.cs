using DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthAncestorOfATreeNodeSolution's, the same methods
// KthAncestorOfATreeNodeSolutionTests proves correct - the textbook "walk the raw
// parent[] array k times" per query against a binary-lifting table that answers
// each query in one jump per set bit of k.
//
// The table's construction deliberately stays inside the measured arm rather than
// moving to [GlobalSetup]: what this comparison is about is whether one
// O(n log n) build pays for itself across the query batch, and hoisting it would
// measure only the jumps. The parent array and the query batch - the workload's
// sizing and seeding - are what [GlobalSetup] builds.
//
// Branching picks the tree's shape: node i's parent is (i - 1) / Branching. At 1
// that is a straight chain, the deepest tree LeetCode allows, where a walk costs
// up to n steps per query; at 2 it is a balanced binary tree only log2(n) deep,
// where a walk is never long and the table has to pay for its build. Both run to
// LeetCode's 5 * 10^4 nodes, against its full 5 * 10^4 queries.
public class KthAncestorOfATreeNodeBenchmarks
{
    private const int RandomSeed = 1483; // LC problem number
    private const int QueryCount = 50_000;
    private const int RootParent = -1;

    private int[] _parent = [];

    private (int Node, int K)[] _queries = [];

    // Every query's answer, in query order - what each arm returns.
    private int[] _ancestors = [];

    [Params(2_000, 50_000)]
    public int NodeCount { get; set; }

    [Params(1, 2)]
    public int Branching { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _parent = new int[NodeCount];
        _parent[0] = RootParent;

        for (var i = 1; i < NodeCount; i++)
        {
            _parent[i] = (i - 1) / Branching;
        }

        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => (Node: random.Next(NodeCount), K: random.Next(1, NodeCount)))
            .ToArray();
        _ancestors = new int[QueryCount];
    }

    [Benchmark(Baseline = true)]
    public int[] WalkParentArrayPerQuery()
    {
        for (var i = 0; i < _queries.Length; i++)
        {
            var (node, k) = _queries[i];
            _ancestors[i] = KthAncestorOfATreeNodeSolution.GetKthAncestorByParentWalk(_parent, node, k);
        }

        return _ancestors;
    }

    [Benchmark]
    public int[] BinaryLiftingTable()
    {
        var jumps = KthAncestorOfATreeNodeSolution.BuildAncestorJumps(_parent);

        for (var i = 0; i < _queries.Length; i++)
        {
            var (node, k) = _queries[i];
            _ancestors[i] = KthAncestorOfATreeNodeSolution.GetKthAncestorByBinaryLifting(jumps, node, k);
        }

        return _ancestors;
    }
}
