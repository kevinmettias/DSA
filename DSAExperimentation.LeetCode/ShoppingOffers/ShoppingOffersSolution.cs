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
            if (!CanApply(needs, offer))
            {
                continue;
            }

            var afterOffer = Reduce(needs, offer);
            best = Math.Min(best, offer[^1] + MinCostByBruteForce(price, special, afterOffer));
        }

        return best;
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

    private static bool CanApply(int[] remaining, int[] offer)
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

    public static int MinCostByMemoizedDfs(int[] price, int[][] special, int[] needs) =>
        Memoizer.Memoize<string, int>(Encode(needs), new OfferApplicationOrder(price, special));

    // The rule, named: from a remaining-needs vector, buy everything left at full price
    // or apply one applicable offer and pay for whatever it leaves behind. The price list
    // and the offers are the whole of what the rule needs from its caller, so they are
    // the constructor's only inputs.
    private sealed class OfferApplicationOrder(int[] price, int[][] special) : IRecurrence<string, int>
    {
        public int Replay(string state, IRecurrence<string, int> rest)
        {
            var remaining = Decode(state);
            var best = DirectCost(price, remaining);

            foreach (var offer in special)
            {
                if (!CanApply(remaining, offer))
                {
                    continue;
                }

                var afterOffer = Reduce(remaining, offer);
                best = Math.Min(best, offer[^1] + rest.Replay(Encode(afterOffer), rest));
            }

            return best;
        }
    }

    private static string Encode(int[] needs) => string.Join(',', needs);

    private static int[] Decode(string key) => Array.ConvertAll(key.Split(','), int.Parse);
}
