using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ContainsDuplicateII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContainsDuplicateIISolution's, the same methods
// ContainsDuplicateIITests proves correct. Values are a random permutation of
// distinct integers, so no duplicate ever exists and both strategies are forced
// through their full worst-case scan instead of an early exit making brute force
// look artificially competitive.
[MemoryDiagnoser]
public class ContainsDuplicateIIBenchmarks
{
    private const int WindowK = 10;
    private const int Seed = 219;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HasNearbyDuplicateByBruteForce() => ContainsDuplicateIISolution.HasNearbyDuplicateByBruteForce(_nums, WindowK);

    [Benchmark]
    public bool HasNearbyDuplicateByHashMap() => ContainsDuplicateIISolution.HasNearbyDuplicateByHashMap(_nums, WindowK);
}
