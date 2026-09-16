using DSAExperimentation.LeetCode.InverseCoinChange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InverseCoinChange;

// Harness only. Both strategies are InverseCoinChangeSolution's - this file
// just pins them to LeetCode's published examples, including the
// unreconstructible case that must return an empty array.
public sealed partial class InverseCoinChangeTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [0, 1, 0, 2, 0, 3, 0, 4, 0, 5], [2, 4, 6] },
            { [1, 2, 2, 3, 4], [1, 2, 5] },
            { [1, 2, 3, 4, 15], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindDenominationsByArrayTabulation_LeetCodeExamples_ReturnsDenominations(
        int[] numWays, int[] expected) =>
        Assert.Equal(expected, InverseCoinChangeSolution.FindDenominationsByArrayTabulation(numWays));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindDenominationsByMemoizedRecurrence_LeetCodeExamples_ReturnsDenominations(
        int[] numWays, int[] expected) =>
        Assert.Equal(expected, InverseCoinChangeSolution.FindDenominationsByMemoizedRecurrence(numWays));
}
