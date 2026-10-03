using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumScoreTriangulationOfPolygon;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumScoreTriangulationOfPolygonSolution's, the same
// methods MinimumScoreTriangulationOfPolygonSolutionTests proves correct - plain un-memoized
// interval recursion over (left, right) vertex-index pairs, exponential because the
// same sub-polygon recurs across many different choices of apex outside it, vs. this
// repo's own Memoizer<TState,TResult> caching that exact pair.
//
// Sizes are per arm, as in BurstBalloonsBenchmarks: the un-memoized baseline's blowup
// is real, so it stops at 14 vertices, while the memoized arm's O(n^3) runs on to
// LC 1039's own bound of 50. The two are compared at the sizes both run.
public class MinimumScoreTriangulationOfPolygonBenchmarks
{
    private const int RandomSeed = 1;

    private Dictionary<int, int[]> _valuesBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 25, 50];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _valuesBySize = MemoizedSizes.ToDictionary(
            vertexCount => vertexCount,
            vertexCount => PolygonTriangulationWorkloads.BuildVertexWeights(vertexCount, seed: RandomSeed));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int UnmemoizedRecursion(int vertexCount) =>
        MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByUnmemoizedRecursion(
            _valuesBySize[vertexCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int vertexCount) =>
        MinimumScoreTriangulationOfPolygonSolution.MinScoreTriangulationByMemoizedRecursion(
            _valuesBySize[vertexCount]);
}
