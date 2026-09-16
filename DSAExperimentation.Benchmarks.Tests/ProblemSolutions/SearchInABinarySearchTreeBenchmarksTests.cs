using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInABinarySearchTreeBenchmarks (ARCHITECTURE 17.9): both arms search the
// same tree for the same key, so a harness whose arms disagree is timing two different problems.
// Setup builds the tree from one shuffled insertion order seeded at 1 and aims at the highest
// value, so the same NodeCount must rebuild the same tree - and since the tree holds every value
// 0..NodeCount-1, that highest value is present and its own value is the answer both arms must
// return. Each arm only projects the found node's value out, so one harness instance is safe to
// read twice in either order.
public sealed partial class SearchInABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;
    private const int HighestValue = SmallestNodeCount - 1;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
        Assert.Equal(HighestValue, BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_SeededTree_AgreesWithBinarySearchTreeDescent()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchTreeDescent(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchTreeDescent_SeededTree_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchTreeDescent());
    }

    private static SearchInABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new SearchInABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
