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

        var direction = 0;
        var x = 0;
        var y = 0;
        var maxDistanceSquared = 0;

        foreach (var command in commands)
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
}
