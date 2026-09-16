using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BurstBalloonsWorkloads (ARCHITECTURE 17.7): one positive balloon value
// per requested slot, drawn from the modest magnitude band the exponential un-memoized
// baseline's runtime depends on, and rebuilt identically from the same seed.
public sealed partial class BurstBalloonsWorkloadsTests
{
    private const int Count = 14;
    private const int Seed = 312; // LC problem number
    private const int MinBalloonValue = 1;
    private const int MaxBalloonValue = 99;

    [Fact]
    public void BuildBalloons_Count_ReturnsOneValuePerBalloon() =>
        Assert.Equal(Count, BurstBalloonsWorkloads.BuildBalloons(Count, Seed).Length);

    [Fact]
    public void BuildBalloons_EveryBalloon_StaysWithinThePositiveMagnitudeBand()
    {
        var balloons = BurstBalloonsWorkloads.BuildBalloons(Count, Seed);

        Assert.All(balloons, value => Assert.InRange(value, MinBalloonValue, MaxBalloonValue));
    }

    [Fact]
    public void BuildBalloons_SameSeed_ReturnsTheSameBalloons() =>
        Assert.Equal(
            BurstBalloonsWorkloads.BuildBalloons(Count, Seed),
            BurstBalloonsWorkloads.BuildBalloons(Count, Seed));
}
