using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FinalPricesWithASpecialDiscountInAShop;

// LeetCode 1475. Final Prices With a Special Discount in a Shop: the classic
// "next not-greater element" pattern via a monotonic stack of pending indices -
// this repo's own Stack<int> (LIFO over DynamicArray<int>, ARCHITECTURE.md §4.1),
// popped whenever the current price undercuts (or matches) the price on top.
public sealed partial class FinalPricesWithASpecialDiscountInAShopTests
{
    [Fact]
    public void FinalPrices_LeetCodeExample_AppliesNextNotGreaterDiscount()
    {
        int[] prices = [8, 4, 6, 2, 3];

        var result = FinalPrices(prices);

        Assert.Equal([4, 2, 4, 2, 3], result);
    }

    [Fact]
    public void FinalPrices_StrictlyIncreasingPrices_NoDiscountApplies()
    {
        int[] prices = [1, 2, 3, 4, 5];

        var result = FinalPrices(prices);

        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    private static int[] FinalPrices(int[] prices)
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
