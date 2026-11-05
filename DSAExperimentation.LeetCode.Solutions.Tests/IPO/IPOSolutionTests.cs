using DSAExperimentation.LeetCode.Solutions.IPO;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IPO;

// Harness only: both strategies live in IPOSolution and are asserted against the
// same cases - the classic two-heap greedy (a min-heap of projects by required
// capital feeding a max-heap of unlocked profits) and the linear-rescan baseline
// it has to justify itself against. The cases are a budget of one project, which can
// take only the one affordable at the start, and a project no starting capital can
// afford. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class IPOSolutionTests
{
    public static TheoryData<CapitalCase> Cases =>
        new()
        {
            {
                new CapitalCase(
                    K: 1, W: 0, Profits: new[] { 1, 2, 3 }, Capitals: new[] { 0, 1, 2 }, Expected: 1)
            },
            { new CapitalCase(K: 3, W: 0, Profits: new[] { 5 }, Capitals: new[] { 10 }, Expected: 0) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaximizedCapitalByLinearScan_Cases_GreedilyPicksBestAffordableProjects(
        CapitalCase testCase)
    {
        var actual = IPOSolution.FindMaximizedCapitalByLinearScan(
            testCase.K, testCase.W, testCase.Profits, testCase.Capitals);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaximizedCapitalByTwoHeapGreedy_Cases_GreedilyPicksBestAffordableProjects(
        CapitalCase testCase)
    {
        var actual = IPOSolution.FindMaximizedCapitalByTwoHeapGreedy(
            testCase.K, testCase.W, testCase.Profits, testCase.Capitals);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the project budget (how many to pick), the starting
    // capital, each project's profit and required capital, and the maximized total.
    // Five positions is over the limit, and the two arrays are the same `int[]` type,
    // so the row names every role instead of leaving them to position.
    public readonly record struct CapitalCase(
        int K, int W, int[] Profits, int[] Capitals, int Expected);
}
