using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.FinalPricesWithASpecialDiscountInAShop;

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
    // its full price.
    public static int[] FinalPricesByMonotonicStack(int[] prices)
    {
        var discountingItem = NearestBoundary.SmallerOrEqualToTheRight(prices, NoDiscountingItem);
        var result = (int[])prices.Clone();

        for (var i = 0; i < prices.Length; i++)
        {
            if (discountingItem[i] != NoDiscountingItem)
            {
                result[i] -= prices[discountingItem[i]];
            }
        }

        return result;
    }
}
