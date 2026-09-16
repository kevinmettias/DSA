using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ApplesWorkloads (ARCHITECTURE 17.7). A workload generator is measurement
// scaffolding, so what it owes a test is the shape the benchmark reading depends on: a price per
// shop, the spanning roads that keep the network connected, a road that carries both a cost and
// a tax, and a seed that rebuilds the same workload on every run.
public sealed partial class ApplesWorkloadsTests
{
    private const int ShopCount = 12;
    private const int ExtraRoadsPerShop = 3;
    private const int Seed = 3928; // LC problem number
    private const int RoadFieldCount = 4; // FromShop, ToShop, Cost, TaxMultiplier
    private const int CostFieldIndex = 2;
    private const int TaxMultiplierFieldIndex = 3;
    private const int MaxTaxMultiplier = 100;

    [Fact]
    public void Build_ShopCount_ReturnsAPositivePriceForEveryShop()
    {
        var (prices, _) = ApplesWorkloads.Build(ShopCount, ExtraRoadsPerShop, Seed);

        Assert.Equal(ShopCount, prices.Length);
        Assert.All(prices, price => Assert.True(price > 0));
    }

    [Fact]
    public void Build_EveryShopBeyondTheFirst_GetsARoadToAnEarlierShop()
    {
        var (_, roads) = ApplesWorkloads.Build(ShopCount, ExtraRoadsPerShop, Seed);

        foreach (var shop in Enumerable.Range(1, ShopCount - 1))
        {
            Assert.Contains(roads, road => SpanningRoad(road, shop));
        }
    }

    [Fact]
    public void Build_EveryRoad_CarriesACostAndATax()
    {
        var (_, roads) = ApplesWorkloads.Build(ShopCount, ExtraRoadsPerShop, Seed);

        Assert.All(roads, road => Assert.Equal(RoadFieldCount, road.Length));
        Assert.All(roads, road => Assert.True(road[CostFieldIndex] > 0));
        Assert.All(roads, road => Assert.InRange(road[TaxMultiplierFieldIndex], 1, MaxTaxMultiplier));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (prices, roads) = ApplesWorkloads.Build(ShopCount, ExtraRoadsPerShop, Seed);
        var (repeatPrices, repeatRoads) = ApplesWorkloads.Build(ShopCount, ExtraRoadsPerShop, Seed);

        Assert.Equal(prices, repeatPrices);
        Assert.Equal(AnswerText.Of(roads), AnswerText.Of(repeatRoads));
    }

    private static bool SpanningRoad(int[] road, int shop) =>
        (road[0] == shop && road[1] < shop) || (road[1] == shop && road[0] < shop);
}
