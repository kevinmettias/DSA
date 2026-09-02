namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3652 - random prices and a random -1/0/1 strategy
// of matching length, plus a window size k fixed at a modest even constant so both
// arms are scored on the same window width regardless of how n grows.
internal static class BestTimeToBuyAndSellStockUsingStrategyWorkloads
{
    private const int MaxPrice = 100_000;

    public static (int[] Prices, int[] Strategy) Build(int count, int seed)
    {
        var random = new Random(seed);
        var prices = new int[count];
        var strategy = new int[count];

        for (var i = 0; i < count; i++)
        {
            prices[i] = random.Next(1, MaxPrice + 1);
            strategy[i] = random.Next(-1, 2);
        }

        return (prices, strategy);
    }
}
