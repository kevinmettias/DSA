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
    private const int KDivisor = 4;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _k = (Length / KDivisor) + 1;
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => HouseRobberIVSolution.MinCapabilityByLinearScan(_nums, _k);

    [Benchmark]
    public int SequenceLowerBound() => HouseRobberIVSolution.MinCapabilityBySequenceLowerBound(_nums, _k);
}
