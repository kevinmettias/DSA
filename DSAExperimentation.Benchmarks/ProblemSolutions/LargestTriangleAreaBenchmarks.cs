using DSAExperimentation.LeetCode.LargestTriangleArea;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestTriangleAreaSolution's, the same methods
// LargestTriangleAreaSolutionTests proves correct. Points are drawn uniformly from
// LC 812's [-50, 50] square, so most of them land strictly inside the hull and the
// reducing arm discards them before the cubic step ever sees them. Length stops at
// LC 812's 50-point cap.
public class LargestTriangleAreaBenchmarks
{
    // LC problem number, reused as the deterministic point seed.
    private const int RandomSeed = 812;
    private const int CoordinateLowerBound = -50;
    private const int CoordinateUpperBound = 51;

    private (int X, int Y)[] _points = [];

    [Params(5, 50)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<(int X, int Y)>();

        while (seen.Count < Length)
        {
            var x = random.Next(CoordinateLowerBound, CoordinateUpperBound);
            var y = random.Next(CoordinateLowerBound, CoordinateUpperBound);
            seen.Add((x, y));
        }

        _points = [.. seen];
    }

    [Benchmark(Baseline = true)]
    public double BruteForceAllTriples() =>
        LargestTriangleAreaSolution.LargestAreaByAllTriples(_points);

    [Benchmark]
    public double ConvexHullReduction() =>
        LargestTriangleAreaSolution.LargestAreaByConvexHullReduction(_points);
}
