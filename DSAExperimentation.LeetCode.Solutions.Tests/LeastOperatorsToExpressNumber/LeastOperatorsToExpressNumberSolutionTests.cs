using DSAExperimentation.LeetCode.Solutions.LeastOperatorsToExpressNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LeastOperatorsToExpressNumber;

// Harness only. Both strategies are LeastOperatorsToExpressNumberSolution's; this
// file states its cases once and asserts every strategy against them - the
// un-memoized baseline included, which the benchmark previously measured without
// anything checking its answer. The cases are targets with a short exact form: the
// base itself, 1 as a single division, and a pure power of the base. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class LeastOperatorsToExpressNumberSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            // The target is the base itself: written as "x", with no operators.
            { 5, 5, 0 },

            // Below the base: target 1 is "x/x", a single division.
            { 7, 1, 1 },

            // A pure power of the base needs only its multiplications.
            { 4, 64, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LeastOpsExpressTargetByUnmemoizedRecursion_Cases_ReturnsFewestOperators(
        int baseValue, int target, int expected)
    {
        var actual = LeastOperatorsToExpressNumberSolution.LeastOpsExpressTargetByUnmemoizedRecursion(baseValue, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LeastOpsExpressTargetByMemoizedRecursion_Cases_ReturnsFewestOperators(
        int baseValue, int target, int expected)
    {
        var actual = LeastOperatorsToExpressNumberSolution.LeastOpsExpressTargetByMemoizedRecursion(baseValue, target);

        Assert.Equal(expected, actual);
    }
}
