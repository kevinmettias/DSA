using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignHashSetBenchmarks (ARCHITECTURE 17.9): the generic arm check compares
// the arms only with each other, so what stays here is a count derived from the workload's
// construction rather than from either arm: the workload has to contain every probed value in the
// set it probes.
public sealed partial class DesignHashSetBenchmarksTests
{
    private const int SmallestCount = 200;

    // The add order is 0..Count-1 and the probe order is that same set shuffled, so every one of the
    // Count probes has to report a hit whatever order the shuffle produced: a total short of Count is
    // what a probe order built against a different set - or a set that dropped an add - would leave
    // behind.
    [Fact]
    public void ListScan_ShuffledProbeOrder_HitsEveryProbe() =>
        Assert.Equal(SmallestCount, BuildHarness().ListScan());

    private static DesignHashSetBenchmarks BuildHarness()
    {
        var harness = new DesignHashSetBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
