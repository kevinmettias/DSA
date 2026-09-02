using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountAlmostEqualPairsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountAlmostEqualPairsIISolution's, the same methods
// CountAlmostEqualPairsIITests proves correct. Zero-padding every number to
// PaddedWidth is charged to [GlobalSetup] via the solution's own Pad, not to either
// measured pairwise scan.
[MemoryDiagnoser]
public class CountAlmostEqualPairsIIBenchmarks
{
    private const int MinValueInclusive = 1;
    private const int MaxValueExclusive = 10_000_000; // LC's own nums[i] < 1e7 bound
    private const int Seed = 3267;

    [Params(10, 30)]
    public int Length;

    private string[] _padded = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var nums = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
        _padded = CountAlmostEqualPairsIISolution.Pad(nums);
    }

    [Benchmark(Baseline = true)]
    public long BoundedSwapBruteForce() => CountAlmostEqualPairsIISolution.CountByBoundedSwapBruteForce(_padded);

    [Benchmark]
    public long BoundedSwapBacktrack() => CountAlmostEqualPairsIISolution.CountByBoundedSwapBacktrack(_padded);
}
