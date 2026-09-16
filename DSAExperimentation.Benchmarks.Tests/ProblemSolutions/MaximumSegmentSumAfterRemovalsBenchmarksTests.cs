using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSegmentSumAfterRemovalsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n) rescan after each removal against the
// reverse-time disjoint set - so a harness whose arms disagree is timing two different problems.
// Setup draws the values from one fixed seed and shuffles the removal order from it, so the same
// Length must rebuild the same workload; otherwise two published numbers were never comparable in the
// first place.
//
// Both arms fold the returned answer[] down to its sum, so agreement witnessed here is agreement on
// the whole-array total, not element by element: two arms whose per-removal answers differ but happen
// to total the same would still pass. That is a weakness of the arms as written, not of this harness
// - fixing it means changing a return type, which is a harness decision for the campaign owner.
public sealed partial class MaximumSegmentSumAfterRemovalsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RescanAfterEachRemoval(), BuildHarness().RescanAfterEachRemoval());

    [Fact]
    public void RescanAfterEachRemoval_ShuffledRemovalOrder_AgreesWithReverseTimeDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReverseTimeDisjointSet(), harness.RescanAfterEachRemoval());
    }

    [Fact]
    public void ReverseTimeDisjointSet_ShuffledRemovalOrder_AgreesWithRescanAfterEachRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanAfterEachRemoval(), harness.ReverseTimeDisjointSet());
    }

    private static MaximumSegmentSumAfterRemovalsBenchmarks BuildHarness()
    {
        var harness = new MaximumSegmentSumAfterRemovalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
