using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaxChunksToMakeSortedIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the suffix at every candidate boundary
// against the monotonic-stack merge that settles each boundary in one pass - so a harness whose arms
// disagree is timing two different problems. Both arms return the chunk count LC 768 asks for over a
// permutation of 0..Length-1. Setup shuffles that permutation from one fixed seed, so the same
// Length must rebuild the same array and the same count.
public sealed partial class MaxChunksToMakeSortedIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForceSuffixRescan(), BuildHarness().BruteForceSuffixRescan());
        Assert.Equal(BuildHarness().MonotonicStackMerge(), BuildHarness().MonotonicStackMerge());
    }

    [Fact]
    public void BruteForceSuffixRescan_SeededPermutation_AgreesWithMonotonicStackMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackMerge(), harness.BruteForceSuffixRescan());
    }

    [Fact]
    public void MonotonicStackMerge_SeededPermutation_AgreesWithBruteForceSuffixRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSuffixRescan(), harness.MonotonicStackMerge());
    }

    private static MaxChunksToMakeSortedIIBenchmarks BuildHarness()
    {
        var harness = new MaxChunksToMakeSortedIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
