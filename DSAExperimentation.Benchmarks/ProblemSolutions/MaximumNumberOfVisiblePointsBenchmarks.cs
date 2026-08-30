using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Visible Points (LC 1610): the canonical O(n^2) brute force
// (every point tried as the anchor, compared pairwise against every other point's
// circular angular distance) vs. sort-then-slide - this repo's own
// Algorithms.Sorting.MergeSort (SortAnArrayBenchmarks/MaximumNumberOfVisiblePoints
// Tests precedent) sorting the angles once, then a single O(n) two-pointer sweep
// over the angle-doubled array to linearize the 360-degree wraparound instead of
// the brute force's per-pair modular distance check. _points is generated so none
// land exactly on _location (that case short-circuits to O(1) and would understate
// both strategies' real windowed-comparison cost).
[MemoryDiagnoser]
public class MaximumNumberOfVisiblePointsBenchmarks
{
    private const int Angle = 30;
    private static readonly int[] Location = [0, 0];

    [Params(200, 2_000)]
    public int PointCount;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _points = new int[PointCount][];

        for (var i = 0; i < PointCount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(-1_000, 1_000);
                y = random.Next(-1_000, 1_000);
            } while (x == 0 && y == 0);

            _points[i] = [x, y];
        }
    }

    [Benchmark(Baseline = true)]
    public int PairwiseBruteForce()
    {
        var angles = ToAngles();
        var best = 0;

        for (var i = 0; i < angles.Length; i++)
        {
            var count = 0;

            for (var j = 0; j < angles.Length; j++)
            {
                var diff = Math.Abs(angles[i] - angles[j]);
                diff = Math.Min(diff, 360.0 - diff);

                if (diff <= Angle + 1e-9)
                {
                    count++;
                }
            }

            best = Math.Max(best, count);
        }

        return best;
    }

    [Benchmark]
    public int SortAndSlideWindow()
    {
        var angles = ToAngles();
        MergeSort.Sort<double, ArrayIndexedSequence<double>>(new ArrayIndexedSequence<double>(angles));

        var doubled = new double[angles.Length * 2];

        for (var i = 0; i < angles.Length; i++)
        {
            doubled[i] = angles[i];
            doubled[i + angles.Length] = angles[i] + 360.0;
        }

        var best = 0;
        var left = 0;

        for (var right = 0; right < doubled.Length; right++)
        {
            while (doubled[right] - doubled[left] > Angle + 1e-9)
            {
                left++;
            }

            best = Math.Max(best, right - left + 1);
        }

        return Math.Min(best, angles.Length);
    }

    private double[] ToAngles()
    {
        var angles = new double[_points.Length];

        for (var i = 0; i < _points.Length; i++)
        {
            angles[i] = Math.Atan2(_points[i][1] - Location[1], _points[i][0] - Location[0]) * 180.0 / Math.PI;
        }

        return angles;
    }
}
