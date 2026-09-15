using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumScoreTriangulationOfPolygon;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumScoreTriangulationOfPolygonSolution's, the same
// methods MinimumScoreTriangulationOfPolygonTests proves correct - plain un-memoized
// interval recursion over (left, right) vertex-index pairs, exponential because the
// same sub-polygon recurs across many different choices of apex outside it, vs. this
// repo's own Memoizer<TState,TResult> caching that exact pair. VertexCount is kept
// modest (<=14) for the same reason BurstBalloonsBenchmarks' UnmemoizedRecursion is:
// the un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class MinimumScoreTriangulationOfPolygonBenchmarks
{
    private const int RandomSeed = 1;

    private int[] _values = [];

    [Params(10, 14)]
    public int VertexCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = PolygonTriangulationWorkloads.BuildVertexWeights(VertexCount, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByUnmemoizedRecursion(_values);

    [Benchmark]
    public int MemoizedRecursion() =>
        MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByMemoizedRecursion(_values);
}
