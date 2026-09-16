using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumAreaRectangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumAreaRectangleSolution's, the same methods
// MinimumAreaRectangleTests proves correct. The points are drawn from a grid barely
// larger than the point count, so rectangles are plentiful and both arms do real
// corner-confirmation work. Point construction is charged to [GlobalSetup].
[MemoryDiagnoser]
public class MinimumAreaRectangleBenchmarks
{
    private const int RandomSeed = 939; // LC 939
    private const int GridPadding = 2;

    private int[][] _points = [];

    [Params(60, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _points = LatticePointWorkloads.InGrid(Length, GridPadding, RandomSeed);

    [Benchmark(Baseline = true)]
    public int LinearScanLookup() => MinimumAreaRectangleSolution.MinAreaRectByLinearScan(_points);

    [Benchmark]
    public int SetLookup() => MinimumAreaRectangleSolution.MinAreaRectBySetLookup(_points);
}
