using DSAExperimentation.Algorithms.Sampling;

namespace DSAExperimentation.Tests.Algorithms.Sampling;

// Exact, so every ticket is enumerated rather than sampled: index i must own exactly its weight's
// share of them, a zero weight none.
public sealed partial class PrefixSumSamplerTests
{
    private static readonly int[] Weights = [3, 0, 1, 6];

    [Fact]
    public void Total_IsTheSumOfTheWeights() => Assert.Equal(10, new PrefixSumSampler(Weights).Total);

    [Fact]
    public void IndexFor_EveryTicket_GivesEachIndexItsWeightsShare()
    {
        var sampler = new PrefixSumSampler(Weights);
        var counts = new int[Weights.Length];

        for (var ticket = 0; ticket < sampler.Total; ticket++)
        {
            counts[sampler.IndexFor(ticket)]++;
        }

        Assert.Equal(Weights, counts);
    }

    [Fact]
    public void PrefixSumSampler_NegativeWeight_Throws() => Assert.Throws<ArgumentException>(() => new PrefixSumSampler([2, -1]));

    [Fact]
    public void PrefixSumSampler_NoPositiveWeight_Throws() => Assert.Throws<ArgumentException>(() => new PrefixSumSampler([0, 0]));
}
