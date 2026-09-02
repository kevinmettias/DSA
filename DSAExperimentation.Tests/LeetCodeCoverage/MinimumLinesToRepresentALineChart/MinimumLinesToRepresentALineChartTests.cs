using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumLinesToRepresentALineChart;

// LeetCode 2280. Minimum Lines to Represent a Line Chart: sort the (day, price) points by day
// with this repo's own MergeSort over ArrayIndexedSequence (same composition ArrayPartitionTests
// already uses for a plain int[]), then walk consecutive point triples counting slope changes.
// Collinearity between segment (p0,p1) and segment (p1,p2) is tested via the cross-product form
// (y1-y0)*(x2-x1) == (y2-y1)*(x1-x0) instead of comparing floating-point slopes, so no division
// or precision loss is ever involved - widened to long before multiplying since day/price can
// each be up to 1e9 per the problem's constraints.
public sealed partial class MinimumLinesToRepresentALineChartTests
{
    [Fact]
    public void MinimumLines_LeetCodeExampleOne_ReturnsThree()
    {
        int[][] stockPrices = [[1, 7], [2, 6], [3, 5], [4, 4], [5, 4], [6, 3], [7, 2], [8, 1]];

        Assert.Equal(3, MinimumLines(stockPrices));
    }

    [Fact]
    public void MinimumLines_LeetCodeExampleTwo_UnsortedInputStillCollapsesToOneLine()
    {
        int[][] stockPrices = [[3, 4], [1, 2], [7, 8], [2, 3]];

        Assert.Equal(1, MinimumLines(stockPrices));
    }

    [Fact]
    public void MinimumLines_SinglePoint_ReturnsZero()
    {
        int[][] stockPrices = [[5, 5]];

        Assert.Equal(0, MinimumLines(stockPrices));
    }

    private static int MinimumLines(int[][] stockPrices)
    {
        if (stockPrices.Length == 1)
        {
            return 0;
        }

        var points = stockPrices.Select(price => (Day: price[0], Price: price[1])).ToArray();
        var byDay = Comparer<(int Day, int Price)>.Create((a, b) => a.Day.CompareTo(b.Day));
        MergeSort.Sort<(int Day, int Price), ArrayIndexedSequence<(int Day, int Price)>>(
            new ArrayIndexedSequence<(int Day, int Price)>(points), byDay);

        var lines = 1;

        for (var i = 2; i < points.Length; i++)
        {
            if (!IsCollinear(points[i - 2], points[i - 1], points[i]))
            {
                lines++;
            }
        }

        return lines;
    }

    private static bool IsCollinear((int Day, int Price) p0, (int Day, int Price) p1, (int Day, int Price) p2)
        => (long)(p1.Price - p0.Price) * (p2.Day - p1.Day) == (long)(p2.Price - p1.Price) * (p1.Day - p0.Day);
}
