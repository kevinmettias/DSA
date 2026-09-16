using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HouseRobberIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberIVSolution's, the same methods
// HouseRobberIVTests proves correct - one sweeping every candidate capability in
// O(range * n), the other bisecting the same monotone predicate in
// O(n * log range).
[MemoryDiagnoser]
public class HouseRobberIVBenchmarks
{
    private const int RandomSeed = 2560; // LeetCode problem number
    private const int MaxValueExclusive = 2_000;
    private const int RequiredHouseCountDivisor = 4;

    private int[] _nums = [];

    private int _requiredHouseCount;
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _requiredHouseCount = (Length / RequiredHouseCountDivisor) + 1;
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => HouseRobberIVSolution.MinCapabilityByLinearScan(_nums, _requiredHouseCount);

    [Benchmark]
    public int SequenceLowerBound() => HouseRobberIVSolution.MinCapabilityBySequenceLowerBound(_nums, _requiredHouseCount);
}
