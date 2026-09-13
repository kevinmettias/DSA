using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.WalkingRobotSimulation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WalkingRobotSimulationSolution's, the same methods
// WalkingRobotSimulationTests proves correct. The command stream and the obstacle
// field are generated once in [GlobalSetup], and the hashed arm is handed a prepared
// Set<(int, int)> through its hoisted overload so building the set is not charged to
// the walk being measured.
[MemoryDiagnoser]
public class WalkingRobotSimulationBenchmarks
{
    private const int CommandCount = 1_000;
    private const int CommandKindBound = 10;
    private const int TurnLeftCommand = -2;
    private const int TurnRightCommand = -1;
    private const int MaxStepsExclusive = 10;
    private const int ObstacleCoordinateBound = 5_000;
    private const int WorkloadSeed = 1;

    [Params(50, 2_000)]
    public int ObstacleCount;

    private int[] _commands = null!;
    private int[][] _obstacles = null!;
    private Set<(int X, int Y)> _blocked = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WorkloadSeed);

        _commands = Enumerable.Range(0, CommandCount)
            .Select(_ => random.Next(0, CommandKindBound) switch
            {
                0 => TurnLeftCommand,
                1 => TurnRightCommand,
                _ => random.Next(1, MaxStepsExclusive),
            })
            .ToArray();

        _obstacles = Enumerable.Range(0, ObstacleCount)
            .Select(_ => new[]
            {
                random.Next(-ObstacleCoordinateBound, ObstacleCoordinateBound),
                random.Next(-ObstacleCoordinateBound, ObstacleCoordinateBound),
            })
            .ToArray();

        _blocked = new Set<(int X, int Y)>();

        foreach (var obstacle in _obstacles)
        {
            _blocked.TryAdd((obstacle[0], obstacle[1]));
        }
    }

    [Benchmark(Baseline = true)]
    public int LinearScanObstacles() =>
        WalkingRobotSimulationSolution.MaxDistanceSquaredByLinearScan(_commands, _obstacles);

    [Benchmark]
    public int SetObstacles() =>
        WalkingRobotSimulationSolution.MaxDistanceSquaredByObstacleSet(_commands, _blocked);
}
