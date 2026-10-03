using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostTreeFromLeafValues;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostTreeFromLeafValuesSolution's, the same
// methods MinimumCostTreeFromLeafValuesSolutionTests proves correct - the un-memoized
// interval recursion vs. the O(n) monotonic-decreasing sweep. Length is kept modest
// for the same reason MinimumScoreTriangulationOfPolygonBenchmarks' baseline sizes are:
// the baseline's blowup is real. Leaf values are drawn from LC 1130's own [1, 15].
public class MinimumCostTreeFromLeafValuesBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1130;

    private const int MaxLeafValueExclusive = 16;

    private int[] _arr = [];

    [Params(10, 14)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = SeededDraws.Values(Length, 1, MaxLeafValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        MinimumCostTreeFromLeafValuesSolution
            .MinimumCostTreeFromLeafValuesByUnmemoizedRecursion(_arr);

    [Benchmark]
    public int MonotonicStack() =>
        MinimumCostTreeFromLeafValuesSolution
            .MinimumCostTreeFromLeafValuesByMonotonicStack(_arr);
}
