using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountNumberOfTrapezoidsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNumberOfTrapezoidsIISolution's, the same
// methods CountNumberOfTrapezoidsIITests proves correct
// (CountNumberOfTrapezoidsIBenchmarks precedent). PointCount stays small for
// the brute-force arm - C(n, 4) already reaches into the millions past a few
// dozen points - while the parallel-segment-counting arm scales to the real
// problem's n up to 500 comfortably at O(n^2). Points are drawn from a small
// coordinate range so repeated slopes - and therefore parallel sides - reliably
// occur.
[MemoryDiagnoser]
public class CountNumberOfTrapezoidsIIBenchmarks
{
    private const int Seed = 3625; // LC problem number
    private const int CoordinateBound = 20;

    [Params(30, 80)]
    public int PointCount;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var points = new int[PointCount][];
        var seen = new HashSet<(int X, int Y)>();

        for (var i = 0; i < PointCount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(-CoordinateBound, CoordinateBound);
                y = random.Next(-CoordinateBound, CoordinateBound);
            }
            while (!seen.Add((x, y)));

            points[i] = [x, y];
        }

        _points = points;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountNumberOfTrapezoidsIISolution.CountTrapezoidsByBruteForce(_points);

    [Benchmark]
    public int ParallelSegmentCounting() =>
        CountNumberOfTrapezoidsIISolution.CountTrapezoidsByParallelSegmentCounting(_points);
}
