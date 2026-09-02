using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Operations on Tree (LC 1993): Upgrade's "find every locked descendant" step, repeated across many
// queries against the same near-leaf node (a tiny subtree in a large tree). WholeTreeScanEachQuery
// is the naive brute force - for every node in the whole tree, walk that node's own ancestor chain
// to test whether it sits under the target (O(NodeCount * depth) per query). SubtreeDepthFirstSearch
// EachQuery instead reuses this repo's own DepthFirestSearch.Traverse (Algorithms.Traversal.DepthFirst,
// ARCHITECTURE.md §12's bare-successor-function engine) with successors = node => node.Children, which
// only ever visits the target's actual subtree - O(subtree size) per query, independent of NodeCount.
[MemoryDiagnoser]
public class OperationsOnTreeBenchmarks
{
    private const int RandomSeed = 1993; // LC problem number
    private const int BranchingFactor = 4;
    private const int QueryCount = 2_000;
    private const int LockEveryNth = 7;

    [Params(2_000, 20_000)]
    public int NodeCount;

    private BenchmarkTreeNode[] _nodes = null!;
    private int _targetId;

    [GlobalSetup]
    public void Setup()
    {
        _nodes = CreateNodes(NodeCount);
        LinkParentChildren(_nodes);
        LockEveryNthNode(_nodes);

        // The last-created node in a heap-shaped tree has no children of its own - a leaf, so its
        // "subtree" is itself alone, the extreme case that most separates a whole-tree scan from a
        // subtree-only walk.
        _targetId = NodeCount - 1;
    }

    private static BenchmarkTreeNode[] CreateNodes(int nodeCount)
    {
        var nodes = new BenchmarkTreeNode[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new BenchmarkTreeNode(i);
        }

        return nodes;
    }

    private static void LinkParentChildren(BenchmarkTreeNode[] nodes)
    {
        for (var i = 1; i < nodes.Length; i++)
        {
            var parent = (i - 1) / BranchingFactor;
            nodes[i].Parent = nodes[parent];
            nodes[parent].Children.Add(nodes[i]);
        }
    }

    private static void LockEveryNthNode(BenchmarkTreeNode[] nodes)
    {
        for (var i = 0; i < nodes.Length; i += LockEveryNth)
        {
            nodes[i].LockedBy = 1;
        }
    }

    [Benchmark(Baseline = true)]
    public int WholeTreeScanEachQuery()
    {
        var totalFound = 0;

        for (var query = 0; query < QueryCount; query++)
        {
            totalFound += CountDescendantsWholeTreeScan(_nodes[_targetId]);
        }

        return totalFound;
    }

    private int CountDescendantsWholeTreeScan(BenchmarkTreeNode target)
    {
        var found = 0;

        foreach (var candidate in _nodes)
        {
            if (candidate != target && candidate.LockedBy != 0 && IsDescendantOf(candidate, target))
            {
                found++;
            }
        }

        return found;
    }

    private static bool IsDescendantOf(BenchmarkTreeNode candidate, BenchmarkTreeNode target)
    {
        for (var ancestor = candidate.Parent; ancestor is not null; ancestor = ancestor.Parent)
        {
            if (ancestor == target)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public int SubtreeDepthFirstSearchEachQuery()
    {
        var totalFound = 0;

        for (var query = 0; query < QueryCount; query++)
        {
            totalFound += CountDescendantsViaDfs(_nodes[_targetId]);
        }

        return totalFound;
    }

    private static int CountDescendantsViaDfs(BenchmarkTreeNode target)
    {
        var subtree = DepthFirstSearch.Traverse(target, n => n.Children);
        return subtree.Count(n => n != target && n.LockedBy != 0);
    }

    private sealed class BenchmarkTreeNode(int id)
    {
        public int Id { get; } = id;

        public List<BenchmarkTreeNode> Children { get; } = [];

        public BenchmarkTreeNode? Parent { get; set; }

        public int LockedBy { get; set; }
    }
}
