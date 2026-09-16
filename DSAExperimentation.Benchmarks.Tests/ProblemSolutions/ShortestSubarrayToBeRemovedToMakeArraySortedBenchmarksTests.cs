using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestSubarrayToBeRemovedToMakeArraySortedBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - a cubic
// "try every removal window" scan against a prefix/suffix binary-search stitch - so a harness
// whose arms disagree is removing from two different arrays. Setup draws the array from one
// fixed seed, so the same Length must rebuild the same elements; otherwise two published
// numbers were never comparable.
public sealed partial class ShortestSubarrayToBeRemovedToMakeArraySortedBenchmarksTests
{
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_EightyElementRandomArray_AgreesWithTwoPointerBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoPointerBinarySearch(), harness.BruteForce());
    }

    [Fact]
    public void TwoPointerBinarySearch_EightyElementRandomArray_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.TwoPointerBinarySearch());
    }

    private static ShortestSubarrayToBeRemovedToMakeArraySortedBenchmarks BuildHarness()
    {
        var harness = new ShortestSubarrayToBeRemovedToMakeArraySortedBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
