using DSAExperimentation.Algorithms.Sampling;

namespace DSAExperimentation.Tests.Algorithms.Sampling;

public sealed partial class SamplingWeightsTests
{
    [Fact]
    public void TotalOf_Weights_IsTheirSum() => Assert.Equal(10, SamplingWeights.TotalOf([3, 0, 1, 6]));

    [Fact]
    public void TotalOf_NegativeWeight_Throws() => Assert.Throws<ArgumentException>(() => SamplingWeights.TotalOf([2, -1]));

    [Fact]
    public void TotalOf_NoPositiveWeight_Throws() => Assert.Throws<ArgumentException>(() => SamplingWeights.TotalOf([0, 0]));
}
