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

        _commands = Enumerable.Range(0, 1_000)
            .Select(_ => random.Next(0, 10) switch
            {
                0 => -2,
                1 => -1,
                _ => random.Next(1, 10),
            })
            .ToArray();

        _obstacles = Enumerable.Range(0, ObstacleCount)
            .Select(_ => (random.Next(-5_000, 5_000), random.Next(-5_000, 5_000)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanObstacles()
    {
        var direction = 0;
        var x = 0;
        var y = 0;
        var maxDistanceSquared = 0;

        foreach (var command in _commands)
        {
            if (command == -2)
            {
                direction = (direction + 3) % 4;
                continue;
            }

            if (command == -1)
            {
                direction = (direction + 1) % 4;
                continue;
            }

            for (var step = 0; step < command; step++)
            {
                var nextX = x + DeltaX[direction];
                var nextY = y + DeltaY[direction];

                if (IsBlockedLinear(nextX, nextY))
                {
                    break;
                }

                x = nextX;
                y = nextY;
            }

            maxDistanceSquared = Math.Max(maxDistanceSquared, x * x + y * y);
        }

        return maxDistanceSquared;
    }

    [Benchmark]
    public int SetObstacles()
    {
        var blocked = new Set<(int X, int Y)>();

        foreach (var obstacle in _obstacles)
        {
            blocked.TryAdd(obstacle);
        }

        var direction = 0;
        var x = 0;
        var y = 0;
        var maxDistanceSquared = 0;

        foreach (var command in _commands)
        {
            if (command == -2)
            {
                direction = (direction + 3) % 4;
                continue;
            }

            if (command == -1)
            {
                direction = (direction + 1) % 4;
                continue;
            }

            for (var step = 0; step < command; step++)
            {
                var nextX = x + DeltaX[direction];
                var nextY = y + DeltaY[direction];

                if (blocked.Has((nextX, nextY)))
                {
                    break;
                }

                x = nextX;
                y = nextY;
            }

            maxDistanceSquared = Math.Max(maxDistanceSquared, x * x + y * y);
        }

        return maxDistanceSquared;
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
}
