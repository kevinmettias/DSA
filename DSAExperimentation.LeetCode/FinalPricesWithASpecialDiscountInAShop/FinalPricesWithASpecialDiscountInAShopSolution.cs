using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.FinalPricesWithASpecialDiscountInAShop;

// LeetCode 1475. Final Prices With a Special Discount in a Shop: every item is
// discounted by the first later price that does not exceed it - the classic "next
// not-greater element" query, asked once per position.
internal static class FinalPricesWithASpecialDiscountInAShopSolution
{
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

    // One left-to-right pass keeping indices whose discount is still pending on
    // this repo's own Stack<int> (LIFO over DynamicArray<int>,
    // ARCHITECTURE.md §4.1). The stack's prices are decreasing downward, so the
    // current price settles every pending index it undercuts and nothing else.
    public static int[] FinalPricesByMonotonicStack(int[] prices)
    {
        var result = (int[])prices.Clone();
        var pendingIndices = new RepoStack();

        for (var i = 0; i < prices.Length; i++)
        {
            while (pendingIndices.TryPeek(out var top) && prices[i] <= prices[top])
            {
                pendingIndices.TryPop(out _);
                result[top] -= prices[i];
            }

            pendingIndices.Push(i);
        }

        return result;
    }
}
