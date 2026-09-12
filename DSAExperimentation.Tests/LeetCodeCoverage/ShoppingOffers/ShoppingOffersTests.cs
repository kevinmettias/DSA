using DSAExperimentation.LeetCode.ShoppingOffers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShoppingOffers;

// Harness only. Both strategies are ShoppingOffersSolution's - this file just pins
// them to LeetCode's published examples, including the brute-force baseline, which
// was never asserted before this migration.
public sealed class ShoppingOffersTests
{
    public static TheoryData<int[], int[][], int[], int> Examples =>
        new()
        {
            { [2, 5], [[3, 0, 5], [1, 2, 10]], [3, 2], 14 },
            { [2, 3, 4], [[1, 1, 0, 4], [2, 2, 1, 9]], [1, 2, 1], 11 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByBruteForce_LeetCodeExamples_PrefersTheCheaperOffer(
        int[] price, int[][] special, int[] needs, int expected) =>
        Assert.Equal(expected, ShoppingOffersSolution.MinCostByBruteForce(price, special, needs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByMemoizedDfs_LeetCodeExamples_PrefersTheCheaperOffer(
        int[] price, int[][] special, int[] needs, int expected) =>
        Assert.Equal(expected, ShoppingOffersSolution.MinCostByMemoizedDfs(price, special, needs));
}
