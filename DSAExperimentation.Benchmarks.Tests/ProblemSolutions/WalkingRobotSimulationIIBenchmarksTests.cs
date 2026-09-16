using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WalkingRobotSimulationIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// WalkingRobotSimulationIISolution's competing strategies for the same question - the per-cell step
// simulation against the O(1)-per-Move perimeter arithmetic - so a harness whose arms disagree is
// reporting the position of two different robots.
//
// Both arms answer with LC 2069's own quantity, the cell the robot stands on after the whole move
// script, and both build their robot inside the call, so one harness instance is safe to hand to
// either arm in either order. On the smaller parameter the answer is decisive: the script is twenty
// moves of 1000 cells and the robot starts at (0, 0) facing east on a 100000 x 100000 grid, so
// 20000 cells east never reaches the far corner and never turns, leaving the robot at (20000, 0).
// That literal is asserted alongside the agreement so agreement cannot hold on a shared wrong cell.
public sealed partial class WalkingRobotSimulationIIBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 50_000)] per-move step counts.
    private const int SmallestStepsPerMove = 1_000;

    // Setup's own move count: the script it materializes is this many copies of StepsPerMove.
    private const int MoveCount = 20;

    private const int ExpectedColumn = SmallestStepsPerMove * MoveCount;
    private const int ExpectedRow = 0;

    private static readonly (int X, int Y) ExpectedPosition = (ExpectedColumn, ExpectedRow);

    [Fact]
    public void Setup_SameStepsPerMove_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().StepSimulation(), BuildHarness().StepSimulation());

    [Fact]
    public void StepSimulation_SmallestStepsPerMove_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPosition, harness.StepSimulation());
        Assert.Equal(harness.PerimeterFormula(), harness.StepSimulation());
    }

    [Fact]
    public void PerimeterFormula_SmallestStepsPerMove_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPosition, harness.PerimeterFormula());
        Assert.Equal(harness.StepSimulation(), harness.PerimeterFormula());
    }

    private static WalkingRobotSimulationIIBenchmarks BuildHarness()
    {
        var harness = new WalkingRobotSimulationIIBenchmarks { StepsPerMove = SmallestStepsPerMove };
        harness.Setup();

        return harness;
    }
}
