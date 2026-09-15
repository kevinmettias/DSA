using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GasStation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GasStationSolution's, the same methods
// GasStationTests proves correct. _gas/_cost are built so a solution always
// exists but sits at the very end of the array, forcing the brute-force
// baseline through nearly all of its O(n^2) simulated laps instead of
// succeeding on an early candidate start.
[MemoryDiagnoser]
public class GasStationBenchmarks
{
    private const int RandomSeed = 134; // LC problem number
    private const int MaxStationAmount = 10;

    private int[] _gas = [];

    private int[] _cost = [];
    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        FillWithRandomAmounts();

        // Guarantee a solution exists (total gas >= total cost) without moving it to
        // index 0, so brute force can't short-circuit on its very first candidate.
        var deficit = _cost.Sum() - _gas.Sum();
        _gas[^1] += Math.Max(deficit, 0) + 1;
    }

    // One draw for gas and one for cost per station, in that order, off a single
    // seeded generator - the exact sequence the benchmarks are pinned to.
    private void FillWithRandomAmounts()
    {
        var random = new Random(RandomSeed);
        _gas = new int[Length];
        _cost = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _gas[i] = random.Next(1, MaxStationAmount);
            _cost[i] = random.Next(1, MaxStationAmount);
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSimulateEveryStart() => GasStationSolution.CanCompleteCircuitByBruteForceSimulation(_gas, _cost);

    [Benchmark]
    public int GreedyDebtReset() => GasStationSolution.CanCompleteCircuitByGreedyDebtReset(_gas, _cost);
}
