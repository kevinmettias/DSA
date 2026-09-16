using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimizeDeviationInArrayBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning the working values for the largest on every halving step
// against this repo's own max-heap, which offers it up in O(log n) - so a harness whose arms disagree is
// timing two different problems. Both arms return the minimized deviation as an int, so they are compared
// directly, and both copy the input into their own working storage rather than halving the input array in
// place, so one harness is safe to read twice in either order. Setup draws the values from one fixed seed,
// so the same Length must rebuild the same array and with it the same deviation.
public sealed partial class MinimizeDeviationInArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearRescanEachStep(), BuildHarness().LinearRescanEachStep());

    [Fact]
    public void LinearRescanEachStep_SeededValues_AgreesWithMaxHeapReduce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MaxHeapReduce(), harness.LinearRescanEachStep());
    }

    [Fact]
    public void MaxHeapReduce_SeededValues_AgreesWithLinearRescanEachStep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearRescanEachStep(), harness.MaxHeapReduce());
    }

    private static MinimizeDeviationInArrayBenchmarks BuildHarness()
    {
        var harness = new MinimizeDeviationInArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
