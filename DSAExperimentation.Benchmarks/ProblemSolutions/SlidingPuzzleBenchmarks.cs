using BenchmarkDotNet.Attributes;
using DSAExperimentation.Domain.SlidingPuzzle;
using DSAExperimentation.LeetCode.SlidingPuzzle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SlidingPuzzleSolution's, the same methods
// SlidingPuzzleTests proves correct. ReduceGraphBfs is handed the prepared
// PuzzleGraph its hoisted overload takes, so the 720-node board-permutation
// graph is built once in [GlobalSetup] rather than on every measured call.
// StartState varies how many slides separate the board from "123450" (LC's
// own 1-move and 5-move examples), the same search-depth axis
// OpenTheLockBenchmarks' DeadendCount varies for the lock's Cayley graph.
[MemoryDiagnoser]
public class SlidingPuzzleBenchmarks
{
    [Params("123405", "412503")]
    public string StartState = null!;

    private PuzzleGraph _graph = null!;

    [GlobalSetup]
    public void Setup() => _graph = PuzzleGraph.Build();

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() => SlidingPuzzleSolution.MinMovesByMutationQueue(StartState);

    [Benchmark]
    public int ReduceGraphBfs() => SlidingPuzzleSolution.MinMovesByReduceGraph(_graph, StartState);
}
