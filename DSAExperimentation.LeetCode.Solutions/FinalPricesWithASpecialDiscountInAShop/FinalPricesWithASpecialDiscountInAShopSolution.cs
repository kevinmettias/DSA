using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.Solutions.FinalPricesWithASpecialDiscountInAShop;

// LeetCode 1475. Final Prices With a Special Discount in a Shop: every item is
// discounted by the first later price that does not exceed it - the classic "next
// not-greater element" query, asked once per position.
internal static class FinalPricesWithASpecialDiscountInAShopSolution
{
    private const int NoDiscountingItem = -1;

    // The textbook baseline: for each item, scan forward until a price undercuts
    // (or matches) it. Deliberately plain BCL arrays and nested loops, O(n^2) -
    // the arm the monotonic-stack strategy below has to justify itself against.
    public static int[] FinalPricesByBruteForce(int[] prices)
    {
        var result = (int[])prices.Clone();

        for (var i = 0; i < prices.Length; i++)
        {
            for (var j = i + 1; j < prices.Length; j++)
            {
                if (prices[j] <= prices[i])
                {
                    result[i] -= prices[j];
                    break;
                }
            }
        }

        return result;
    }

    // One NearestBoundary.SmallerOrEqualToTheRight sweep: the discount for each item
    // is the nearest later price that undercuts or matches it - exactly the boundary
    // that monotonic stack leaves standing - and an item with no such price keeps
    // its full price. Each boundary is overwritten by its final price, so the sweep's
    // own array is the answer and no copy of prices is made.
    public static int[] FinalPricesByMonotonicStack(int[] prices)
    {
        var finalPrices = NearestBoundary.SmallerOrEqualToTheRight(prices, NoDiscountingItem);

        for (var i = 0; i < prices.Length; i++)
        {
            if (finalPrices[i] == NoDiscountingItem)
            {
                finalPrices[i] = prices[i];
                continue;
            }

            finalPrices[i] = prices[i] - prices[finalPrices[i]];
        }

        return finalPrices;
    }
}
