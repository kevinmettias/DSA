using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountConnectedComponentsInLCMGraphBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - checking every pair's lcm directly against the
// threshold against unioning each present value with its own multiples - so a harness whose arms
// disagree is timing two different problems. Both arms return an int, so they are compared directly.
// Setup draws from one fixed seed, so the same Length must rebuild the same value set, and its
// documented shape is that those values really do connect: a component count below the value count
// is what makes either arm do join work instead of reporting n isolated nodes.
public sealed partial class CountConnectedComponentsInLCMGraphBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameConnectedWorkload()
    {
        Assert.Equal(
            BuildHarness().PairwiseLcmScan(),
            BuildHarness().PairwiseLcmScan());

        Assert.True(BuildHarness().MultipleUnion() < SmallestLength);
    }

    [Fact]
    public void PairwiseLcmScan_FiftyDistinctValues_AgreesWithMultipleUnion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MultipleUnion(), harness.PairwiseLcmScan());
    }

    [Fact]
    public void MultipleUnion_FiftyDistinctValues_AgreesWithPairwiseLcmScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseLcmScan(), harness.MultipleUnion());
    }

    private static CountConnectedComponentsInLCMGraphBenchmarks BuildHarness()
    {
        var harness = new CountConnectedComponentsInLCMGraphBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
