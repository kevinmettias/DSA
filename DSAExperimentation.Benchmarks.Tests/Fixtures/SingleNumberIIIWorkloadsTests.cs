using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SingleNumberIIIWorkloads (ARCHITECTURE 17.7). LC 260's contract is exactly
// two values appearing once and everything else twice, and the reading depends on it holding: an
// input with a third singleton or a tripled value is one neither arm is specified for, so the
// benchmark would be timing a question the problem never asked.
public sealed partial class SingleNumberIIIWorkloadsTests
{
    private const int Length = 200;
    private const int Seed = 260; // LC problem number
    private const int ElementsPerPair = 2;
    private const int SingletonCount = 2;
    private const int PairedAppearanceCount = 2;

    [Fact]
    public void BuildValues_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Length, SingleNumberIIIWorkloads.BuildValues(Length, Seed).Length);

    [Fact]
    public void BuildValues_Values_ContainExactlyTwoThatAppearOnce()
    {
        var values = SingleNumberIIIWorkloads.BuildValues(Length, Seed);

        Assert.Equal(SingletonCount, values.Count(value => AppearanceCount(values, value) == 1));
    }

    [Fact]
    public void BuildValues_Values_ContainOnlySingletonsAndPairs()
    {
        var values = SingleNumberIIIWorkloads.BuildValues(Length, Seed);

        Assert.All(
            values.Distinct(),
            value => Assert.InRange(
                AppearanceCount(values, value), 1, PairedAppearanceCount));
        Assert.Equal(
            SingletonCount + (((Length - SingletonCount) / ElementsPerPair) * ElementsPerPair),
            values.Length);
    }

    [Fact]
    public void BuildValues_SameSeed_ReturnsTheSameValues() =>
        Assert.Equal(
            SingleNumberIIIWorkloads.BuildValues(Length, Seed),
            SingleNumberIIIWorkloads.BuildValues(Length, Seed));

    private static int AppearanceCount(int[] values, int value) =>
        values.Count(candidate => candidate == value);
}
