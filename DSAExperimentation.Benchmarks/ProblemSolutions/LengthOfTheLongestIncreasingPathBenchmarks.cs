using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LengthOfTheLongestIncreasingPath;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LengthOfTheLongestIncreasingPathSolution's, the same
// methods LengthOfTheLongestIncreasingPathTests proves correct. BruteForce is O(n^2),
// so PointCount stays modest enough for it to still finish - SegmentTreeSweep is the
// O(n log n) arm this problem's own 10^5 bound actually needs.
[MemoryDiagnoser]
public class LengthOfTheLongestIncreasingPathBenchmarks
{
    private const int RandomSeed = 3288; // LeetCode problem number
    private const int CoordinateBound = 1_000_000;

    private int[][] _coordinates = [];

    private int _k;
    [Params(200, 1_000)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<(int X, int Y)>();

        while (seen.Count < PointCount)
        {
            seen.Add((random.Next(CoordinateBound), random.Next(CoordinateBound)));
        }

        _coordinates = seen.Select(p => new[] { p.X, p.Y }).ToArray();
        _k = PointCount / 2;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => LengthOfTheLongestIncreasingPathSolution.MaxPathLengthByBruteForce(_coordinates, _k);

    [Benchmark]
    public int SegmentTreeSweep() => LengthOfTheLongestIncreasingPathSolution.MaxPathLengthBySegmentTree(_coordinates, _k);
}
