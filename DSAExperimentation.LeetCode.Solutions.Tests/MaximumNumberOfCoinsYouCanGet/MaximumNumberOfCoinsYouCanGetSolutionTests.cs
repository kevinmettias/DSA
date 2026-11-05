using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfCoinsYouCanGet;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfCoinsYouCanGet;

// Harness only. Both strategies live in MaximumNumberOfCoinsYouCanGetSolution;
// this file pins them to a single round of three piles, in which you take the middle
// one, which is also what finally gets the round-simulating baseline - previously
// benchmark-only - under assertion, so the index pattern the sorted arm relies on is
// checked against an arm that actually plays the game. LeetCode's published examples
// run in CatalogCaseTests.
public sealed partial class MaximumNumberOfCoinsYouCanGetSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 3], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxCoinsByRoundSimulation_Cases_ReturnsYourTotal(int[] piles, int expected) =>
        Assert.Equal(expected, MaximumNumberOfCoinsYouCanGetSolution.MaxCoinsByRoundSimulation(piles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxCoinsByMergeSort_Cases_ReturnsYourTotal(int[] piles, int expected) =>
        Assert.Equal(expected, MaximumNumberOfCoinsYouCanGetSolution.MaxCoinsByMergeSort(piles));
}
