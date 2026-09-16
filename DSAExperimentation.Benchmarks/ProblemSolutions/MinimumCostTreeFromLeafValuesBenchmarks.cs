using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumCostTreeFromLeafValues;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostTreeFromLeafValuesSolution's, the same
// methods MinimumCostTreeFromLeafValuesTests proves correct - the un-memoized
// interval recursion vs. the O(n) monotonic-decreasing sweep. Length is kept modest
// for the same reason MinimumScoreTriangulationOfPolygonBenchmarks' VertexCount is:
// the baseline's blowup is real.
[MemoryDiagnoser]
public class MinimumCostTreeFromLeafValuesBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1130;

    private const int MaxLeafValueExclusive = 100;

    private int[] _arr = [];

    [Params(10, 14)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxLeafValueExclusive)).ToArray();
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
