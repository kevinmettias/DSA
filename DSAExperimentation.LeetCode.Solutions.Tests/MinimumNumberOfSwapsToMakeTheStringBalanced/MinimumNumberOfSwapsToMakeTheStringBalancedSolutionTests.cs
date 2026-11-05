using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfSwapsToMakeTheStringBalanced;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfSwapsToMakeTheStringBalanced;

// Harness only: both strategies live in MinimumNumberOfSwapsToMakeTheStringBalancedSolution
// and are asserted against the same cases, among them the already-balanced cases and
// the all-closers-then-all-openers shape the benchmark measures.
public sealed partial class MinimumNumberOfSwapsToMakeTheStringBalancedSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "[[]]", 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByBackwardScan_Cases_ReturnsMinimumSwapCount(string text, int expected) =>
        Assert.Equal(expected, MinimumNumberOfSwapsToMakeTheStringBalancedSolution.MinSwapsByBackwardScan(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapsByStack_Cases_ReturnsMinimumSwapCount(string text, int expected) =>
        Assert.Equal(expected, MinimumNumberOfSwapsToMakeTheStringBalancedSolution.MinSwapsByStack(text));
}
