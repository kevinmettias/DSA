using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ShoppingOffers;

// LeetCode 638. Shopping Offers: buy exactly `needs` of each item at minimum cost,
// optionally applying special offers (a bundle discount) any number of times, DFS'd
// over the remaining-needs vector.
//
// MinCostByBruteForce is the textbook unmemoized DFS - deliberately written without
// this repo's primitives - the arm MinCostByMemoizedDfs is measured against. Both
// walk the same recurrence: try every applicable offer (or none) and keep the
// cheapest. The memoized arm uses this repo's own Memoizer (CoinChangeII/
// WordBreakII precedent for a composite recursion state), keyed on the needs vector
// encoded as a comma-joined string so two structurally-equal remaining-needs
// vectors - reachable via different offer application orders - reuse the same
// cached answer instead of re-solving it; a raw int[] key would use reference
// equality and never hit.
internal static class ShoppingOffersSolution
{
    // The textbook baseline this composition has to justify itself against: plain
    // recursion over the remaining-needs vector with no cache, so every offer
    // order that lands on the same remaining vector re-solves it from scratch.
    public static int MinCostByBruteForce(int[] price, int[][] special, int[] needs)
    {
        var best = DirectCost(price, needs);

        foreach (var offer in special)
        {
            if (!Applies(needs, offer))
            {
                continue;
            }

            var afterOffer = Reduce(needs, offer);
            best = Math.Min(best, offer[^1] + MinCostByBruteForce(price, special, afterOffer));
        }

        return best;
    }

    public static int MinCostByMemoizedDfs(int[] price, int[][] special, int[] needs)
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
                best = Math.Min(best, offer[^1] + bestPrice(Encode(afterOffer)));
            }

            return best;
        }
    }

    // Shared by both strategies: buying every remaining item at full price, with
    // no offer applied, is always a valid (if not always cheapest) answer.
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
