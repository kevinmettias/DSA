using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIVSolution's, the same methods StoneGameIVTests
// proves correct. Plain un-memoized minimax recursion over the remaining stone count -
// exponential, since the same remaining count recurs through many different perfect-
// square-removal sequences reaching it - vs. the identical recurrence over this repo's
// own Memoizer caching that count, the shape DivisorGameBenchmarks and
// StoneGameIIIBenchmarks already use. StoneCount is kept modest for the same reason
// those benchmarks document: the un-memoized baseline's blowup is real (branching
// factor sqrt(StoneCount), wider than DivisorGame's or StoneGameIII's own branching).
[MemoryDiagnoser]
public class StoneGameIVBenchmarks
{
    [Params(16, 20)]
    public int StoneCount { get; set; }

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByUnmemoizedRecursion() =>
        StoneGameIVSolution.CanAliceWinByUnmemoizedRecursion(StoneCount);

    [Benchmark]
    public bool CanAliceWinByMemoizedRecursion() =>
        StoneGameIVSolution.CanAliceWinByMemoizedRecursion(StoneCount);
}
