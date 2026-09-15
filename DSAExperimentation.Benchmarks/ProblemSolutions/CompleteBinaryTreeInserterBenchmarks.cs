using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CompleteBinaryTreeInserter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CompleteBinaryTreeInserterSolution's, the same
// factories CompleteBinaryTreeInserterTests proves correct - a rescan-per-insert
// baseline (O(current size) per call) vs. this repo's own pre-seeded incomplete-node
// queue (amortized O(1) per call). Each [Benchmark] rebuilds a fresh perfect tree
// (BinaryTrees.Balanced, 2^k-1 nodes) and replays the same InsertCount insertions,
// so both strategies pay for the full sequence rather than one call - the tree is
// mutated by the run, so it cannot be hoisted into [GlobalSetup] the way the insert
// values are.
[MemoryDiagnoser]
public class CompleteBinaryTreeInserterBenchmarks
{
    private const int InsertCount = 200;

    private int[] _insertValues = [];

    [Params(63, 1_023)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _insertValues = Enumerable.Range(NodeCount, InsertCount).ToArray();

    [Benchmark(Baseline = true)]
    public int NaiveRescanPerInsert() =>
        Replay(CompleteBinaryTreeInserterSolution.CreateByBfsRescan(BinaryTrees.Balanced(NodeCount)));

    [Benchmark]
    public int QueueTrackedInserter() =>
        Replay(CompleteBinaryTreeInserterSolution.CreateByIncompleteQueue(BinaryTrees.Balanced(NodeCount)));

    private int Replay(ICompleteBinaryTreeInserter inserter)
    {
        var lastParent = 0;

        foreach (var value in _insertValues)
        {
            lastParent = inserter.Insert(value);
        }

        return lastParent;
    }
}
