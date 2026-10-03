using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.GasStation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GasStationSolution's, the same methods
// GasStationSolutionTests proves correct. GasStationWorkloads builds _gas/_cost so the
// only station a circuit can start from is the last one - the answer exists and is
// unique, as LC 134 guarantees - forcing the brute-force baseline through nearly all
// of its O(n^2) simulated laps instead of succeeding on an early candidate start.
public class GasStationBenchmarks
{
    private const int RandomSeed = 134; // LC problem number

    private int[] _gas = [];

    private int[] _cost = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => (_gas, _cost) = GasStationWorkloads.Build(Length, RandomSeed);

    [Benchmark(Baseline = true)]
    public int BruteForceSimulateEveryStart() => GasStationSolution.CanCompleteCircuitByBruteForceSimulation(_gas, _cost);

    [Benchmark]
    public int GreedyDebtReset() => GasStationSolution.CanCompleteCircuitByGreedyDebtReset(_gas, _cost);
}
