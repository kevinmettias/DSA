using DSAExperimentation.LeetCode.FindGreatestCommonDivisorOfArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindGreatestCommonDivisorOfArray;

// Harness only: both gcd strategies live in FindGreatestCommonDivisorOfArraySolution
// and are asserted against the same examples - LeetCode's three published ones plus
// the coprime, single-element and min-divides-max cases that pin the ends of the scan.
public sealed class FindGreatestCommonDivisorOfArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 5, 6, 9, 10], 2 },
            { [7, 5, 6, 8, 3], 1 },
            { [3, 3], 3 },
            { [3], 3 },
            { [6, 12, 18], 6 },
            { [1, 1000], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindGcdBySubtraction_LeetCodeExamples_ReturnsGcdOfMinAndMax(int[] nums, int expected) =>
        Assert.Equal(expected, FindGreatestCommonDivisorOfArraySolution.FindGcdBySubtraction(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindGcdByEuclidean_LeetCodeExamples_ReturnsGcdOfMinAndMax(int[] nums, int expected) =>
        Assert.Equal(expected, FindGreatestCommonDivisorOfArraySolution.FindGcdByEuclidean(nums));
}
