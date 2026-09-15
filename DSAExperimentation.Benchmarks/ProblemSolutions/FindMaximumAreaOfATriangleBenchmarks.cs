using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindMaximumAreaOfATriangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMaximumAreaOfATriangleSolution's, the same
// methods FindMaximumAreaOfATriangleTests proves correct. Coordinates are drawn
// from a grid narrower than the point count (LargestTriangleAreaBenchmarks' own
// point-generation precedent, tightened here) so rows and columns collide and
// both arms have real axis-aligned triangles to score, not just -1 every run.
[MemoryDiagnoser]
public class FindMaximumAreaOfATriangleBenchmarks
{
    private const int Seed = 3588;

    private int[][] _coords = [];

    [Params(60, 300)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var bound = Math.Max(10, PointCount / 3);
        var seen = new HashSet<(int X, int Y)>();

        while (seen.Count < PointCount)
        {
            seen.Add((random.Next(1, bound), random.Next(1, bound)));
        }

        _coords = seen.Select(point => new[] { point.X, point.Y }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => FindMaximumAreaOfATriangleSolution.MaxAreaByBruteForce(_coords);

    [Benchmark]
    public long SpreadHashMap() => FindMaximumAreaOfATriangleSolution.MaxAreaBySpreadHashMap(_coords);
}
