using DSAExperimentation.LeetCode.MinimumPossibleMaximumWaitingTime;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumPossibleMaximumWaitingTime;

// Harness only. Both search strategies are
// MinimumPossibleMaximumWaitingTimeSolution's - this file just pins them to
// LeetCode's published examples, including the unserved-first-car case the
// search has to answer with -1 rather than a wait time.
public sealed partial class MinimumPossibleMaximumWaitingTimeTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [6, 8, 4, 6, 5], [16, 13], 6 },
            { [10, 15], [12, 17], 0 },
            { [10, 5], [8, 8], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWaitByRecursiveSearch_LeetCodeExamples_ReturnsMinimumPossibleMaxWait(
        int[] demand, int[] fuel, int expected)
    {
        var actual = MinimumPossibleMaximumWaitingTimeSolution.MinWaitByRecursiveSearch(demand, fuel);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWaitByMemoizedSearch_LeetCodeExamples_ReturnsMinimumPossibleMaxWait(
        int[] demand, int[] fuel, int expected)
    {
        var actual = MinimumPossibleMaximumWaitingTimeSolution.MinWaitByMemoizedSearch(demand, fuel);
        Assert.Equal(expected, actual);
    }
}
