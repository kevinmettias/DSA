using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FruitsIntoBasketsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is FruitsIntoBasketsIISolution's, the same
// method FruitsIntoBasketsIISolutionTests proves correct. LC caps n at 100, so
// Length stays inside that bound - this measures the O(n^2) rescan at its
// own intended scale rather than one this problem was never meant to run at
// (see FruitsIntoBasketsIIIBenchmarks for the n <= 1e5 sibling).
public class FruitsIntoBasketsIIBenchmarks
{
    private const int Seed = 3477;
    private const int MaxCapacityExclusive = 1_000;

    private int[] _fruits = [];

    private int[] _baskets = [];
    [Params(20, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _fruits = SeededDraws.Values(Length, 1, MaxCapacityExclusive, random);
        _baskets = SeededDraws.Values(Length, 1, MaxCapacityExclusive, random);
    }

    [Benchmark]
    public int BruteForce() => FruitsIntoBasketsIISolution.CountUnplacedByBruteForce(_fruits, _baskets);
}
