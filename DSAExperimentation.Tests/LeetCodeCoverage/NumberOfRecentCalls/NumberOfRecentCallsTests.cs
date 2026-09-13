using DSAExperimentation.LeetCode.NumberOfRecentCalls;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRecentCalls;

// Harness only: both strategies live in NumberOfRecentCallsSolution and are asserted
// against the same examples - LeetCode's published ping stream, the far-apart stream
// the old test drove by hand, a single call, and the two window boundaries (a request
// exactly 3000ms old is still counted, one 3001ms old is not).
public sealed class NumberOfRecentCallsTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 100, 3001, 3002], [1, 2, 3, 3] },
            { [1, 10_000, 20_000], [1, 1, 1] },
            { [1], [1] },
            { [0, 3000], [1, 2] },
            { [0, 3001], [1, 1] },
            { [1, 2, 3, 4, 5], [1, 2, 3, 4, 5] },
            { [100, 3100, 3101, 6101], [1, 2, 2, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PingCountsByFullHistoryRescan_LeetCodeExamples_ReturnsCountPerPing(
        int[] timestamps, int[] expected) =>
        Assert.Equal(expected, NumberOfRecentCallsSolution.PingCountsByFullHistoryRescan(timestamps));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PingCountsBySlidingWindowQueue_LeetCodeExamples_ReturnsCountPerPing(
        int[] timestamps, int[] expected) =>
        Assert.Equal(expected, NumberOfRecentCallsSolution.PingCountsBySlidingWindowQueue(timestamps));
}
