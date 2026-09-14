using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.MinimumReverseOperations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumReverseOperationsSolution's, the same methods
// MinimumReverseOperationsTests proves correct. BruteForceScan tests every one of the
// n candidate destinations for each position popped off the BFS frontier (O(n) per
// pop), while ReduceGraph composes this repo's own Reduce.Graph over
// ReversalTopology, whose ReversalChildren computes only the O(K) positions actually
// reachable in one reversal directly from the window arithmetic - so the gap between
// the two arms widens as K shrinks relative to n.
//
// The ReduceGraph arm is handed a prepared ReversalBoard so board construction is
// charged to [GlobalSetup] rather than to the search (#17.4); the scan arm takes
// LeetCode's own (n, p, banned, k) because that is already its input.
[MemoryDiagnoser]
public class MinimumReverseOperationsBenchmarks
{
    // Small relative to NodeCount, so ReduceGraph's O(K)-per-pop children are a real
    // fraction of BruteForceScan's O(n)-per-pop full scan.
    private const int WindowSize = 8;
    private const int StartPosition = 0;

    [Params(200, 4_000)]
    public int NodeCount;

    private int[] _banned = null!;
    private ReversalBoard _board = null!;

    [GlobalSetup]
    public void Setup()
    {
        _banned = [];
        _board = new ReversalBoard(NodeCount, WindowSize, new Set<int>(_banned));
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() =>
        MinimumReverseOperationsSolution.MinOperationsByBruteForceScan(
            NodeCount, StartPosition, _banned, WindowSize);

    [Benchmark]
    public int[] ReduceGraph() =>
        MinimumReverseOperationsSolution.MinOperationsByReduceGraph(_board, StartPosition);
}
