using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumNumberOfCoinsForFruits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfCoinsForFruitsSolution's, the same
// methods MinimumNumberOfCoinsForFruitsSolutionTests proves correct. Random prices avoid
// the window collapsing to a fixed small shape, so the segment-tree strategy's
// O(log n) queries are actually exercised against the brute force's O(window)
// rescans.
public class MinimumNumberOfCoinsForFruitsBenchmarks
{
    private const int MaxPriceExclusive = 1_000;
    private const int Seed = 2944;

    private int[] _prices = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _prices = SeededDraws.Values(Length, 1, MaxPriceExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDp() => MinimumNumberOfCoinsForFruitsSolution.MinCoinsByBruteForceDp(_prices);

    [Benchmark]
    public int SegmentTreeDp() => MinimumNumberOfCoinsForFruitsSolution.MinCoinsBySegmentTreeDp(_prices);
}
