using DSAExperimentation.LeetCode.Solutions.FindValueOfMysteriousFunctionClosestToTarget;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindValueOfMysteriousFunctionClosestToTarget;

// Harness only: both strategies live in FindValueOfMysteriousFunctionClosestToTargetSolution
// and are asserted against the same cases - LeetCode's three published ones plus a case
// where no subarray hits the target exactly, so the smallest gap has to come from a strict
// prefix of some AND walk.
public sealed partial class FindValueOfMysteriousFunctionClosestToTargetSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1000000], 1, 999999 },
            { [5, 89, 79, 44], 47, 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ClosestToTargetByBruteForce_Cases_ReturnsSmallestGap(int[] arr, int target, int expected)
    {
        var actual =
            FindValueOfMysteriousFunctionClosestToTargetSolution.ClosestToTargetByBruteForce(arr, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ClosestToTargetByDistinctAndValues_Cases_ReturnsSmallestGap(
        int[] arr,
        int target,
        int expected)
    {
        var actual =
            FindValueOfMysteriousFunctionClosestToTargetSolution.ClosestToTargetByDistinctAndValues(arr, target);

        Assert.Equal(expected, actual);
    }
}
