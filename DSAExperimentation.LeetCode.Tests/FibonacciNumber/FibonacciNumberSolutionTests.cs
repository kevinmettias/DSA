using DSAExperimentation.LeetCode.FibonacciNumber;

namespace DSAExperimentation.LeetCode.Tests.FibonacciNumber;

// Harness only. Every strategy is FibonacciNumberSolution's - this file just pins
// them, the naive baseline included (it was never asserted before this migration),
// to LeetCode's published examples and four terms derived by hand below.
public sealed partial class FibonacciNumberSolutionTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { 2, 1 },
            { 3, 2 },
            { 4, 3 },

            // The two seeds F(0) = 0 and F(1) = 1, then 0, 1, 1, 2, 3, 5 puts F(5) at 5,
            // and the sequence continued to its 21st term (the bound n = 30 allows it)
            // reaches F(20) = 4181 + 2584 = 6765.
            { 0, 0 },
            { 1, 1 },
            { 5, 5 },
            { 20, 6765 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FibByNaiveRecursion_LeetCodeExamples_ReturnsFibonacciNumber(int sequenceIndex, int expected) =>
        Assert.Equal(expected, FibonacciNumberSolution.FibByNaiveRecursion(sequenceIndex));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FibByMemoizedTopDown_LeetCodeExamples_ReturnsFibonacciNumber(int sequenceIndex, int expected) =>
        Assert.Equal(expected, FibonacciNumberSolution.FibByMemoizedTopDown(sequenceIndex));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FibByIterativeRollingPair_LeetCodeExamples_ReturnsFibonacciNumber(int sequenceIndex, int expected) =>
        Assert.Equal(expected, FibonacciNumberSolution.FibByIterativeRollingPair(sequenceIndex));
}
