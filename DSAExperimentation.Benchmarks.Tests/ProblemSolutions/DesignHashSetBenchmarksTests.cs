using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignHashSetBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a flat list scanned for the value against this repo's own Set
// - so a harness whose arms disagree is timing two different problems. Setup builds both the add
// order and the shuffled probe order from Count, so the same Count must rebuild the same workload,
// and that workload has to contain every probed value in the set it probes.
public sealed partial class DesignHashSetBenchmarksTests
{
    private const int SmallestCount = 200;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameAddAndProbeOrder()
    {
        // The add order is 0..Count-1 and the probe order is that same set shuffled, so every one
        // of the Count probes has to report a hit whatever order the shuffle produced: a total
        // short of Count is what a probe order built against a different set - or a set that
        // dropped an add - would leave behind.
        Assert.Equal(SmallestCount, BuildHarness().ListScan());
        Assert.Equal(BuildHarness().ListScan(), BuildHarness().ListScan());
    }

    [Fact]
    public void ListScan_ShuffledProbeOrder_AgreesWithSetBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetBacked(), harness.ListScan());
    }

    [Fact]
    public void SetBacked_ShuffledProbeOrder_AgreesWithListScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListScan(), harness.SetBacked());
    }

    private static DesignHashSetBenchmarks BuildHarness()
    {
        var harness = new DesignHashSetBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
