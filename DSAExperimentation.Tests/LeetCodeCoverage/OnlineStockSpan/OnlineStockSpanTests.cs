using DSAExperimentation.LeetCode.OnlineStockSpan;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OnlineStockSpan;

// Harness only: both strategies live in OnlineStockSpanSolution and are asserted
// against the same examples - LeetCode's published next() stream, the strictly
// increasing stream the old test drove by hand, a single day, a strictly
// decreasing stream where no day ever spans another, and a flat stream where
// every day spans all of them (prices are compared with <=, not <).
public sealed class OnlineStockSpanTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [100, 80, 60, 70, 60, 75, 85], [1, 1, 1, 2, 1, 4, 6] },
            { [10, 20, 30], [1, 2, 3] },
            { [7], [1] },
            { [30, 20, 10], [1, 1, 1] },
            { [5, 5, 5, 5], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpansByBackwardScan_LeetCodeExamples_ReturnsSpanPerDay(int[] prices, int[] expected) =>
        Assert.Equal(expected, OnlineStockSpanSolution.SpansByBackwardScan(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpansByMonotonicStack_LeetCodeExamples_ReturnsSpanPerDay(int[] prices, int[] expected) =>
        Assert.Equal(expected, OnlineStockSpanSolution.SpansByMonotonicStack(prices));
}
