using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Lines to Represent a Line Chart (LC 2280): ArraySortFloatingSlope is the naive
// approach most people reach for first - the BCL's Array.Sort plus floating-point division to
// compare consecutive slopes. MergeSortIntegerSlope instead sorts with this repo's own MergeSort
// over ArrayIndexedSequence (same composition ArrayPartitionBenchmarks/
// MinimumLinesToRepresentALineChartTests already use) and compares slopes via the cross-product
// form, avoiding both the division and any floating-point precision loss.
[MemoryDiagnoser]
public class MinimumLinesToRepresentALineChartBenchmarks
{
    private const int RandomSeed = 2280;
    private const int PriceBound = 100_000;

    // A slope comparison needs three consecutive points (the current one plus its two
    // predecessors); with fewer points than this, every point trivially sits on the same line.
    private const int SlopeComparisonWindowSize = 3;

    [Params(200, 5_000)]
    public int Length;

    private (int Day, int Price)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // A shuffled 0..Length-1 range guarantees exactly Length distinct days, so every
        // generated input actually has Length points instead of silently collapsing duplicates.
        var days = Enumerable.Range(0, Length).ToArray();
        for (var i = days.Length - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (days[i], days[swapIndex]) = (days[swapIndex], days[i]);
        }

        _points = days.Select(day => (day, random.Next(0, PriceBound))).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArraySortFloatingSlope()
    {
        var points = _points.ToArray();
        Array.Sort(points, (a, b) => a.Day.CompareTo(b.Day));

        return CountLinesFloating(points);
    }

    [Benchmark]
    public int MergeSortIntegerSlope()
    {
        var points = _points.ToArray();
        var byDay = Comparer<(int Day, int Price)>.Create((a, b) => a.Day.CompareTo(b.Day));
        MergeSort.Sort<(int Day, int Price), ArrayIndexedSequence<(int Day, int Price)>>(
            new ArrayIndexedSequence<(int Day, int Price)>(points), byDay);

        return CountLinesIntegerSlope(points);
    }

    private static int CountLinesFloating((int Day, int Price)[] points)
    {
        if (points.Length < SlopeComparisonWindowSize)
        {
            return Math.Max(points.Length - 1, 0);
        }

        var lines = 1;

        for (var i = SlopeComparisonWindowSize - 1; i < points.Length; i++)
        {
            if (FloatingSlopeChangedAt(points, i))
            {
                lines++;
            }
        }

        return lines;
    }

    private static bool FloatingSlopeChangedAt((int Day, int Price)[] points, int i)
    {
        var (x0, y0) = points[i - (SlopeComparisonWindowSize - 1)];
        var (x1, y1) = points[i - 1];
        var (x2, y2) = points[i];

        var slope1 = (double)(y1 - y0) / (x1 - x0);
        var slope2 = (double)(y2 - y1) / (x2 - x1);

        return slope1 != slope2;
    }

    private static int CountLinesIntegerSlope((int Day, int Price)[] points)
    {
        if (points.Length < SlopeComparisonWindowSize)
        {
            return Math.Max(points.Length - 1, 0);
        }

        var lines = 1;

        for (var i = SlopeComparisonWindowSize - 1; i < points.Length; i++)
        {
            var (x0, y0) = points[i - (SlopeComparisonWindowSize - 1)];
            var (x1, y1) = points[i - 1];
            var (x2, y2) = points[i];

            var collinear = (long)(y1 - y0) * (x2 - x1) == (long)(y2 - y1) * (x1 - x0);
            if (!collinear)
            {
                lines++;
            }
        }

        return lines;
    }
}
