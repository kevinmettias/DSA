using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BestTimeToBuyAndSellStockUsingStrategyWorkloads (ARCHITECTURE 17.7):
// one price and one strategy step per index, the strategy drawn only from the hold/buy/sell
// alphabet both arms are scored over, rebuilt identically from the same seed.
public sealed partial class BestTimeToBuyAndSellStockUsingStrategyWorkloadsTests
{
    private const int Count = 64;
    private const int Seed = 3652; // LC problem number
    private const int MinPrice = 1;
    private const int MaxPrice = 100_000;
    private const int MinStrategyStep = -1;
    private const int MaxStrategyStep = 1;

    [Fact]
    public void Build_Count_ReturnsOnePriceAndOneStrategyStepPerIndex()
    {
        var (prices, strategy) = BestTimeToBuyAndSellStockUsingStrategyWorkloads.Build(Count, Seed);

        Assert.Equal(Count, prices.Length);
        Assert.Equal(Count, strategy.Length);
    }

    [Fact]
    public void Build_EveryPrice_StaysWithinThePositivePriceRange()
    {
        var (prices, _) = BestTimeToBuyAndSellStockUsingStrategyWorkloads.Build(Count, Seed);

        Assert.All(prices, price => Assert.InRange(price, MinPrice, MaxPrice));
    }

    [Fact]
    public void Build_EveryStrategyStep_IsHoldBuyOrSell()
    {
        var (_, strategy) = BestTimeToBuyAndSellStockUsingStrategyWorkloads.Build(Count, Seed);

        Assert.All(strategy, step => Assert.InRange(step, MinStrategyStep, MaxStrategyStep));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (prices, strategy) = BestTimeToBuyAndSellStockUsingStrategyWorkloads.Build(Count, Seed);
        var (repeatPrices, repeatStrategy) = BestTimeToBuyAndSellStockUsingStrategyWorkloads.Build(Count, Seed);

        Assert.Equal(prices, repeatPrices);
        Assert.Equal(strategy, repeatStrategy);
    }
}
