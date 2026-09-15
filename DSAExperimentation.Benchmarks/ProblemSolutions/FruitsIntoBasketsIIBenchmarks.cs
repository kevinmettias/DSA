using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FruitsIntoBasketsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is FruitsIntoBasketsIISolution's, the same
// method FruitsIntoBasketsIITests proves correct. LC caps n at 100, so
// Length stays inside that bound - this measures the O(n^2) rescan at its
// own intended scale rather than one this problem was never meant to run at
// (see FruitsIntoBasketsIIIBenchmarks for the n <= 1e5 sibling).
[MemoryDiagnoser]
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
        _fruits = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxCapacityExclusive)).ToArray();
        _baskets = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxCapacityExclusive)).ToArray();
    }

    [Benchmark]
    public int BruteForce() => FruitsIntoBasketsIISolution.CountUnplacedByBruteForce(_fruits, _baskets);
}
