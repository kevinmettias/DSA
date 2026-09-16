using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumBalancedShipmentsBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the largest number of balanced shipments the weights can
// be cut into - so a harness whose arms disagree is timing two different problems. Setup draws the
// weights from the fixture's fixed seed, so the same parcel count must rebuild the same workload;
// neither arm mutates it.
public sealed partial class MaximumBalancedShipmentsBenchmarksTests
{
    private const int SmallestParcelCount = 200;

    [Fact]
    public void Setup_SameParcelCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithPreviousGreaterStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.PreviousGreaterStack());
    }

    [Fact]
    public void PreviousGreaterStack_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PreviousGreaterStack(), harness.BruteForce());
    }

    private static MaximumBalancedShipmentsBenchmarks BuildHarness()
    {
        var harness = new MaximumBalancedShipmentsBenchmarks { ParcelCount = SmallestParcelCount };
        harness.Setup();

        return harness;
    }
}
