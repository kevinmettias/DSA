using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CompleteBinaryTreeInserter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CompleteBinaryTreeInserterSolution's, the same
// factories CompleteBinaryTreeInserterSolutionTests proves correct - a rescan-per-insert
// baseline (O(current size) per call) vs. this repo's own pre-seeded incomplete-node
// queue (amortized O(1) per call). Each [Benchmark] rebuilds a fresh complete tree
// (BinaryTrees.Balanced) - perfect at 63 nodes, with a partial last level at LC 919's
// 1,000-node bound - and replays the same InsertCount insertions,
// so both strategies pay for the full sequence rather than one call - the tree is
// mutated by the run, so it cannot be hoisted into [GlobalSetup] the way the insert
// values are.
public class CompleteBinaryTreeInserterBenchmarks
{
    private const int InsertCount = 200;

    private int[] _insertValues = [];

    // The parent value every Insert reports, in order; sized in setup so the replay allocates nothing.
    private int[] _parents = [];

    [Params(63, 1_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _insertValues = Enumerable.Range(NodeCount, InsertCount).ToArray();
        _parents = new int[InsertCount];
    }

    [Benchmark(Baseline = true)]
    public int[] NaiveRescanPerInsert() =>
        Replay(CompleteBinaryTreeInserterSolution.CreateByBfsRescan(BinaryTrees.Balanced(NodeCount)));

    [Benchmark]
    public int[] QueueTrackedInserter() =>
        Replay(CompleteBinaryTreeInserterSolution.CreateByIncompleteQueue(BinaryTrees.Balanced(NodeCount)));

    private int[] Replay(ICompleteBinaryTreeInserter inserter)
    {
        for (var i = 0; i < _insertValues.Length; i++)
        {
            _parents[i] = inserter.Insert(_insertValues[i]);
        }

        return _parents;
    }
}
