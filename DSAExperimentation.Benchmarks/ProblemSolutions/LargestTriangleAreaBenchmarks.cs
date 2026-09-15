using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LargestTriangleArea;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestTriangleAreaSolution's, the same methods
// LargestTriangleAreaTests proves correct. Points are drawn uniformly from a
// bounded square, so almost all of them land strictly inside the hull and the
// reducing arm discards them before the cubic step ever sees them.
[MemoryDiagnoser]
public class LargestTriangleAreaBenchmarks
{
    // LC problem number, reused as the deterministic point seed.
    private const int RandomSeed = 812;
    private const int CoordinateUpperBound = 1_000;

    private (int X, int Y)[] _points = [];

    [Params(60, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<(int X, int Y)>();

        while (seen.Count < Length)
        {
            seen.Add((random.Next(0, CoordinateUpperBound), random.Next(0, CoordinateUpperBound)));
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
