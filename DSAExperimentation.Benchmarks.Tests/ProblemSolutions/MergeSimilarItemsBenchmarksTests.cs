using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeSimilarItemsBenchmarks (ARCHITECTURE 17.9): the fixture's items1 holds the
// even values and items2 the odd ones, each once, so the answer is documented to be both value sets
// merged into one entry per value - an entry count derived from the workload's construction rather
// than from either arm, which the generic agreement check cannot supply.
public sealed partial class MergeSimilarItemsBenchmarksTests
{
    private const int SmallestLength = 200;

    // items1 and items2 hold disjoint values, so the merge keeps every one of them.
    private const int DisjointValueSetCount = 2;

    private const int ExpectedMergedItemCount = SmallestLength * DisjointValueSetCount;

    [Fact]
    public void LinearScan_DisjointSeededItems_KeepsOneEntryPerValue() =>
        Assert.Equal(ExpectedMergedItemCount, BuildHarness().LinearScan().Count);

    private static MergeSimilarItemsBenchmarks BuildHarness()
    {
        var harness = new MergeSimilarItemsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
