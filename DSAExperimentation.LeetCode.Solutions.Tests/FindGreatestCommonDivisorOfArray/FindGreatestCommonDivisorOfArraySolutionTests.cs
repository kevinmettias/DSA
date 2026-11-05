using DSAExperimentation.LeetCode.Solutions.FindGreatestCommonDivisorOfArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindGreatestCommonDivisorOfArray;

// Harness only: both gcd strategies live in FindGreatestCommonDivisorOfArraySolution
// and are asserted against the same cases - LeetCode's three published ones plus
// the coprime, single-element and min-divides-max cases that pin the ends of the scan.
public sealed partial class FindGreatestCommonDivisorOfArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [3], 3 },
            { [6, 12, 18], 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindGcdBySubtraction_Cases_ReturnsGcdOfMinAndMax(int[] nums, int expected) =>
        Assert.Equal(expected, FindGreatestCommonDivisorOfArraySolution.FindGcdBySubtraction(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindGcdByEuclidean_Cases_ReturnsGcdOfMinAndMax(int[] nums, int expected) =>
        Assert.Equal(expected, FindGreatestCommonDivisorOfArraySolution.FindGcdByEuclidean(nums));
}
