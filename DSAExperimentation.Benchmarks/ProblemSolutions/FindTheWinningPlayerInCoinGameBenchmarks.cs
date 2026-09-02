using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheWinningPlayerInCoinGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheWinningPlayerInCoinGameSolution's, the same
// methods FindTheWinningPlayerInCoinGameTests proves correct. No [GlobalSetup]
// is needed - x and y are the entire input, LC's own constraint (1 <= x, y <=
// 100), and BenchmarkDotNet feeds them straight to each [Benchmark] call.
[MemoryDiagnoser]
public class FindTheWinningPlayerInCoinGameBenchmarks
{
    [Params(1, 100)]
    public int X;

    [Params(4, 100)]
    public int Y;

    [Benchmark(Baseline = true)]
    public string Simulation() => FindTheWinningPlayerInCoinGameSolution.WinningPlayerBySimulation(X, Y);

    [Benchmark]
    public string TurnParity() => FindTheWinningPlayerInCoinGameSolution.WinningPlayerByTurnParity(X, Y);
}
