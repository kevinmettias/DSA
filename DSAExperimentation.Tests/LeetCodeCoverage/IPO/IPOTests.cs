using DSAExperimentation.LeetCode.IPO;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IPO;

// Harness only: both strategies live in IPOSolution and are asserted against the
// same examples - the classic two-heap greedy (a min-heap of projects by required
// capital feeding a max-heap of unlocked profits) and the linear-rescan baseline
// it has to justify itself against.
public sealed class IPOTests
{
    public static TheoryData<int, int, int[], int[], int> Examples =>
        new()
        {
            { 2, 0, new[] { 1, 2, 3 }, new[] { 0, 1, 1 }, 4 },
            { 1, 0, new[] { 1, 2, 3 }, new[] { 0, 1, 2 }, 1 },
            { 3, 0, new[] { 5 }, new[] { 10 }, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaximizedCapitalByLinearScan_LeetCodeExamples_GreedilyPicksBestAffordableProjects(
        int k, int w, int[] profits, int[] capitals, int expected) =>
        Assert.Equal(expected, IPOSolution.FindMaximizedCapitalByLinearScan(k, w, profits, capitals));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaximizedCapitalByTwoHeapGreedy_LeetCodeExamples_GreedilyPicksBestAffordableProjects(
        int k, int w, int[] profits, int[] capitals, int expected) =>
        Assert.Equal(expected, IPOSolution.FindMaximizedCapitalByTwoHeapGreedy(k, w, profits, capitals));
}
