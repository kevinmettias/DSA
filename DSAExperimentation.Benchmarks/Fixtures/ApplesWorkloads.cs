namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3928: every shop i > 0 gets a road to some
// earlier shop j < i, guaranteeing the network is connected, then extra random
// roads add density - FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks'
// own spanning-plus-extra-edges shape, here also drawing a cost and a tax
// multiplier per road instead of one plain weight.
internal static class ApplesWorkloads
{
    private const int PriceUpperBound = 1_000_000_000;
    private const int CostUpperBound = 1_000_000_000;
    private const int TaxUpperBound = 100;

    public static (int[] Prices, int[][] Roads) Build(int shopCount, int extraRoadsPerShop, int seed)
    {
        var random = new Random(seed);
        var prices = BuildPrices(random, shopCount);
        var roads = new List<int[]>();

        AppendSpanningRoads(roads, random, shopCount);
        AppendExtraRoads(roads, random, shopCount, extraRoadsPerShop);

        return (prices, [.. roads]);
    }

    private static int[] BuildPrices(Random random, int shopCount)
    {
        var prices = new int[shopCount];

        for (var i = 0; i < shopCount; i++)
        {
            prices[i] = random.Next(1, PriceUpperBound);
        }

        return prices;
    }

    // Every shop i > 0 reaches some earlier shop j < i, so these roads alone span the network.
    private static void AppendSpanningRoads(List<int[]> roads, Random random, int shopCount)
    {
        for (var i = 1; i < shopCount; i++)
        {
            var j = random.Next(i);

            var road = BuildRoad(random, i, j);
            roads.Add(road);
        }
    }

    private static void AppendExtraRoads(List<int[]> roads, Random random, int shopCount, int extraRoadsPerShop)
    {
        for (var i = 0; i < shopCount; i++)
        {
            for (var e = 0; e < extraRoadsPerShop; e++)
            {
                var target = random.Next(shopCount);

                if (target != i)
                {
                    var road = BuildRoad(random, i, target);
                    roads.Add(road);
                }
            }
        }
    }

    private static int[] BuildRoad(Random random, int u, int v) =>
        [u, v, random.Next(1, CostUpperBound), random.Next(1, TaxUpperBound + 1)];
}
