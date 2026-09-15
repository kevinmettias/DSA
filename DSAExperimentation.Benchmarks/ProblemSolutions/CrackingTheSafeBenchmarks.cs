using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CrackingTheSafe;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CrackingTheSafeSolution's, the same methods
// CrackingTheSafeTests proves correct. K is fixed at 2 (binary passwords, the
// problem's own running example) and N is [Params]-scaled - the search space grows as
// 2^N, so N stays small (2, 3) the same way SudokuSolverBenchmarks/NQueensBenchmarks
// keep their fixed-size inputs small enough for a reasonable iteration budget while
// still isolating the constant-factor cost of Backtrack.TrySearch's generic delegate
// dispatch from an equivalent purpose-built recursion.
[MemoryDiagnoser]
public class CrackingTheSafeBenchmarks
{
    private const int K = 2;

    [Params(2, 3)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public string GreedyRecursion() => CrackingTheSafeSolution.CrackSafeByGreedyRecursion(N, K);

    [Benchmark]
    public string BacktrackEngine() => CrackingTheSafeSolution.CrackSafeByBacktrackEngine(N, K);
}
