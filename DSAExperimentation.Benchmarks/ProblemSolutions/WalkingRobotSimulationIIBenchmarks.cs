using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.WalkingRobotSimulationII.WalkingRobotSimulationIISolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WalkingRobotSimulationIISolution's, the same classes
// WalkingRobotSimulationIITests proves correct - the textbook per-unit-step walk
// (what this repo's own WalkingRobotSimulation, LC 874, does) against the
// O(1)-per-Move perimeter arithmetic LC 2069 raises its limits to force. Both track
// the identical loop and land on the same cell; only the per-Move cost differs,
// O(num steps) against O(1), which is why the gap widens with StepsPerMove rather
// than with the number of moves. [GlobalSetup] materializes the move script so
// building it is not charged to either arm.
[MemoryDiagnoser]
public class WalkingRobotSimulationIIBenchmarks
{
    private const int Width = 100_000;
    private const int Height = 100_000;
    private const int MoveCount = 20;

    [Params(1_000, 50_000)]
    public int StepsPerMove;

    private int[] _moves = null!;

    [GlobalSetup]
    public void Setup()
    {
        _moves = Enumerable.Repeat(StepsPerMove, MoveCount).ToArray();
    }

    [Benchmark(Baseline = true)]
    public (int X, int Y) StepSimulation() => Replay(new RobotByStepSimulation(Width, Height));

    [Benchmark]
    public (int X, int Y) PerimeterFormula() => Replay(new RobotByPerimeterFormula(Width, Height));

    private (int X, int Y) Replay(IRobot robot)
    {
        foreach (var steps in _moves)
        {
            robot.Move(steps);
        }

        return robot.GetPos();
    }
}
