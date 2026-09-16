using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameIVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the fewest jumps from index 0 to the last index under i+1, i-1
// and same-value reachability - so a harness whose arms disagree is timing two different
// problems. Both arms take LeetCode's own int[] and the hop graph is hoisted into [GlobalSetup],
// so what is measured is the search itself; both arms build nothing of their own, which is why
// one harness instance is safe to call twice in either order.
public sealed partial class JumpGameIVBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameJumpArray() =>
        Assert.Equal(
            BuildHarness().BfsWithGroupPruning(),
            BuildHarness().BfsWithGroupPruning());

    [Fact]
    public void BfsWithGroupPruning_NarrowValueRange_AgreesWithDijkstraOverHopGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DijkstraOverHopGraph(), harness.BfsWithGroupPruning());
    }

    [Fact]
    public void DijkstraOverHopGraph_NarrowValueRange_AgreesWithBfsWithGroupPruning()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BfsWithGroupPruning(), harness.DijkstraOverHopGraph());
    }

    private static JumpGameIVBenchmarks BuildHarness()
    {
        var harness = new JumpGameIVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
