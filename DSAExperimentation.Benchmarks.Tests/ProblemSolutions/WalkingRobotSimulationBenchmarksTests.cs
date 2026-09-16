using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WalkingRobotSimulationBenchmarks (ARCHITECTURE 17.9): its two arms are
// WalkingRobotSimulationSolution's competing strategies for the same question - rescanning the raw
// obstacle list at every step against testing a prepared Set<(int, int)> - so a harness whose arms
// disagree is walking two different obstacle fields.
//
// Both arms answer with LC 874's own quantity, the largest squared distance from the origin reached
// before any command would drive the robot into an obstacle, which is the whole answer rather than a
// proxy for it. Agreement also pins the prepared set: the arms are handed the same field in two
// different shapes, so matching on the distance means the Set really is the obstacle list. Setup
// builds the command stream, the obstacles and the set from one seed, so the same ObstacleCount must
// rebuild all three.
public sealed partial class WalkingRobotSimulationBenchmarksTests
{
    // The smaller of Setup's [Params(50, 2_000)] obstacle counts.
    private const int SmallestObstacleCount = 50;

    [Fact]
    public void Setup_SameObstacleCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScanObstacles(), BuildHarness().LinearScanObstacles());

    [Fact]
    public void LinearScanObstacles_SmallestObstacleCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetObstacles(), harness.LinearScanObstacles());
    }

    [Fact]
    public void SetObstacles_SmallestObstacleCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanObstacles(), harness.SetObstacles());
    }

    private static WalkingRobotSimulationBenchmarks BuildHarness()
    {
        var harness = new WalkingRobotSimulationBenchmarks { ObstacleCount = SmallestObstacleCount };
        harness.Setup();

        return harness;
    }
}
