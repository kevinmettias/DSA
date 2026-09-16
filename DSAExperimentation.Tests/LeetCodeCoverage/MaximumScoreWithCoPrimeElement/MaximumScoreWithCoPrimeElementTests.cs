using DSAExperimentation.LeetCode.MaximumScoreWithCoPrimeElement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumScoreWithCoPrimeElement;

// Harness only. Both strategies are MaximumScoreWithCoPrimeElementSolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class MaximumScoreWithCoPrimeElementTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [3, 4, 6], 5, 4 },
            { [1, 2, 3], 4, 3 },
            { [2, 2], 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByBruteForce_LeetCodeExamples_ReturnsMaximumScore(int[] nums, int maxVal, int expected)
    {
        var actual = MaximumScoreWithCoPrimeElementSolution.MaximumScoreByBruteForce(nums, maxVal);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByDivisorSieve_LeetCodeExamples_ReturnsMaximumScore(int[] nums, int maxVal, int expected)
    {
        var actual = MaximumScoreWithCoPrimeElementSolution.MaximumScoreByDivisorSieve(nums, maxVal);

        Assert.Equal(expected, actual);
    }
}
