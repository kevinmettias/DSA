using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignANumberContainerSystemBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the whole assignment table for the smallest
// matching index against a per-number heap that discards superseded indices lazily - so a harness whose
// arms disagree is timing two different problems. Setup builds the call script from one fixed seed:
// each index first gets its own distinct number, then a fifth of the indices are reassigned, which is
// the churn that makes the linear arm's scan genuinely long. Each arm builds its own subject inside the
// call and replays the same script over it, so a single harness is safe to call in either order. The
// reading is the summed index every find reported - the harness's own aggregate, chosen so the replay
// cannot be eliminated as dead code - so each find contributes either -1 or an index below Count; the
// same Count must rebuild the same script and with it the same sum.
public sealed partial class DesignANumberContainerSystemBenchmarksTests
{
    private const int SmallestCount = 200;

    private const int MinimumFoundIndexSum = -SmallestCount;
    private const int MaximumFoundIndexSum = SmallestCount * (SmallestCount - 1);

    [Fact]
    public void Setup_SameCount_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().LinearScan(),
            MinimumFoundIndexSum,
            MaximumFoundIndexSum);
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_TwoHundredSeededIndices_AgreesWithLazyDeletionHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionHeap(), harness.LinearScan());
    }

    [Fact]
    public void LazyDeletionHeap_TwoHundredSeededIndices_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.LazyDeletionHeap());
    }

    private static DesignANumberContainerSystemBenchmarks BuildHarness()
    {
        var harness = new DesignANumberContainerSystemBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
