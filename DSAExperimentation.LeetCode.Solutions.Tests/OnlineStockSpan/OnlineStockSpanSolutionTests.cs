using DSAExperimentation.LeetCode.Solutions.OnlineStockSpan;

namespace DSAExperimentation.LeetCode.Solutions.Tests.OnlineStockSpan;

// Harness only: both strategies live in OnlineStockSpanSolution, and these rows
// feed each one's whole-stream Spans, in the nested class named for the strategy,
// the method the hand-written benchmark times - a strictly increasing stream, a
// single day, a strictly decreasing stream where no day ever spans another, and a
// flat stream where every day spans all of them (prices are compared with <=, not
// <). Spans feeds every price to the object the strategy's CreateBy factory makes;
// LeetCode's published examples run that object in CatalogCaseTests.
public sealed partial class OnlineStockSpanSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [10, 20, 30], [1, 2, 3] },
            { [7], [1] },
            { [30, 20, 10], [1, 1, 1] },
            { [5, 5, 5, 5], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void BackwardScanSpans_Cases_ReturnsSpanPerDay(int[] prices, int[] expected) =>
        Assert.Equal(expected, OnlineStockSpanSolution.BackwardScan.Spans(prices));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MonotonicStackSpans_Cases_ReturnsSpanPerDay(int[] prices, int[] expected) =>
        Assert.Equal(expected, OnlineStockSpanSolution.MonotonicStack.Spans(prices));
}
