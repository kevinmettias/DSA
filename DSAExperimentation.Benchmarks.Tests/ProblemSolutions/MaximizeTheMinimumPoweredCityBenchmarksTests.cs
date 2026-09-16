using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeTheMinimumPoweredCityBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the highest minimum power every city can be brought to
// with the fixed extra-station budget - so a harness whose arms disagree is timing two different
// problems. Setup draws the station values from a fixed seed and folds them into the prepared plan
// the arms are handed, so the same length must rebuild the same plan; neither arm mutates it.
public sealed partial class MaximizeTheMinimumPoweredCityBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.SequenceLowerBound());
    }

    [Fact]
    public void SequenceLowerBound_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.LinearScan());
    }

    private static MaximizeTheMinimumPoweredCityBenchmarks BuildHarness()
    {
        var harness = new MaximizeTheMinimumPoweredCityBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
