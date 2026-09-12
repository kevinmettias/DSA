using DSAExperimentation.LeetCode.FibonacciNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FibonacciNumber;

// Harness only. Both strategies are FibonacciNumberSolution's - this file just pins
// them to LeetCode's published examples, including the naive baseline, which was
// never asserted before this migration.
public sealed class FibonacciNumberTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 0, 0 },
            { 1, 1 },
            { 5, 5 },
            { 20, 6765 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FibByNaiveRecursion_LeetCodeExamples_ReturnsFibonacciNumber(int n, int expected) =>
        Assert.Equal(expected, FibonacciNumberSolution.FibByNaiveRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FibByMemoizedTopDown_LeetCodeExamples_ReturnsFibonacciNumber(int n, int expected) =>
        Assert.Equal(expected, FibonacciNumberSolution.FibByMemoizedTopDown(n));
}
