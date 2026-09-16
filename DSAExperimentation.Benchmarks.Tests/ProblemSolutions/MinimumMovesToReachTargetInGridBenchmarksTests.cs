using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumMovesToReachTargetInGridBenchmarks (ARCHITECTURE 17.9): both arms
// are MinimumMovesToReachTargetInGridSolution's, the same methods
// MinimumMovesToReachTargetInGridTests proves correct, and both return the fewest moves from
// the start square to the target square. The bounded forward BFS and the composed backward
// reduction are competing strategies for that one number, so arms that disagree are timing two
// different problems.
//
// Setup's workload comes from a seeded fixture, so the same parameters must rebuild the same
// square pair; otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumMovesToReachTargetInGridBenchmarksTests
{
    // The smallest declared [Params] value: the baseline BFS is bounded by the move count, so
    // a smaller bound is the cheaper way to reach the same comparison.
    private const int SmallestMoveCount = 8;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BackwardReduction(),
            BuildHarness().BackwardReduction());

    [Fact]
    public void BoundedForwardBfs_AgreesWithBackwardReduction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BackwardReduction(), harness.BoundedForwardBfs());
    }

    [Fact]
    public void BackwardReduction_AgreesWithBoundedForwardBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BoundedForwardBfs(), harness.BackwardReduction());
    }

    private static MinimumMovesToReachTargetInGridBenchmarks BuildHarness()
    {
        var harness = new MinimumMovesToReachTargetInGridBenchmarks { MoveCount = SmallestMoveCount };
        harness.Setup();

        return harness;
    }
}
