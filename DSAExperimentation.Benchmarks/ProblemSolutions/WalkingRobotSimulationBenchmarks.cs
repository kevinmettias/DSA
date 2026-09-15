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

    private int[] _commands = [];

    private int[][] _obstacles = [];
    private Set<(int X, int Y)> _blocked = new();
    [Params(50, 2_000)]
    public int ObstacleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WorkloadSeed);

        _commands = BuildCommands(random);
        _obstacles = BuildObstacles(random, ObstacleCount);
        _blocked = BuildBlockedSet(_obstacles);
    }

    private static int[] BuildCommands(Random random) =>
        Enumerable.Range(0, CommandCount)
            .Select(_ => random.Next(0, CommandKindBound) switch
            {
                0 => TurnLeftCommand,
                1 => TurnRightCommand,
                _ => random.Next(1, MaxStepsExclusive),
            })
            .ToArray();

    private static int[][] BuildObstacles(Random random, int obstacleCount) =>
        Enumerable.Range(0, obstacleCount)
            .Select(_ => new[]
            {
                random.Next(-ObstacleCoordinateBound, ObstacleCoordinateBound),
                random.Next(-ObstacleCoordinateBound, ObstacleCoordinateBound),
            })
            .ToArray();

    private static Set<(int X, int Y)> BuildBlockedSet(int[][] obstacles)
    {
        var blocked = new Set<(int X, int Y)>();

        foreach (var obstacle in obstacles)
        {
            blocked.TryAdd((obstacle[0], obstacle[1]));
        }

        return blocked;
    }

    [Benchmark(Baseline = true)]
    public int LinearScanObstacles() =>
        WalkingRobotSimulationSolution.MaxDistanceSquaredByLinearScan(_commands, _obstacles);

    [Benchmark]
    public int SetObstacles() =>
        WalkingRobotSimulationSolution.MaxDistanceSquaredByObstacleSet(_commands, _blocked);
}
