using DSAExperimentation.LeetCode.IPO;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IPO;

// Harness only: both strategies live in IPOSolution and are asserted against the
// same examples - the classic two-heap greedy (a min-heap of projects by required
// capital feeding a max-heap of unlocked profits) and the linear-rescan baseline
// it has to justify itself against.
public sealed partial class IPOTests
{
    public static TheoryData<CapitalExample> Examples =>
        new()
        {
            {
                new CapitalExample(
                    K: 2, W: 0, Profits: new[] { 1, 2, 3 }, Capitals: new[] { 0, 1, 1 }, Expected: 4)
            },
            {
                new CapitalExample(
                    K: 1, W: 0, Profits: new[] { 1, 2, 3 }, Capitals: new[] { 0, 1, 2 }, Expected: 1)
            },
            { new CapitalExample(K: 3, W: 0, Profits: new[] { 5 }, Capitals: new[] { 10 }, Expected: 0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaximizedCapitalByLinearScan_LeetCodeExamples_GreedilyPicksBestAffordableProjects(
        CapitalExample example)
    {
        var actual = IPOSolution.FindMaximizedCapitalByLinearScan(
            example.K, example.W, example.Profits, example.Capitals);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaximizedCapitalByTwoHeapGreedy_LeetCodeExamples_GreedilyPicksBestAffordableProjects(
        CapitalExample example)
    {
        var actual = IPOSolution.FindMaximizedCapitalByTwoHeapGreedy(
            example.K, example.W, example.Profits, example.Capitals);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the project budget (how many to pick), the starting
    // capital, each project's profit and required capital, and the maximized total.
    // Five positions is over the limit, and the two arrays are the same `int[]` type,
    // so the row names every role instead of leaving them to position.
    public readonly record struct CapitalExample(
        int K, int W, int[] Profits, int[] Capitals, int Expected);
}
