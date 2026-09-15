using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimizeManhattanDistances;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimizeManhattanDistancesSolution's, the same
// methods MinimizeManhattanDistancesTests proves correct. The transform arm is
// handed the already-sorted u/v arrays, so the O(n log n) sort is charged to
// [GlobalSetup] and only the O(n) per-removal sweep is measured.
[MemoryDiagnoser]
public class MinimizeManhattanDistancesBenchmarks
{
    private const int Seed = 3102;
    private const int CoordinateBound = 1_000_000;

    private int[][] _points = [];

    private (long Value, int PointIndex)[] _sortedByU = [];
    private (long Value, int PointIndex)[] _sortedByV = [];
    // BruteForce is O(n^3) - it is the arm ManhattanTransform has to justify itself
    // against, so PointCount stays small enough for that cubic scan to stay
    // reasonable rather than growing to sizes only the composed strategy could
    // finish.
    [Params(50, 200)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _points = Enumerable.Range(0, PointCount)
            .Select(_ => new[] { random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound) })
            .ToArray();

        var (sortedByU, sortedByV) = MinimizeManhattanDistancesSolution.BuildSortedTransforms(_points);
        _sortedByU = sortedByU;
        _sortedByV = sortedByV;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MinimizeManhattanDistancesSolution.MinDistanceByBruteForce(_points);

    [Benchmark]
    public int ManhattanTransform() =>
        MinimizeManhattanDistancesSolution.MinDistanceByManhattanTransform(_sortedByU, _sortedByV);
}
