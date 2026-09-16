using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestUnsortedContinuousSubarrayBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - a selection-sort scan against a
// merge-sort scan - so a harness whose arms disagree is measuring two different arrays. Setup
// draws the array from one fixed seed, so the same Length must rebuild the same elements;
// otherwise two published numbers were never comparable.
public sealed partial class ShortestUnsortedContinuousSubarrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray() =>
        Assert.Equal(BuildHarness().MergeSortScan(), BuildHarness().MergeSortScan());

    [Fact]
    public void MergeSortScan_TwoHundredElementRandomArray_AgreesWithSelectionSortScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SelectionSortScan(), harness.MergeSortScan());
    }

    [Fact]
    public void SelectionSortScan_TwoHundredElementRandomArray_AgreesWithMergeSortScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortScan(), harness.SelectionSortScan());
    }

    private static ShortestUnsortedContinuousSubarrayBenchmarks BuildHarness()
    {
        var harness = new ShortestUnsortedContinuousSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
