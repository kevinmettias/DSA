using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OperationsOnTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OperationsOnTreeSolution's, the same factories
// OperationsOnTreeTests proves correct. What is measured is upgrade's "find every
// locked descendant" step, repeated across many queries against the same near-leaf
// node in a large tree - the extreme case that most separates a whole-tree scan
// (O(NodeCount * depth) per query) from a subtree-only walk (O(subtree size), so
// O(1) here).
//
// [GlobalSetup] builds one heap-shaped tree per arm and locks every seventh node,
// so tree construction and the lock state are charged to setup rather than to the
// query being measured. The target is the last-created node, which in a heap-shaped
// tree is a leaf with no children of its own.
[MemoryDiagnoser]
public class OperationsOnTreeBenchmarks
{
    private const int BranchingFactor = 4;
    private const int QueryCount = 2_000;
    private const int LockEveryNth = 7;
    private const int LockHolder = 1;

    private LockingTree _wholeTreeScan = null!;

    private LockingTree _subtreeDepthFirstSearch = null!;
    private int _targetId;
    [Params(2_000, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var parent = HeapShapedParents(NodeCount);
        _wholeTreeScan = LockEveryNthNode(OperationsOnTreeSolution.CreateByWholeTreeScan(parent));
        _subtreeDepthFirstSearch =
            LockEveryNthNode(OperationsOnTreeSolution.CreateBySubtreeDepthFirstSearch(parent));
        _targetId = NodeCount - 1;
    }

    private static int[] HeapShapedParents(int nodeCount)
    {
        var parent = new int[nodeCount];
        parent[0] = -1;

        for (var i = 1; i < nodeCount; i++)
        {
            parent[i] = (i - 1) / BranchingFactor;
        }

        return parent;
    }

    private static LockingTree LockEveryNthNode(LockingTree tree)
    {
        for (var i = 0; i < tree.NodeCount; i += LockEveryNth)
        {
            tree.Lock(i, LockHolder);
        }

        return tree;
    }

    [Benchmark(Baseline = true)]
    public int WholeTreeScanEachQuery() => TotalLockedDescendants(_wholeTreeScan);

    [Benchmark]
    public int SubtreeDepthFirstSearchEachQuery() => TotalLockedDescendants(_subtreeDepthFirstSearch);

    private int TotalLockedDescendants(LockingTree tree)
    {
        var totalFound = 0;

        for (var query = 0; query < QueryCount; query++)
        {
            totalFound += tree.LockedDescendantsOf(_targetId).Length;
        }

        return totalFound;
    }
}
