using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountNumberOfTrapezoidsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNumberOfTrapezoidsISolution's, the same
// methods CountNumberOfTrapezoidsITests proves correct
// (MaximizeSubarrayGCDScoreBenchmarks precedent). PointCount stays small for
// the brute-force arm - C(n, 4) already reaches into the millions past a few
// dozen points - while the grouped-by-y arm scales to the real problem's n up
// to 10^5 trivially. Points are drawn from a small y range so repeated
// y-values - and therefore horizontal sides - actually occur.
[MemoryDiagnoser]
public class CountNumberOfTrapezoidsIBenchmarks
{
    private const int Seed = 3623; // LC problem number
    private const int YBucketCount = 20;
    private const int CoordinateBound = 1000;

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
                x = random.Next(CoordinateBound);
                y = random.Next(YBucketCount);
            }
            while (!seen.Add((x, y)));

            points[i] = [x, y];
        }

        _points = points;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountNumberOfTrapezoidsISolution.CountTrapezoidsByBruteForce(_points);

    [Benchmark]
    public int HorizontalPairCounting() =>
        CountNumberOfTrapezoidsISolution.CountTrapezoidsByHorizontalPairCounting(_points);
}
