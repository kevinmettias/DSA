using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlattenNestedListIteratorBenchmarks (ARCHITECTURE 17.9): the generic arms
// check only compares the arms with each other, so this pins the baseline iterator's answer to the
// fixture's own structure - [GlobalSetup] lays down leaves 0, 1, 2, ... in ascending pairs, so the
// flattened values and their order are known without any arm.
public sealed partial class FlattenNestedListIteratorBenchmarksTests
{
    private const int SmallestLeafCount = 200;

    [Fact]
    public void EagerFlatten_PairedLeaves_ReturnsTheAscendingRun() =>
        Assert.Equal(ExpectedLeaves(), BuildHarness().EagerFlatten());

    // [GlobalSetup] fills the list with pairs of consecutive integers, so the flattened sequence is
    // the ascending run 0..SmallestLeafCount - 1.
    private static IEnumerable<int> ExpectedLeaves() => Enumerable.Range(0, SmallestLeafCount);

    private static FlattenNestedListIteratorBenchmarks BuildHarness()
    {
        var harness = new FlattenNestedListIteratorBenchmarks { LeafCount = SmallestLeafCount };
        harness.Setup();

        return harness;
    }
}
