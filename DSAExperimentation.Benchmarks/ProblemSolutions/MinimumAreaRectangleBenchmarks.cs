using BenchmarkDotNet.Attributes;
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

    [Params(60, 400)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var grid = (int)Math.Ceiling(Math.Sqrt(Length)) + GridPadding;

        var coordinates = new HashSet<(int X, int Y)>();

        while (coordinates.Count < Length)
        {
            coordinates.Add((random.Next(grid), random.Next(grid)));
        }

        _points = coordinates.Select(c => new[] { c.X, c.Y }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanLookup() => MinimumAreaRectangleSolution.MinAreaRectByLinearScan(_points);

    [Benchmark]
    public int SetLookup() => MinimumAreaRectangleSolution.MinAreaRectBySetLookup(_points);
}
