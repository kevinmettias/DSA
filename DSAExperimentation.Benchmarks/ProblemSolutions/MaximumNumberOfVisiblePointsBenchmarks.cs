using DSAExperimentation.LeetCode.MaximumNumberOfVisiblePoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfVisiblePointsSolution's, the same
// methods MaximumNumberOfVisiblePointsSolutionTests proves correct - the canonical O(n^2)
// brute force against sort-then-slide, which uses this repo's own
// Algorithms.Sorting.MergeSort once and then a single O(n) two-pointer sweep over the
// angle-doubled array. _points is generated so none land exactly on Location (that
// case short-circuits to O(1) and would understate both strategies' real windowed-
// comparison cost); [GlobalSetup] owns that construction. Every coordinate stays in
// LC 1610's [0, 100], with Location at the centre so points surround it on all sides.
public class MaximumNumberOfVisiblePointsBenchmarks
{
    private const int Angle = 30;
    private const int MaxCoordinate = 100;
    private const int Center = MaxCoordinate / 2;
    private const int RandomSeed = 1;
    private static readonly int[] Location = [Center, Center];

    private int[][] _points = [];

    [Params(200, 2_000)]
    public int PointCount { get; set; }

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
                x = random.Next(0, MaxCoordinate + 1);
                y = random.Next(0, MaxCoordinate + 1);
            } while (x == Center && y == Center);

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
