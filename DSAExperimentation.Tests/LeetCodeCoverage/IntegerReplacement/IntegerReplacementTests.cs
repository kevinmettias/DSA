using DSAExperimentation.LeetCode.IntegerReplacement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IntegerReplacement;

// Harness only. Both strategies are IntegerReplacementSolution's - LeetCode's
// published examples plus int.MaxValue (the n+1 overflow edge the solution's own
// long-typed recursion exists to guard against) are asserted against each.
public sealed class IntegerReplacementTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 8, 3 },
            { 7, 4 },
            { 4, 2 },
            { 1, 0 },
            { 2147483647, 32 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStepsByUnmemoizedRecursion_LeetCodeExamples_ReturnsMinimumStepCount(int n, int expected)
        => Assert.Equal(expected, IntegerReplacementSolution.MinStepsByUnmemoizedRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStepsByMemoizedRecurrence_LeetCodeExamples_ReturnsMinimumStepCount(int n, int expected)
        => Assert.Equal(expected, IntegerReplacementSolution.MinStepsByMemoizedRecurrence(n));
}
