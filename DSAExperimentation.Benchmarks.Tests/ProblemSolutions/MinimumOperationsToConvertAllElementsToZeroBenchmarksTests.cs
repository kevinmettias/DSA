using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumOperationsToConvertAllElementsToZeroBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - how many subarray decrements it takes to
// flatten the array to zero - so a harness whose arms disagree is timing two different problems. Both
// arms read the one array [GlobalSetup] built, so the comparison also pins that the divide-and-conquer
// arm's range-minimum recursion and the stack arm's one-pass sweep were handed the same values.
// Setup is deterministic (1..Length), so the same Length must rebuild the same array.
public sealed partial class MinimumOperationsToConvertAllElementsToZeroBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MonotonicStack(), BuildHarness().MonotonicStack());

    [Fact]
    public void DivideAndConquer_SameIncreasingRun_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.DivideAndConquer());
    }

    [Fact]
    public void MonotonicStack_SameIncreasingRun_AgreesWithDivideAndConquer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DivideAndConquer(), harness.MonotonicStack());
    }

    private static MinimumOperationsToConvertAllElementsToZeroBenchmarks BuildHarness()
    {
        var harness = new MinimumOperationsToConvertAllElementsToZeroBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
