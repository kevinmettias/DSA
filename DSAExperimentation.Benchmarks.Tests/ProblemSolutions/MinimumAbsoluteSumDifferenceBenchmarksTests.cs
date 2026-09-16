using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAbsoluteSumDifferenceBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an O(n^2) rescan of the whole of nums1 for every
// position against one sort of nums1 plus a binary search per position - so a harness whose arms
// disagree is timing two different problems. Setup draws both arrays from one seeded stream, so the
// same Length must rebuild both arrays identically.
public sealed partial class MinimumAbsoluteSumDifferenceBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArrays() =>
        Assert.Equal(BuildHarness().FullRescan(), BuildHarness().FullRescan());

    [Fact]
    public void FullRescan_RandomValues_AgreesWithSortedBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedBinarySearch(), harness.FullRescan());
    }

    [Fact]
    public void SortedBinarySearch_RandomValues_AgreesWithFullRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullRescan(), harness.SortedBinarySearch());
    }

    private static MinimumAbsoluteSumDifferenceBenchmarks BuildHarness()
    {
        var harness = new MinimumAbsoluteSumDifferenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
