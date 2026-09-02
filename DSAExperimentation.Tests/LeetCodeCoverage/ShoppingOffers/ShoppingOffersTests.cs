using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShoppingOffers;

// LeetCode 638. Shopping Offers: DFS over the remaining-needs vector, trying every
// applicable special offer (or none) and keeping the cheapest, memoized via this
// repo's own Memoizer (CoinChangeII/WordBreakII precedent for a composite recursion
// state) keyed on the needs vector encoded as a comma-joined string so two
// structurally-equal remaining-needs vectors - reachable via different offer
// application orders - reuse the same cached answer instead of re-solving it.
public sealed partial class ShoppingOffersTests
{
    [Fact]
    public void MinCost_LeetCodeExampleOne_PrefersTheCheaperOffer()
    {
        int[] price = [2, 5];
        int[][] special = [[3, 0, 5], [1, 2, 10]];
        int[] needs = [3, 2];

        var minCost = MinCost(price, special, needs);

        Assert.Equal(14, minCost);
    }

    [Fact]
    public void MinCost_LeetCodeExampleTwo_SkipsOfferThatIsNotCheaper()
    {
        int[] price = [2, 3, 4];
        int[][] special = [[1, 1, 0, 4], [2, 2, 1, 9]];
        int[] needs = [1, 2, 1];

        var minCost = MinCost(price, special, needs);

        Assert.Equal(11, minCost);
    }

    private static int MinCost(int[] price, int[][] special, int[] needs)
    {
        return Memoizer.Memoize<string, int>(Encode(needs), BestPrice);

        int BestPrice(string key, Func<string, int> bestPrice)
        {
            var remaining = Decode(key);
            var best = DirectCost(price, remaining);

            foreach (var offer in special)
            {
                if (!Applies(remaining, offer))
                {
                    continue;
                }

                var afterOffer = Reduce(remaining, offer);
                var afterOfferKey = Encode(afterOffer);
                best = Math.Min(best, offer[^1] + bestPrice(afterOfferKey));
            }

            return best;
        }
    }

    private static int DirectCost(int[] price, int[] remaining)
    {
        var total = 0;
        for (var i = 0; i < remaining.Length; i++)
        {
            total += remaining[i] * price[i];
        }

        return total;
    }

    private static bool Applies(int[] remaining, int[] offer)
    {
        for (var i = 0; i < remaining.Length; i++)
        {
            if (offer[i] > remaining[i])
            {
                return false;
            }
        }

        return true;
    }

    private static int[] Reduce(int[] remaining, int[] offer)
    {
        var next = new int[remaining.Length];
        for (var i = 0; i < remaining.Length; i++)
        {
            next[i] = remaining[i] - offer[i];
        }

        return next;
    }

    private static string Encode(int[] needs) => string.Join(',', needs);

    private static int[] Decode(string key) => Array.ConvertAll(key.Split(','), int.Parse);
}
