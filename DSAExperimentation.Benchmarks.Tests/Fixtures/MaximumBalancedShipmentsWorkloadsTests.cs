using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaximumBalancedShipmentsWorkloads (ARCHITECTURE 17.7). The reading depends on
// LC 3638's weights being the only input either strategy needs, with enough variety that a shipment's
// maximum weight is a real choice rather than one value for the whole array.
public sealed partial class MaximumBalancedShipmentsWorkloadsTests
{
    private const int Count = 256;
    private const int Seed = 3638; // LC problem number
    private const int MinWeight = 1;
    private const int MaxWeight = 1_000_000_000;
    private const int DistinctValueFloor = 2;

    [Fact]
    public void BuildWeights_Count_ReturnsOneWeightPerPosition() =>
        Assert.Equal(Count, MaximumBalancedShipmentsWorkloads.BuildWeights(Count, Seed).Length);

    [Fact]
    public void BuildWeights_EveryWeight_StaysWithinTheDocumentedBand() =>
        Assert.All(
            MaximumBalancedShipmentsWorkloads.BuildWeights(Count, Seed),
            weight => Assert.InRange(weight, MinWeight, MaxWeight));

    [Fact]
    public void BuildWeights_Weights_AreNotAllTheSame()
    {
        var weights = MaximumBalancedShipmentsWorkloads.BuildWeights(Count, Seed);

        Assert.InRange(weights.Distinct().Count(), DistinctValueFloor, Count);
    }

    [Fact]
    public void BuildWeights_SameSeed_ReturnsTheSameWeights() =>
        Assert.Equal(
            MaximumBalancedShipmentsWorkloads.BuildWeights(Count, Seed),
            MaximumBalancedShipmentsWorkloads.BuildWeights(Count, Seed));
}
