using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Walking Robot Simulation (LC 874): an O(k) linear scan of the obstacles array on
// every attempted step vs. this repo's own Set<(int,int)> giving each step an O(1)
// membership check - the same "brute force vs. hashed lookup" shape TwoSumBenchmarks
// already uses, applied here to obstacle checks instead of complement lookups.
[MemoryDiagnoser]
public class WalkingRobotSimulationBenchmarks
{
    private const int CommandCount = 1_000;
    private const int CommandKindBound = 10;
    private const int TurnLeftCommand = -2;
    private const int MaxStepsExclusive = 10;
    private const int ObstacleCoordinateBound = 5_000;
    private const int DirectionCount = 4;

    private static readonly int[] DeltaX = [0, 1, 0, -1];
    private static readonly int[] DeltaY = [1, 0, -1, 0];

    [Params(50, 2_000)]
    public int ObstacleCount;

    private int[] _commands = null!;
    private (int X, int Y)[] _obstacles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);

        _commands = Enumerable.Range(0, CommandCount)
            .Select(_ => random.Next(0, CommandKindBound) switch
            {
                0 => TurnLeftCommand,
                1 => -1,
                _ => random.Next(1, MaxStepsExclusive),
            })
            .ToArray();

        _obstacles = Enumerable.Range(0, ObstacleCount)
            .Select(_ => (random.Next(-ObstacleCoordinateBound, ObstacleCoordinateBound), random.Next(-ObstacleCoordinateBound, ObstacleCoordinateBound)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanObstacles()
    {
        var state = new RobotState(0, 0, 0);
        var maxDistanceSquared = 0;

        foreach (var command in _commands)
        {
            state = ExecuteCommandLinear(state, command);
            maxDistanceSquared = Math.Max(maxDistanceSquared, state.DistanceSquared);
        }

        return maxDistanceSquared;
    }

    private RobotState ExecuteCommandLinear(RobotState state, int command)
    {
        if (command == TurnLeftCommand)
        {
            return TurnLeft(state);
        }

        if (command == -1)
        {
            return TurnRight(state);
        }

        return MoveForwardLinear(state, command);
    }

    private static RobotState TurnLeft(RobotState state) =>
        state with { Direction = (state.Direction + DirectionCount - 1) % DirectionCount };

    private static RobotState TurnRight(RobotState state) =>
        state with { Direction = (state.Direction + 1) % DirectionCount };

    private RobotState MoveForwardLinear(RobotState state, int steps)
    {
        var (x, y) = (state.X, state.Y);

        for (var step = 0; step < steps; step++)
        {
            if (!TryStepLinear((x, y), state.Direction, out var next))
            {
                break;
            }

            (x, y) = next;
        }

        return state with { X = x, Y = y };
    }

    private bool TryStepLinear((int X, int Y) position, int direction, out (int X, int Y) next)
    {
        next = (position.X + DeltaX[direction], position.Y + DeltaY[direction]);
        return !IsBlockedLinear(next.X, next.Y);
    }

    [Benchmark]
    public int SetObstacles()
    {
        var blocked = new Set<(int X, int Y)>();

        foreach (var obstacle in _obstacles)
        {
            blocked.TryAdd(obstacle);
        }

        var state = new RobotState(0, 0, 0);
        var maxDistanceSquared = 0;

        foreach (var command in _commands)
        {
            state = ExecuteCommandSet(blocked, state, command);
            maxDistanceSquared = Math.Max(maxDistanceSquared, state.DistanceSquared);
        }

        return maxDistanceSquared;
    }

    private static RobotState ExecuteCommandSet(Set<(int X, int Y)> blocked, RobotState state, int command)
    {
        if (command == TurnLeftCommand)
        {
            return TurnLeft(state);
        }

        if (command == -1)
        {
            return TurnRight(state);
        }

        return MoveForwardSet(blocked, state, command);
    }

    private static RobotState MoveForwardSet(Set<(int X, int Y)> blocked, RobotState state, int steps)
    {
        var (x, y) = (state.X, state.Y);

        for (var step = 0; step < steps; step++)
        {
            if (!TryStepSet(blocked, (x, y), state.Direction, out var next))
            {
                break;
            }

            (x, y) = next;
        }

        return state with { X = x, Y = y };
    }

    private static bool TryStepSet(Set<(int X, int Y)> blocked, (int X, int Y) position, int direction, out (int X, int Y) next)
    {
        next = (position.X + DeltaX[direction], position.Y + DeltaY[direction]);
        return !blocked.Has(next);
    }

    private bool IsBlockedLinear(int x, int y)
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle.X == x && obstacle.Y == y)
            {
                return true;
            }
        }

        return false;
    }

    private readonly record struct RobotState(int Direction, int X, int Y)
    {
        public int DistanceSquared => X * X + Y * Y;
    }
}
