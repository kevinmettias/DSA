using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RangeModuleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - how many of a fixed query batch fall entirely inside the
// tracked ranges - so a harness whose arms disagree is timing two different problems. Both arms
// return that count as a scalar.
//
// This is a stateful Design benchmark whose two subjects are hoisted into FIELDS, so arm order
// would matter if either arm mutated them - but [GlobalSetup] pre-populates both instances and the
// benchmark arms only call QueryRange, which is a read. One harness is therefore safe to call
// twice in either order, and the single-harness rule holds here.
//
// Setup pre-populates both instances and draws the query batch from one fixed seed, so the same
// Length must rebuild the same pair.
public sealed partial class RangeModuleBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_PrepopulatedDisjointRanges_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.IntervalSetBinarySearch());
    }

    [Fact]
    public void IntervalSetBinarySearch_PrepopulatedDisjointRanges_AgreesWithTheLinearScanArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntervalSetBinarySearch(), harness.LinearScan());
    }

    private static RangeModuleBenchmarks BuildHarness()
    {
        var harness = new RangeModuleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
