using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheWinningPlayerInCoinGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheWinningPlayerInCoinGameSolution's, the same
// methods FindTheWinningPlayerInCoinGameTests proves correct. No [GlobalSetup]
// is needed - the two coin counts are the entire input and each lies in LC's own
// 1..100 range, and BenchmarkDotNet feeds them straight to each [Benchmark] call.
[MemoryDiagnoser]
public class FindTheWinningPlayerInCoinGameBenchmarks
{
    [Params(1, 100)]
    public int SeventyFiveCoinCount { get; set; }

    [Params(4, 100)]
    public int TenCoinCount { get; set; }

    [Benchmark(Baseline = true)]
    public string Simulation() =>
        FindTheWinningPlayerInCoinGameSolution.WinningPlayerBySimulation(SeventyFiveCoinCount, TenCoinCount);

    [Benchmark]
    public string TurnParity() =>
        FindTheWinningPlayerInCoinGameSolution.WinningPlayerByTurnParity(SeventyFiveCoinCount, TenCoinCount);
}
