using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LastStoneWeightII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LastStoneWeightIISolution's. Random weights in 1..99
// keep the half-capacity large enough that the O(stones.Length * half) recurrence is
// what is being measured, from opposite directions.
[MemoryDiagnoser]
public class LastStoneWeightIIBenchmarks
{
    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1049;

    // Exclusive upper bound for the random stone weights: weights are 1..100.
    private const int StoneWeightUpperBoundExclusive = 100;

    [Params(30, 200)]
    public int Length;

    private int[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = Enumerable.Range(0, Length).Select(_ => random.Next(1, StoneWeightUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int Tabulation() => LastStoneWeightIISolution.MinWeightByTabulation(_stones);

    [Benchmark]
    public int Memoized() => LastStoneWeightIISolution.MinWeightByMemoizer(_stones);
}
