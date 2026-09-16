using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfTheRectangleCornerIsReachable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfTheRectangleCornerIsReachableSolution's, the same
// methods CheckIfTheRectangleCornerIsReachableTests proves correct.
[MemoryDiagnoser]
public class CheckIfTheRectangleCornerIsReachableBenchmarks
{
    private const int Seed = 3235;

    private int[][] _circles = [];

    [Params(50, 400)]
    public int CircleCount { get; set; }

    [GlobalSetup]
    public void Setup() => _circles = RectangleCornerWorkloads.BuildCircles(CircleCount, Seed);

    [Benchmark(Baseline = true)]
    public bool IsReachableByBoundaryFloodFill() => CheckIfTheRectangleCornerIsReachableSolution.IsReachableByBoundaryFloodFill(
        RectangleCornerScenario.XCorner, RectangleCornerScenario.YCorner, _circles);

    [Benchmark]
    public bool IsReachableByDisjointSet() => CheckIfTheRectangleCornerIsReachableSolution.IsReachableByDisjointSet(
        RectangleCornerScenario.XCorner, RectangleCornerScenario.YCorner, _circles);
}
