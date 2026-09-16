using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RangeSumOfBSTBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the sum of every BST value in [Low, High] - so a harness whose
// arms disagree is timing two different problems. Both return that sum as a scalar. Setup shuffles
// 0..Length-1 from one fixed seed and inserts it into a fresh search tree, so the same Length must
// rebuild the same tree; otherwise two published numbers were never comparable in the first place.
public sealed partial class RangeSumOfBSTBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().FullTreeScan(), BuildHarness().FullTreeScan());

    [Fact]
    public void FullTreeScan_SmallestValueRange_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullTreeScan(), harness.SearchTreePrunedWalk());
    }

    [Fact]
    public void SearchTreePrunedWalk_SmallestValueRange_AgreesWithFullTreeScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SearchTreePrunedWalk(), harness.FullTreeScan());
    }

    private static RangeSumOfBSTBenchmarks BuildHarness()
    {
        var harness = new RangeSumOfBSTBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
