using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaxValueOfEquationBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - scanning every ordered pair inside the window against the
// monotonic deque that keeps only the best candidate per right endpoint - so a harness whose arms
// disagree is timing two different problems. Both arms return the largest yi + yj + |xi - xj| LC 1499
// asks for over the strictly increasing x values Setup builds. Setup draws both coordinates from one
// fixed seed, so the same Length must rebuild the same points and the same value.
public sealed partial class MaxValueOfEquationBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().AllPairsScan(), BuildHarness().AllPairsScan());
        Assert.Equal(BuildHarness().MonotonicDeque(), BuildHarness().MonotonicDeque());
    }

    [Fact]
    public void AllPairsScan_EveryPairInsideTheWindow_AgreesWithMonotonicDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDeque(), harness.AllPairsScan());
    }

    [Fact]
    public void MonotonicDeque_EveryPairInsideTheWindow_AgreesWithAllPairsScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AllPairsScan(), harness.MonotonicDeque());
    }

    private static MaxValueOfEquationBenchmarks BuildHarness()
    {
        var harness = new MaxValueOfEquationBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
