using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeSimilarItemsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning the output list for a matching value on every insert
// against accumulating into this repo's own HashMap<int, int> and sorting with MergeSort - so a harness
// whose arms disagree is timing two different problems. Both arms return the merged items in ascending
// value order, which is the order LeetCode 2363 pins, so the order-sensitive rendering is the right
// comparison.
//
// The fixture's items1 holds the even values and items2 the odd ones, each once, so the answer is
// documented to be both value sets merged into one entry per value, and that count is what the arm
// tests pin before comparing the two readings. Both arms only read the two item arrays, so one harness
// is safe to read twice in either order, and Setup derives the items from Length alone, so the same
// Length must rebuild the same disjoint value sets.
public sealed partial class MergeSimilarItemsBenchmarksTests
{
    private const int SmallestLength = 200;

    // items1 and items2 hold disjoint values, so the merge keeps every one of them.
    private const int DisjointValueSetCount = 2;

    private const int ExpectedMergedItemCount = SmallestLength * DisjointValueSetCount;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScan()),
            AnswerText.Of(BuildHarness().LinearScan()));

    [Fact]
    public void LinearScan_DisjointSeededItems_AgreesWithHashMapAndMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMergedItemCount, harness.LinearScan().Count);
        Assert.Equal(
            AnswerText.Of(harness.HashMapAndMergeSort()),
            AnswerText.Of(harness.LinearScan()));
    }

    [Fact]
    public void HashMapAndMergeSort_DisjointSeededItems_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMergedItemCount, harness.HashMapAndMergeSort().Count);
        Assert.Equal(
            AnswerText.Of(harness.LinearScan()),
            AnswerText.Of(harness.HashMapAndMergeSort()));
    }

    private static MergeSimilarItemsBenchmarks BuildHarness()
    {
        var harness = new MergeSimilarItemsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
