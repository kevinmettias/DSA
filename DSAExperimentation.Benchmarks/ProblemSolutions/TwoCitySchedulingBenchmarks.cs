using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TwoCityScheduling;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoCitySchedulingSolution's, the same methods
// TwoCitySchedulingTests proves correct. Each is handed the already-projected people
// its hoisted overload takes, so building the workload is charged to [GlobalSetup]
// rather than to the greedy being measured.
[MemoryDiagnoser]
public class TwoCitySchedulingBenchmarks
{
    private const int RandomSeed = 1029; // LC problem number
    private const int MaxCost = 1_000;

    private (int ACost, int BCost)[] _people = [];

    // Kept even: costs.Length must be 2n per LC 1029's own constraint.
    [Params(200, 4_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _people = Enumerable.Range(0, Length)
            .Select(_ => (ACost: random.Next(1, MaxCost), BCost: random.Next(1, MaxCost)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArraySortGreedy() => TwoCitySchedulingSolution.TwoCityCostMinimumByArraySortGreedy(_people);

    [Benchmark]
    public int MergeSortGreedy() => TwoCitySchedulingSolution.TwoCityCostMinimumByMergeSortGreedy(_people);
}
