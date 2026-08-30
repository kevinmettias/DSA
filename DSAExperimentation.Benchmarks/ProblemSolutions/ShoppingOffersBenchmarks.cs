using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shopping Offers (LC 638): the textbook unmemoized DFS over the remaining-needs
// vector (every offer-application order re-solves each remaining vector it lands on
// from scratch) vs. this repo's Memoizer caching each remaining vector's best price
// exactly once (CoinChangeII/WordBreakII precedent). Three pairwise offers over the
// same three items give multiple offer-application orders that land on the same
// remaining-needs vector - exactly the redundant recomputation BruteForce pays for
// and Memoized avoids.
[MemoryDiagnoser]
public class ShoppingOffersBenchmarks
{
    private static readonly int[] Price = [2, 3, 4];

    private static readonly int[][] Special =
    [
        [1, 1, 0, Price[0] + Price[1] - 1],
        [0, 1, 1, Price[1] + Price[2] - 1],
        [1, 0, 1, Price[0] + Price[2] - 1],
    ];

    [Params(4, 7)]
    public int NeedsPerItem;

    private int[] _needs = null!;

    [GlobalSetup]
    public void Setup() => _needs = [NeedsPerItem, NeedsPerItem, NeedsPerItem];

    [Benchmark(Baseline = true)]
    public int BruteForce() => BestPriceUnmemoized(_needs);

    private static int BestPriceUnmemoized(int[] remaining)
    {
        var best = DirectCost(remaining);

        foreach (var offer in Special)
        {
            if (!Applies(remaining, offer))
            {
                continue;
            }

            best = Math.Min(best, offer[^1] + BestPriceUnmemoized(Reduce(remaining, offer)));
        }

        return best;
    }

    [Benchmark]
    public int Memoized()
    {
        return Memoizer.Memoize<string, int>(Encode(_needs), BestPrice);

        int BestPrice(string key, Func<string, int> bestPrice)
        {
            var remaining = Decode(key);
            var best = DirectCost(remaining);

            foreach (var offer in Special)
            {
                if (!Applies(remaining, offer))
                {
                    continue;
                }

                best = Math.Min(best, offer[^1] + bestPrice(Encode(Reduce(remaining, offer))));
            }

            return best;
        }
    }

    private static int DirectCost(int[] remaining)
    {
        var total = 0;
        for (var i = 0; i < remaining.Length; i++)
        {
            total += remaining[i] * Price[i];
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
