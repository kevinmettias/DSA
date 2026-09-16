using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HeightCheckerBenchmarks (ARCHITECTURE 17.9): both arms are
// HeightCheckerSolution's - the O(n^2) insertion sort of a copy against this repo's MergeSort over
// ArrayIndexedSequence - so a harness whose arms disagree is timing two different problems. Both
// arms sort a copy of the array and count the positions where the copy differs from the original,
// so both answer with the same mismatch count; neither mutates the harness's own array, which means
// arm order does not matter here. The heights are drawn at random, so the count is a property of
// the fixture rather than a value the class comment names, and agreement is the assertion.
// Setup draws off one seed, so the same Length must rebuild the same heights.
public sealed partial class HeightCheckerBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().InsertionSort(), BuildHarness().InsertionSort());

    [Fact]
    public void InsertionSort_SeededHeights_AgreesWithMergeSortComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortComparison(), harness.InsertionSort());
    }

    [Fact]
    public void MergeSortComparison_SeededHeights_AgreesWithInsertionSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InsertionSort(), harness.MergeSortComparison());
    }

    private static HeightCheckerBenchmarks BuildHarness()
    {
        var harness = new HeightCheckerBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
