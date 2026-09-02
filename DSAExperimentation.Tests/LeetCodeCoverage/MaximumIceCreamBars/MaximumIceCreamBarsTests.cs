using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumIceCreamBars;

// LeetCode 1833. Maximum Ice Cream Bars: sort costs ascending with this repo's
// MergeSort over ArrayIndexedSequence, then greedily buy the cheapest bars
// first until coins run out - the standard exchange-argument greedy proof
// (any optimal selection can be reordered to buy strictly cheaper-or-equal
// bars first without lowering its count).
public sealed partial class MaximumIceCreamBarsTests
{
    [Fact]
    public void MaxIceCream_ClassicExample_ReturnsFourBarsAffordable()
    {
        int[] costs = [1, 3, 2, 4, 1];

        var result = MaxIceCream(costs, coins: 7);

        Assert.Equal(4, result);
    }

    [Fact]
    public void MaxIceCream_CoinsTooFewForCheapestBar_ReturnsZero()
    {
        int[] costs = [10, 6, 8, 7, 7, 8];

        var result = MaxIceCream(costs, coins: 5);

        Assert.Equal(0, result);
    }

    private static int MaxIceCream(int[] costs, int coins)
    {
        var sorted = costs.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var count = 0;

        foreach (var cost in sorted)
        {
            if (cost > coins)
            {
                break;
            }

            coins -= cost;
            count++;
        }

        return count;
    }
}
