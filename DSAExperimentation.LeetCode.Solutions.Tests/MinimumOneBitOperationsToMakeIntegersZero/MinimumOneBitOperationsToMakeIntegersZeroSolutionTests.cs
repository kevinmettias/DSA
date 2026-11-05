using DSAExperimentation.LeetCode.Solutions.MinimumOneBitOperationsToMakeIntegersZero;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumOneBitOperationsToMakeIntegersZero;

// Harness only. Both strategies are MinimumOneBitOperationsToMakeIntegersZeroSolution's
// - this file just pins them to the already-zero
// case, the single-flip case, and two powers/patterns far enough along the Gray code
// path that the closed form and the search would visibly disagree if either drifted.
public sealed partial class MinimumOneBitOperationsToMakeIntegersZeroSolutionTests
{
    public static TheoryData<int, int> Cases =>
        new()
        {
            { 4, 7 },
            { 9, 14 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumOneBitOperationsByBreadthFirstSearch_Cases_ReturnsFewestFlips(
        int target, int expected) =>
        Assert.Equal(
            expected,
            MinimumOneBitOperationsToMakeIntegersZeroSolution
                .MinimumOneBitOperationsByBreadthFirstSearch(target));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumOneBitOperationsByInverseGrayCode_Cases_ReturnsFewestFlips(
        int target, int expected) =>
        Assert.Equal(
            expected,
            MinimumOneBitOperationsToMakeIntegersZeroSolution
                .MinimumOneBitOperationsByInverseGrayCode(target));
}
