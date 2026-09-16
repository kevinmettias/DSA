using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeTripletsToFormTargetTripletBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an exhaustive subset search against the Set<int>-tracked
// linear scan - so a harness whose arms disagree is timing two different problems. Both arms return the
// verdict as a bool, so they are compared directly.
//
// Agreement here is weak by construction, and the harness is built that way on purpose. The target is
// not reachable from the generated triplets (every coordinate is drawn below the target's own value), so
// both arms are expected to answer false and both pay their full worst-case scan instead of an early
// exit making brute force look competitive. A green pair therefore witnesses that neither arm ever
// claims a merge the other denies; it would also hold if an arm answered false for the wrong reason.
// Neither arm mutates the triplets, so one harness is safe to read twice in either order, and Setup
// draws from one fixed seed, so the same TripletCount must rebuild the same triplets.
public sealed partial class MergeTripletsToFormTargetTripletBenchmarksTests
{
    private const int SmallestTripletCount = 12;

    [Fact]
    public void Setup_SameTripletCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanFormTargetByBruteForceSubsets(),
            BuildHarness().CanFormTargetByBruteForceSubsets());

    [Fact]
    public void CanFormTargetByBruteForceSubsets_UnreachableTarget_AgreesWithCanFormTargetBySetTrackedLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanFormTargetBySetTrackedLinearScan(),
            harness.CanFormTargetByBruteForceSubsets());
    }

    [Fact]
    public void CanFormTargetBySetTrackedLinearScan_UnreachableTarget_AgreesWithCanFormTargetByBruteForceSubsets()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanFormTargetByBruteForceSubsets(),
            harness.CanFormTargetBySetTrackedLinearScan());
    }

    private static MergeTripletsToFormTargetTripletBenchmarks BuildHarness()
    {
        var harness = new MergeTripletsToFormTargetTripletBenchmarks { TripletCount = SmallestTripletCount };
        harness.Setup();

        return harness;
    }
}
