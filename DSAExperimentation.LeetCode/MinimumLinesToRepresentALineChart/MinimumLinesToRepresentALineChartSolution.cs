using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumLinesToRepresentALineChart;

// LeetCode 2280. Minimum Lines to Represent a Line Chart: given (day, price) points,
// how few straight segments does the chart connecting them in day order need? Sort by
// day, then count how many times the slope changes between consecutive point triples;
// the answer is one line plus one per change.
//
// The two strategies differ in the sort and in how "same slope" is decided:
// ArraySortFloatingSlope is the textbook version - the BCL's Array.Sort plus a
// floating-point division per segment - while MergeSortIntegerSlope sorts with this
// repo's own MergeSort over ArrayIndexedSequence (the same composition
// ArrayPartitionSolution uses for a plain int[]) and compares slopes in the
// cross-product form (y1-y0)*(x2-x1) == (y2-y1)*(x1-x0), which involves no division and
// so loses no precision. The products are widened to long first, since day and price can
// each reach 1e9 per the problem's constraints and their product overflows int.
internal static class MinimumLinesToRepresentALineChartSolution
{
    // LeetCode hands each point in as a two-element array, [day, price].
    private const int DayIndex = 0;
    private const int PriceIndex = 1;

    // A slope comparison needs three consecutive points (the current one plus its two
    // predecessors); with fewer points than this, every point trivially sits on the same
    // line - one line for two points, none at all for a single point.
    private const int SlopeComparisonWindowSize = 3;

    // Baseline: BCL Array.Sort with a comparison delegate, then compare consecutive
    // slopes as doubles. Deliberately BCL only, division included - it is the arm the
    // composed strategy below has to justify itself against.
    public static int MinimumLinesByArraySortFloatingSlope(int[][] stockPrices)
    {
        var points = ToPointsByDay(stockPrices);
        Array.Sort(points, (a, b) => a.Day.CompareTo(b.Day));

        return CountLinesByFloatingSlope(points);
    }

    private static int CountLinesByFloatingSlope((int Day, int Price)[] points)
    {
        if (points.Length < SlopeComparisonWindowSize)
        {
            return TrivialLineCount(points.Length);
        }

        var lines = 1;

        for (var i = SlopeComparisonWindowSize - 1; i < points.Length; i++)
        {
            if (HasFloatingSlopeChangeAt(points, i))
            {
                lines++;
            }
        }

        return lines;
    }

    private static bool HasFloatingSlopeChangeAt((int Day, int Price)[] points, int endIndex)
    {
        var (x0, y0) = points[endIndex - (SlopeComparisonWindowSize - 1)];
        var (x1, y1) = points[endIndex - 1];
        var (x2, y2) = points[endIndex];

        return (double)(y1 - y0) / (x1 - x0) != (double)(y2 - y1) / (x2 - x1);
    }

    // This repo's own MergeSort over ArrayIndexedSequence, then the exact cross-product
    // collinearity test rather than a floating-point slope comparison.
    public static int MinimumLinesByMergeSortIntegerSlope(int[][] stockPrices)
    {
        var points = ToPointsByDay(stockPrices);
        var byDay = Comparer<(int Day, int Price)>.Create((a, b) => a.Day.CompareTo(b.Day));
        MergeSort.Sort<(int Day, int Price), ArrayIndexedSequence<(int Day, int Price)>>(
            new ArrayIndexedSequence<(int Day, int Price)>(points), byDay);

        return CountLinesByIntegerSlope(points);
    }

    private static int CountLinesByIntegerSlope((int Day, int Price)[] points)
    {
        if (points.Length < SlopeComparisonWindowSize)
        {
            return TrivialLineCount(points.Length);
        }

        var lines = 1;

        for (var i = SlopeComparisonWindowSize - 1; i < points.Length; i++)
        {
            if (HasCrossProductSlopeChangeAt(points, i))
            {
                lines++;
            }
        }

        return lines;
    }

    private static bool HasCrossProductSlopeChangeAt((int Day, int Price)[] points, int endIndex)
    {
        var (x0, y0) = points[endIndex - (SlopeComparisonWindowSize - 1)];
        var (x1, y1) = points[endIndex - 1];
        var (x2, y2) = points[endIndex];

        return (long)(y1 - y0) * (x2 - x1) != (long)(y2 - y1) * (x1 - x0);
    }

    // A fresh buffer every call: both strategies sort in place, so reading LeetCode's own
    // jagged input into a private array is what keeps the caller's array untouched.
    private static (int Day, int Price)[] ToPointsByDay(int[][] stockPrices)
    {
        var points = new (int Day, int Price)[stockPrices.Length];

        for (var i = 0; i < stockPrices.Length; i++)
        {
            points[i] = (stockPrices[i][DayIndex], stockPrices[i][PriceIndex]);
        }

        return points;
    }

    // Fewer points than a slope comparison needs: one line joins two points, and a lone
    // point (or none at all) needs no line.
    private static int TrivialLineCount(int pointCount) => Math.Max(pointCount - 1, 0);
}
