using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WalkingRobotSimulation;

// LeetCode 874. Walking Robot Simulation: obstacles hashed into this repo's own
// Set<(int,int)> for O(1) membership checks while walking - the same "hash the
// blocked cells" idiom SetMatrixZeroes already uses for rows/columns.
public sealed class WalkingRobotSimulationTests
{
    private static readonly int[] DeltaX = [0, 1, 0, -1];
    private static readonly int[] DeltaY = [1, 0, -1, 0];

    [Fact]
    public void Simulate_NoObstacles_ReturnsMaxDistanceSquaredAfterTurns()
    {
        int[] commands = [4, -1, 3];
        int[][] obstacles = [];

        var result = Simulate(commands, obstacles);

        Assert.Equal(25, result);
    }

    [Fact]
    public void Simulate_ObstacleBlocksPath_StopsOneCellBeforeIt()
    {
        int[] commands = [4, -1, 4, -2, 4];
        int[][] obstacles = [[2, 4]];

        var result = Simulate(commands, obstacles);

        Assert.Equal(65, result);
    }

    private static int Simulate(int[] commands, int[][] obstacles)
    {
        var blocked = new Set<(int X, int Y)>();

        foreach (var obstacle in obstacles)
        {
            blocked.TryAdd((obstacle[0], obstacle[1]));
        }

        var state = (Direction: 0, X: 0, Y: 0, MaxDistanceSquared: 0);

        foreach (var command in commands)
        {
            state = ProcessCommand(command, state, blocked);
        }

        return state.MaxDistanceSquared;
    }

    private static (int Direction, int X, int Y, int MaxDistanceSquared) ProcessCommand(
        int command,
        (int Direction, int X, int Y, int MaxDistanceSquared) state,
        Set<(int X, int Y)> blocked)
    {
        if (command == -2)
        {
            return ((state.Direction + 3) % 4, state.X, state.Y, state.MaxDistanceSquared);
        }

        if (command == -1)
        {
            return ((state.Direction + 1) % 4, state.X, state.Y, state.MaxDistanceSquared);
        }

        var (x, y) = MoveForward((state.X, state.Y), state.Direction, command, blocked);
        var maxDistanceSquared = Math.Max(state.MaxDistanceSquared, x * x + y * y);

        return (state.Direction, x, y, maxDistanceSquared);
    }

    private static (int X, int Y) MoveForward((int X, int Y) position, int direction, int steps, Set<(int X, int Y)> blocked)
    {
        var (x, y) = position;

        for (var step = 0; step < steps; step++)
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

        return (x, y);
    }
}
