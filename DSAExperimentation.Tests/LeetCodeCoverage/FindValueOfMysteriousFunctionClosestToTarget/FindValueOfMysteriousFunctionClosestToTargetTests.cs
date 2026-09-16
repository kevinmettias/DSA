using DSAExperimentation.LeetCode.FindValueOfMysteriousFunctionClosestToTarget;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindValueOfMysteriousFunctionClosestToTarget;

// Harness only: both strategies live in FindValueOfMysteriousFunctionClosestToTargetSolution
// and are asserted against the same examples - LeetCode's three published ones plus a case
// where no subarray hits the target exactly, so the smallest gap has to come from a strict
// prefix of some AND walk.
public sealed partial class FindValueOfMysteriousFunctionClosestToTargetTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [9, 12, 3, 7, 15], 5, 2 },
            { [1000000], 1, 999999 },
            { [1, 2, 4, 8, 16], 0, 0 },
            { [5, 89, 79, 44], 47, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestToTargetByBruteForce_LeetCodeExamples_ReturnsSmallestGap(int[] arr, int target, int expected)
    {
        var actual =
            FindValueOfMysteriousFunctionClosestToTargetSolution.ClosestToTargetByBruteForce(arr, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestToTargetByDistinctAndValues_LeetCodeExamples_ReturnsSmallestGap(
        int[] arr,
        int target,
        int expected)
    {
        var actual =
            FindValueOfMysteriousFunctionClosestToTargetSolution.ClosestToTargetByDistinctAndValues(arr, target);

        Assert.Equal(expected, actual);
    }
}
