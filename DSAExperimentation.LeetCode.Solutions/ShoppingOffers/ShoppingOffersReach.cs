namespace DSAExperimentation.LeetCode.Solutions.ShoppingOffers;

// The reach of ShoppingOffersSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MinCostByBruteForce takes 77.4 ms over the 51 of the catalog's 57 cases
// with a length of special up to 5 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class ShoppingOffersReach
{
    private const int LargestSpecialLength = 5;

    public static bool IsWithin(int[] price, int[][] special, int[] needs) =>
        special.Length <= LargestSpecialLength;
}
