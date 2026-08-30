using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Kth Ancestor of a Tree Node (LC 1483): the textbook "walk the raw parent[] array k
// times" per query vs. this repo's own ITopDownHooks-driven precompute -
// TopDownTraversal.Walk (the same inherited-attribute primitive
// Algorithms.Paths.AllRootToLeafPaths uses to thread "the path so far" down a
// root-to-node walk) fills in every node's full ancestor chain in one O(n log n)
// pass, so each getKthAncestor call afterward is an O(1) array index instead of an
// O(depth) walk. The tree is heap-shaped (parent(i) = (i-1)/2) rather than a chain: a
// chain would make node i's ancestor array length i, so the precompute itself would
// be O(n^2) and no repo-only technique could beat the O(1)-space naive walk on it. A
// heap-shaped tree keeps every ancestor array at O(log n), which is also why the
// query batch below is large - it's what makes paying that one-time O(n log n)
// precompute worthwhile over paying O(log n) on every single query.
[MemoryDiagnoser]
public class KthAncestorOfATreeNodeBenchmarks
{
    [Params(2_000, 20_000)]
    public int NodeCount;

    private int[] _parent = null!;
    private (int Node, int K)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1483);
        _parent = new int[NodeCount];
        _parent[0] = -1;

        for (var i = 1; i < NodeCount; i++)
        {
            _parent[i] = (i - 1) / 2;
        }

        _queries = Enumerable.Range(0, 1_000_000)
            .Select(_ => (Node: random.Next(NodeCount), K: random.Next(1, NodeCount)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long WalkParentArrayPerQuery()
    {
        var total = 0L;

        foreach (var (node, k) in _queries)
        {
            total += WalkUp(node, k);
        }

        return total;
    }

    [Benchmark]
    public long PrecomputedAncestorChains()
    {
        var ancestorsById = BuildAncestorTable();
        var total = 0L;

        foreach (var (node, k) in _queries)
        {
            var ancestors = ancestorsById[node];
            total += k <= ancestors.Length ? ancestors[^k] : -1;
        }

        return total;
    }

    private int WalkUp(int node, int k)
    {
        var current = node;

        for (var step = 0; step < k; step++)
        {
            if (current == -1)
            {
                return -1;
            }

            current = _parent[current];
        }

        return current;
    }

    private int[][] BuildAncestorTable()
    {
        var nodes = new BenchmarkTreeNode[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            nodes[i] = new BenchmarkTreeNode(i);
        }

        for (var i = 1; i < NodeCount; i++)
        {
            nodes[_parent[i]].Children.Add(nodes[i]);
        }

        var ancestorsById = new int[NodeCount][];

        TopDownTraversal.Walk<
            BenchmarkTreeNode, BenchmarkTreeTopology, ListChildren<BenchmarkTreeNode>,
            NaturalChildOrder<BenchmarkTreeNode, ListChildren<BenchmarkTreeNode>>, ListChildren<BenchmarkTreeNode>,
            CollectAncestorIdsHooks, (int[] Ancestors, int[][] AncestorsById)>(
            nodes[0], ([], ancestorsById));

        return ancestorsById;
    }

    private sealed class BenchmarkTreeNode(int id)
    {
        public int Id { get; } = id;

        public List<BenchmarkTreeNode> Children { get; } = [];
    }

    private readonly struct BenchmarkTreeTopology
        : ITreeTopology<BenchmarkTreeNode, ListChildren<BenchmarkTreeNode>>
    {
        public static ListChildren<BenchmarkTreeNode> GetChildren(BenchmarkTreeNode node) => new(node.Children);
    }

    private readonly struct CollectAncestorIdsHooks
        : ITopDownHooks<BenchmarkTreeNode, (int[] Ancestors, int[][] AncestorsById)>
    {
        public static void Visit(
            BenchmarkTreeNode node, (int[] Ancestors, int[][] AncestorsById) state, int depth, NodePosition position)
            => state.AncestorsById[node.Id] = state.Ancestors;

        public static (int[] Ancestors, int[][] AncestorsById) Descend(
            BenchmarkTreeNode parent, (int[] Ancestors, int[][] AncestorsById) parentState, BenchmarkTreeNode child)
        {
            var ancestors = new int[parentState.Ancestors.Length + 1];
            Array.Copy(parentState.Ancestors, ancestors, parentState.Ancestors.Length);
            ancestors[^1] = parent.Id;

            return (ancestors, parentState.AncestorsById);
        }
    }
}
