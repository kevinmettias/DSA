using DSAExperimentation.LeetCode.LeastOperatorsToExpressNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LeastOperatorsToExpressNumber;

// Harness only. Both strategies are LeastOperatorsToExpressNumberSolution's; this
// file states LeetCode's published examples once and asserts every strategy
// against them - the un-memoized baseline included, which the benchmark previously
// measured without anything checking its answer.
public sealed class LeastOperatorsToExpressNumberTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            // LC example 1: x = 3, target = 19 -> "3*3 + 3*3 + 3/3", 5 operators.
            { 3, 19, 5 },

            // LC example 2: x = 5, target = 501 -> 8 operators.
            { 5, 501, 8 },

            // LC example 3: x = 100, target = 100000000 -> "100*100*100*100", 3.
            { 100, 100_000_000, 3 },

            // The target is the base itself: written as "x", with no operators.
            { 5, 5, 0 },

            // x = 2 is the one base where quotient+1 can land back on the same
            // remaining (remaining == x == 2), the fixed point the round-up guard
            // exists for - "2 + 2/2" = 3 in 2 operators (+, /).
            { 2, 3, 2 },

            // Below the base: target 1 is "x/x", a single division.
            { 7, 1, 1 },

            // A pure power of the base needs only its multiplications.
            { 4, 64, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeastOpsExpressTargetByUnmemoizedRecursion_LeetCodeExamples_ReturnsFewestOperators(
        int x, int target, int expected)
    {
        var actual = LeastOperatorsToExpressNumberSolution.LeastOpsExpressTargetByUnmemoizedRecursion(x, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeastOpsExpressTargetByMemoizedRecursion_LeetCodeExamples_ReturnsFewestOperators(
        int x, int target, int expected)
    {
        var actual = LeastOperatorsToExpressNumberSolution.LeastOpsExpressTargetByMemoizedRecursion(x, target);

        Assert.Equal(expected, actual);
    }
}
