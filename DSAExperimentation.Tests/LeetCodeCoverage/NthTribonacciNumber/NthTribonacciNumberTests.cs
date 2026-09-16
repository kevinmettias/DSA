using DSAExperimentation.LeetCode.NthTribonacciNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NthTribonacciNumber;

// Harness only: both strategies are NthTribonacciNumberSolution's - this file pins
// them to LeetCode's published examples plus the three seed values, including the
// naive triple recursion, which was never asserted before this migration.
// termIndex = 25 is LeetCode's second example and is as far as the exponential arm
// is asked to go.
public sealed partial class NthTribonacciNumberTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 0, 0 },
            { 1, 1 },
            { 2, 1 },
            { 3, 2 },
            { 4, 4 },
            { 25, 1_389_537 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TribonacciByNaiveRecursion_LeetCodeExamples_ReturnsTribonacciNumber(
        int termIndex, int expected) =>
        Assert.Equal(expected, NthTribonacciNumberSolution.TribonacciByNaiveRecursion(termIndex));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TribonacciByMemoizedTopDown_LeetCodeExamples_ReturnsTribonacciNumber(
        int termIndex, int expected) =>
        Assert.Equal(expected, NthTribonacciNumberSolution.TribonacciByMemoizedTopDown(termIndex));
}
