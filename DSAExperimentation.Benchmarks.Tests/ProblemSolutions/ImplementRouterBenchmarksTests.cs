using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementRouterBenchmarks (ARCHITECTURE 17.9). Both arms are competing
// strategies for the same question - a linear scan of the stored packets against a binary-search
// index - so they are asserted to agree. Each arm returns a single long: the sum of a numeric
// projection over every reply in the script (a successful add or a non-empty forward contributes
// one, a count query its own count). That sum is a proxy for the whole replay, since two
// strategies could in principle differ in a way that leaves the total unchanged, but it is a
// total over thousands of replies rather than a single value, and no return type is changed here
// to strengthen it. Each arm constructs its own router inside the call, so one harness is safe
// to call twice in either order. The script is built from a seeded Random, so the same
// PacketCount must rebuild the same packets and therefore the same total.
public sealed partial class ImplementRouterBenchmarksTests
{
    private const int SmallestPacketCount = 200;

    [Fact]
    public void Setup_SamePacketCount_RebuildsTheSameScript() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_ReplayTotal_AgreesWithBinarySearchIndex()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchIndex(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchIndex_ReplayTotal_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchIndex());
    }

    private static ImplementRouterBenchmarks BuildHarness()
    {
        var harness = new ImplementRouterBenchmarks { PacketCount = SmallestPacketCount };
        harness.Setup();

        return harness;
    }
}
