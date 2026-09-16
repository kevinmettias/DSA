using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfMovesToSeatEveryoneBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumNumberOfMovesToSeatEveryoneSolution's, the same methods
// MinimumNumberOfMovesToSeatEveryoneTests proves correct, and both return the same index-wise
// distance sum over the paired-sorted seats and students. They differ only in how the pairing
// is established - an O(n^2) selection sort against this repo's own O(n log n) MergeSort - so
// arms that disagree are timing two different problems.
public sealed partial class MinimumNumberOfMovesToSeatEveryoneBenchmarksTests
{
    // The smallest declared [Params] value: the selection-sort arm is quadratic, so a shorter
    // pair of arrays is the cheaper way to reach the same pairing comparison.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().SelectionSortPairSum(),
            BuildHarness().SelectionSortPairSum());

    [Fact]
    public void SelectionSortPairSum_AgreesWithMergeSortPairSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortPairSum(), harness.SelectionSortPairSum());
    }

    [Fact]
    public void MergeSortPairSum_AgreesWithSelectionSortPairSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SelectionSortPairSum(), harness.MergeSortPairSum());
    }

    private static MinimumNumberOfMovesToSeatEveryoneBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfMovesToSeatEveryoneBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
