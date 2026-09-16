using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberIVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - sweeping every candidate capability against bisecting the
// same monotone predicate - so a harness whose arms disagree is timing two different problems.
// Both arms answer with a single int, so they are compared directly rather than rendered. The
// workload is seeded, so the same Length must rebuild the same values and the same required
// house count, and therefore the same capability.
public sealed partial class HouseRobberIVBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_NarrowestCapability_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.LinearScan());
    }

    [Fact]
    public void SequenceLowerBound_NarrowestCapability_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.SequenceLowerBound());
    }

    private static HouseRobberIVBenchmarks BuildHarness()
    {
        var harness = new HouseRobberIVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
