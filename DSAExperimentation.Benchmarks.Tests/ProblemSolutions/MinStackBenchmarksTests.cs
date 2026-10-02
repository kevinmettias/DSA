using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinStackBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Replay returns a checksum over every value pushed, every minimum read and every value
// popped, so equal checksums mean the two arms walked the same script to the same state.
public sealed partial class MinStackBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().StackPrimitive(), BuildHarness().StackPrimitive());

    [Fact]
    public void SingleListScan_AgreesWithStackPrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackPrimitive(), harness.SingleListScan());
    }

    private static MinStackBenchmarks BuildHarness()
    {
        var harness = new MinStackBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
