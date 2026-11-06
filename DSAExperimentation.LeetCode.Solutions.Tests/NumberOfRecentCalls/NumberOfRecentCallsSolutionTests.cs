using DSAExperimentation.LeetCode.Solutions.NumberOfRecentCalls;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfRecentCalls;

// Harness only: both strategies live in NumberOfRecentCallsSolution, and each
// example's pings are replayed one at a time on the counter its factory makes. The
// examples are LeetCode's published ping stream, the far-apart stream the old test
// drove by hand, a single call, and the two window boundaries (a request exactly
// 3000ms old is still counted, one 3001ms old is not).
public sealed partial class NumberOfRecentCallsSolutionTests
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
    public void CreateByFullHistoryRescan_LeetCodeExamples_ReturnsCountPerPing(
        int[] timestamps, int[] expected)
    {
        var counter = NumberOfRecentCallsSolution.CreateByFullHistoryRescan();
        var counts = PingEach(counter, timestamps);

        Assert.Equal(expected, counts);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySlidingWindowQueue_LeetCodeExamples_ReturnsCountPerPing(
        int[] timestamps, int[] expected)
    {
        var counter = NumberOfRecentCallsSolution.CreateBySlidingWindowQueue();
        var counts = PingEach(counter, timestamps);

        Assert.Equal(expected, counts);
    }

    // One ping per timestamp, in order, on the one counter - LeetCode's call script.
    private static int[] PingEach(NumberOfRecentCallsSolution.IRecentCounter counter, int[] timestamps) =>
        [.. timestamps.Select(counter.Ping)];
}
