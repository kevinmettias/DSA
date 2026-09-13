using DSAExperimentation.LeetCode.MaximumNumberOfCoinsYouCanGet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfCoinsYouCanGet;

// Harness only. Both strategies live in MaximumNumberOfCoinsYouCanGetSolution;
// this file pins them to LeetCode's published examples, which is also what finally
// gets the round-simulating baseline - previously benchmark-only - under
// assertion, so the index pattern the sorted arm relies on is checked against an
// arm that actually plays the game.
public sealed class MaximumNumberOfCoinsYouCanGetTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 4, 1, 2, 7, 8], 9 },
            { [2, 4, 5], 4 },
            { [9, 8, 7, 6, 5, 1, 2, 3, 4], 18 },
            { [1, 2, 3], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCoinsByRoundSimulation_LeetCodeExamples_ReturnsYourTotal(int[] piles, int expected) =>
        Assert.Equal(expected, MaximumNumberOfCoinsYouCanGetSolution.MaxCoinsByRoundSimulation(piles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxCoinsByMergeSort_LeetCodeExamples_ReturnsYourTotal(int[] piles, int expected) =>
        Assert.Equal(expected, MaximumNumberOfCoinsYouCanGetSolution.MaxCoinsByMergeSort(piles));
}
