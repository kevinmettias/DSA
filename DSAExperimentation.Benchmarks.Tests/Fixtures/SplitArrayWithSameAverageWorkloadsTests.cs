using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SplitArrayWithSameAverageWorkloads (ARCHITECTURE 17.7). Values are kept small
// so the DP's (index, count, sum) state space stays small enough to benchmark at all, which is the
// one property of this fixture the comparison rests on - a wider value band would grow the sum axis
// without bound and the memoized arm would stop being measurable beside the exhaustive one.
public sealed partial class SplitArrayWithSameAverageWorkloadsTests
{
    private const int Length = 16;
    private const int Seed = 805; // LC problem number
    private const int MinValue = 1;
    private const int MaxValue = 29; // one below the fixture's own exclusive ceiling of 30

    [Fact]
    public void BuildValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, SplitArrayWithSameAverageWorkloads.BuildValues(Length, Seed).Length);

    [Fact]
    public void BuildValues_EveryValue_StaysUnderTheKeptCeiling() =>
        Assert.All(
            SplitArrayWithSameAverageWorkloads.BuildValues(Length, Seed),
            value => Assert.InRange(value, MinValue, MaxValue));

    [Fact]
    public void BuildValues_Values_TakeMoreThanOneValueSoTheSumAxisIsExercised() =>
        Assert.True(
            SplitArrayWithSameAverageWorkloads.BuildValues(Length, Seed).Distinct().Count() > MinValue);

    [Fact]
    public void BuildValues_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            SplitArrayWithSameAverageWorkloads.BuildValues(Length, Seed),
            SplitArrayWithSameAverageWorkloads.BuildValues(Length, Seed));
}
