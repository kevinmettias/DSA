using DSAExperimentation.LeetCode.MinimumNumberOfSwapsToMakeTheStringBalanced;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfSwapsToMakeTheStringBalanced;

// Harness only: both strategies live in MinimumNumberOfSwapsToMakeTheStringBalancedSolution
// and are asserted against the same examples, including the already-balanced cases and
// the all-closers-then-all-openers shape the benchmark measures.
public sealed class MinimumNumberOfSwapsToMakeTheStringBalancedTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "][][", 1 },
            { "]]][[[", 2 },
            { "[]", 0 },
            { "[[]]", 0 },
            { "]][[", 1 },
            { "]]]][[[[", 2 },
            { "[]][[]", 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSwapsByBackwardScan_LeetCodeExamples_ReturnsMinimumSwapCount(string s, int expected) =>
        Assert.Equal(expected, MinimumNumberOfSwapsToMakeTheStringBalancedSolution.MinSwapsByBackwardScan(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSwapsByStack_LeetCodeExamples_ReturnsMinimumSwapCount(string s, int expected) =>
        Assert.Equal(expected, MinimumNumberOfSwapsToMakeTheStringBalancedSolution.MinSwapsByStack(s));
}
