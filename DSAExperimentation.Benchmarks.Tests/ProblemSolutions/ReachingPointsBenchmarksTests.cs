using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReachingPointsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - whether (TargetX, TargetY) is reachable from the source pair -
// so a harness whose arms disagree is timing two different problems. Both arms return a bool, so
// the two calls are compared directly.
//
// This class has no [GlobalSetup] and no tuned input at all: the workload is the target pair the
// [Params] TargetX names, read straight off the source constants the benchmark declares, so there
// is nothing to rebuild or seed and no Setup to check. It is also the only class in this batch
// where the shared answer is a bare verdict, which is why it is the mutation-proof target below a
// one-character negation moves.
public sealed partial class ReachingPointsBenchmarksTests
{
    private const int SmallestTargetX = 10_000;

    [Fact]
    public void IsReachableBySubtractiveReduction_SmallestTargetX_AgreesWithModuloReduction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsReachableBySubtractiveReduction(), harness.IsReachableByModuloReduction());
    }

    [Fact]
    public void IsReachableByModuloReduction_SmallestTargetX_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsReachableByModuloReduction(), harness.IsReachableBySubtractiveReduction());
    }

    private static ReachingPointsBenchmarks BuildHarness() =>
        new() { TargetX = SmallestTargetX };
}
