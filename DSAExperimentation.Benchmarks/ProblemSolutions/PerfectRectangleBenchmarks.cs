using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PerfectRectangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PerfectRectangleSolution's, the same methods
// PerfectRectangleTests proves correct. The input is a genuine GridSize x GridSize
// unit-square tiling - a real perfect cover, not a rejected one - so neither
// strategy short-circuits early on a detected overlap and both run their full
// worst-case pass.
[MemoryDiagnoser]
public class PerfectRectangleBenchmarks
{
    private int[][] _rectangles = [];

    [Params(20, 150)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rectangles = new List<int[]>();

        for (var x = 0; x < GridSize; x++)
        {
            for (var y = 0; y < GridSize; y++)
            {
                rectangles.Add([x, y, x + 1, y + 1]);
            }
        }

        _rectangles = rectangles.ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool IsRectangleCoverByPairwiseOverlap() =>
        PerfectRectangleSolution.IsRectangleCoverByPairwiseOverlap(_rectangles);

    [Benchmark]
    public bool IsRectangleCoverByCornerToggle() =>
        PerfectRectangleSolution.IsRectangleCoverByCornerToggle(_rectangles);
}
