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

    [Params(50, 400)]
    public int CircleCount;

    private int[][] _circles = null!;

    [GlobalSetup]
    public void Setup() => _circles = RectangleCornerWorkloads.BuildCircles(CircleCount, Seed);

    [Benchmark(Baseline = true)]
    public bool BoundaryFloodFill() => CheckIfTheRectangleCornerIsReachableSolution.IsReachableByBoundaryFloodFill(
        RectangleCornerWorkloads.XCorner, RectangleCornerWorkloads.YCorner, _circles);

    [Benchmark]
    public bool DisjointSet() => CheckIfTheRectangleCornerIsReachableSolution.IsReachableByDisjointSet(
        RectangleCornerWorkloads.XCorner, RectangleCornerWorkloads.YCorner, _circles);
}
