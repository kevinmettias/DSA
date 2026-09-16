using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.EliminationGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EliminationGameSolution's, the same methods
// EliminationGameTests proves correct.
[MemoryDiagnoser]
public class EliminationGameBenchmarks
{
    [Params(10_000, 1_000_000)]
    public int NumberCount { get; set; }

    [Benchmark(Baseline = true)]
    public int ListSimulation() => EliminationGameSolution.LastRemainingByListSimulation(NumberCount);

    [Benchmark]
    public int HeadStepArithmetic() => EliminationGameSolution.LastRemainingByHeadStepArithmetic(NumberCount);
}
