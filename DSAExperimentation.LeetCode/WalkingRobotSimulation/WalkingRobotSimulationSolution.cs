using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Domain.Devices.Compass;

namespace DSAExperimentation.LeetCode.WalkingRobotSimulation;

// LeetCode 874. Walking Robot Simulation: a robot starts at (0, 0) facing north and
// obeys a command stream of "turn left" (-2), "turn right" (-1) and "walk k cells";
// a step onto an obstacle cell is refused and the rest of that walk is abandoned.
// The answer is the largest squared Euclidean distance from the origin the robot
// ever reaches.
//
// Both strategies walk the identical command stream - the only thing that differs is
// how a candidate cell is tested against the obstacle list. The baseline rescans the
// whole obstacle array per attempted step, O(commands * steps * obstacles); the
// composed strategy hashes the obstacles into this repo's own Set<(int, int)> once
// and answers each step in O(1), the same "brute force vs. hashed lookup" shape
// TwoSum uses for complement lookups.
//
// The robot's headings, turns and steps are the compass's (Domain/Devices/Compass): the
// obstacle-set arm reads CompassRules, with east as +X and north as +Y, and the
// baseline keeps its own copies below (ARCHITECTURE §17.5). The -2 and -1 commands
// are LeetCode's.
internal static class WalkingRobotSimulationSolution
{
    private const int TurnLeftCommand = -2;
    private const int TurnRightCommand = -1;

    // The baseline's own copy of the compass: directions in clockwise order starting
    // at north, so a right turn is +1 and a left turn is -1 modulo four.
    private const int DirectionCount = 4;
    private static readonly int[] DeltaX = [0, 1, 0, -1];
    private static readonly int[] DeltaY = [1, 0, -1, 0];

    // The textbook answer: keep the obstacles exactly as LeetCode hands them over and
    // scan them linearly for every cell the robot tries to enter. Deliberately
    // BCL-only - it is the arm the hashed strategy below has to justify itself
    // against.
    public static int MaxDistanceSquaredByLinearScan(int[] commands, int[][] obstacles)
    {
        var state = new RobotState(0, 0, 0);
        var maxDistanceSquared = 0;

        foreach (var command in commands)
        {
            state = command switch
            {
                TurnLeftCommand => TurnLeft(state),
                TurnRightCommand => TurnRight(state),
                _ => MoveForwardByLinearScan(state, command, obstacles),
            };

            maxDistanceSquared = Math.Max(maxDistanceSquared, state.DistanceSquared);
        }

        return maxDistanceSquared;
    }

    private static RobotState MoveForwardByLinearScan(RobotState state, int steps, int[][] obstacles)
    {
        var position = (X: state.X, Y: state.Y);

        for (var step = 0; step < steps; step++)
        {
            var next = NextCell(position, state.Direction);

            if (IsBlocked(obstacles, next))
            {
                break;
            }

            position = next;
        }

        return state with { X = position.X, Y = position.Y };
    }

    private static bool IsBlocked(int[][] obstacles, (int X, int Y) cell)
    {
        foreach (var obstacle in obstacles)
        {
            if (obstacle[0] == cell.X && obstacle[1] == cell.Y)
            {
                return true;
            }
        }

        return false;
    }

    // This repo's own Set<(int, int)>: the obstacle cells are hashed once and every
    // attempted step becomes a single membership check.
    public static int MaxDistanceSquaredByObstacleSet(int[] commands, int[][] obstacles)
    {
        var blocked = new Set<(int X, int Y)>();

        foreach (var obstacle in obstacles)
        {
            blocked.TryAdd((obstacle[0], obstacle[1]));
        }

        return MaxDistanceSquaredByObstacleSet(commands, blocked);
    }

    public static int MaxDistanceSquaredByObstacleSet(int[] commands, Set<(int X, int Y)> blocked)
    {
        var state = new HeadingState(CompassHeading.North, 0, 0);
        var maxDistanceSquared = 0;

        foreach (var command in commands)
        {
            state = command switch
            {
                TurnLeftCommand => state with { Heading = CompassRules.TurnLeft(state.Heading) },
                TurnRightCommand => state with { Heading = CompassRules.TurnRight(state.Heading) },
                _ => MoveForwardByObstacleSet(state, command, blocked),
            };

            maxDistanceSquared = Math.Max(maxDistanceSquared, state.DistanceSquared);
        }

        return maxDistanceSquared;
    }

    private static HeadingState MoveForwardByObstacleSet(HeadingState state, int steps, Set<(int X, int Y)> blocked)
    {
        var position = (X: state.X, Y: state.Y);

        for (var step = 0; step < steps; step++)
        {
            var next = NextCellFacing(position, state.Heading);

            if (blocked.Has(next))
            {
                break;
            }

            position = next;
        }

        return state with { X = position.X, Y = position.Y };
    }

    private static (int X, int Y) NextCellFacing((int X, int Y) position, CompassHeading heading)
    {
        var (east, north) = CompassRules.StepOf(heading);
        return (position.X + east, position.Y + north);
    }

    private static (int X, int Y) NextCell((int X, int Y) position, int direction) =>
        (position.X + DeltaX[direction], position.Y + DeltaY[direction]);

    private static RobotState TurnLeft(RobotState state) =>
        state with { Direction = (state.Direction + DirectionCount - 1) % DirectionCount };

    private static RobotState TurnRight(RobotState state) =>
        state with { Direction = (state.Direction + 1) % DirectionCount };

    private readonly record struct RobotState(int Direction, int X, int Y)
    {
        public int DistanceSquared => (X * X) + (Y * Y);
    }

    private readonly record struct HeadingState(CompassHeading Heading, int X, int Y)
    {
        public int DistanceSquared => (X * X) + (Y * Y);
    }
}
