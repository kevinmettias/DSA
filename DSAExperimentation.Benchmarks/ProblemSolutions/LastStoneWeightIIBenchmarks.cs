using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LastStoneWeightII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LastStoneWeightIISolution's. Random weights in 1..99
// keep the half-capacity large enough that the O(stones.Length * half) recurrence is
// what is being measured, from opposite directions. Length stops at LC 1049's
// 30-stone cap.
public class LastStoneWeightIIBenchmarks
{
    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1049;

    // Exclusive upper bound for the random stone weights: weights are 1..100.
    private const int StoneWeightUpperBoundExclusive = 100;

    private int[] _stones = [];

    [Params(3, 30)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = SeededDraws.Values(Length, 1, StoneWeightUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int Tabulation() => LastStoneWeightIISolution.MinWeightByTabulation(_stones);

    [Benchmark]
    public int Memoized() => LastStoneWeightIISolution.MinWeightByMemoizer(_stones);
}
