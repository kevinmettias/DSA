using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfVisiblePoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfVisiblePointsSolution's, the same
// methods MaximumNumberOfVisiblePointsTests proves correct - the canonical O(n^2)
// brute force against sort-then-slide, which uses this repo's own
// Algorithms.Sorting.MergeSort once and then a single O(n) two-pointer sweep over the
// angle-doubled array. _points is generated so none land exactly on Location (that
// case short-circuits to O(1) and would understate both strategies' real windowed-
// comparison cost); [GlobalSetup] owns that construction.
[MemoryDiagnoser]
public class MaximumNumberOfVisiblePointsBenchmarks
{
    private const int Angle = 30;
    private const int CoordinateBound = 1_000;
    private const int RandomSeed = 1;
    private static readonly int[] Location = [0, 0];

    [Params(200, 2_000)]
    public int PointCount;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = new int[PointCount][];

        for (var i = 0; i < PointCount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(-CoordinateBound, CoordinateBound);
                y = random.Next(-CoordinateBound, CoordinateBound);
            } while (x == 0 && y == 0);

            _points[i] = [x, y];
        }
    }

    [Benchmark(Baseline = true)]
    public int PairwiseBruteForce() =>
        MaximumNumberOfVisiblePointsSolution.VisiblePointsByPairwiseBruteForce(_points, Angle, Location);

    [Benchmark]
    public int SortAndSlideWindow() =>
        MaximumNumberOfVisiblePointsSolution.VisiblePointsBySortAndSlideWindow(_points, Angle, Location);
}
