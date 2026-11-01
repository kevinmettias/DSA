using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WalkingRobotSimulationIIBenchmarks (ARCHITECTURE 17.9): the generic check
// holds the arms to agreeing with each other, which a shared wrong cell would pass, so this pins the
// baseline's cell to one derived from the loop arithmetic alone.
//
// Both arms answer with LC 2069's own quantity, the cell the robot stands on after the whole move
// script. On the smaller parameter the answer is decisive: the script is twenty moves of 1000 cells
// and the robot starts at (0, 0) facing east on a 100 x 100 grid, whose boundary loop is 396 cells.
// 20000 cells is 50 whole loops and 200 more: 99 east to (99, 0), 99 north to (99, 99), and 2 west,
// leaving the robot at (97, 99).
public sealed partial class WalkingRobotSimulationIIBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 50_000)] per-move step counts.
    private const int SmallestStepsPerMove = 1_000;

    // Setup's own move count: the script it materializes is this many copies of StepsPerMove.
    private const int MoveCount = 20;

    // Setup's own grid side: the robot walks the boundary of a GridSide x GridSide square.
    private const int GridSide = 100;
    private const int SideLength = GridSide - 1;
    private const int LoopLength = 4 * SideLength;
    private const int StepsIntoLastLoop = SmallestStepsPerMove * MoveCount % LoopLength;

    // Past the east and north sides, the rest of the last loop runs west along the top row.
    private const int ExpectedColumn = SideLength - (StepsIntoLastLoop - (2 * SideLength));
    private const int ExpectedRow = SideLength;

    private static readonly (int X, int Y) ExpectedPosition = (ExpectedColumn, ExpectedRow);

    [Fact]
    public void StepSimulation_TwentyLongMoves_EndsOnTheLoopArithmeticCell() =>
        Assert.Equal(ExpectedPosition, BuildHarness().StepSimulation());

    private static WalkingRobotSimulationIIBenchmarks BuildHarness()
    {
        var harness = new WalkingRobotSimulationIIBenchmarks { StepsPerMove = SmallestStepsPerMove };
        harness.Setup();

        return harness;
    }
}
