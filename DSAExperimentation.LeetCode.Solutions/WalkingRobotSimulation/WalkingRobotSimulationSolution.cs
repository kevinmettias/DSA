using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Domain.Devices.Compass;

namespace DSAExperimentation.LeetCode.Solutions.WalkingRobotSimulation;

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
// two arms read CompassRules alike, with east as +X and north as +Y (ARCHITECTURE §17.5), and
// differ only in how they find an obstacle. The -2 and -1 commands are LeetCode's.
[LeetCodeQuestion("walking-robot-simulation", LeetCodeAnswerOrder.AsWritten)]
internal static class WalkingRobotSimulationSolution
{
    private const int TurnLeftCommand = -2;
    private const int TurnRightCommand = -1;

    // The textbook answer: keep the obstacles exactly as LeetCode hands them over and
    // scan them linearly for every cell the robot tries to enter. Deliberately
    // container-free - it is the arm the hashed strategy below has to justify itself
    // against.
    public static int MaxDistanceSquaredByLinearScan(int[] commands, int[][] obstacles)
        => MaxDistanceSquared(commands, new ObstacleList(obstacles));

    // The walk both arms share: a turn changes the heading, a walk steps until an obstacle stops
    // it, and the farthest square distance reached is the answer. TObstacles is how the arm keeps
    // and finds its obstacles, the one thing the two differ in.
    private static int MaxDistanceSquared<TObstacles>(int[] commands, TObstacles obstacles)
        where TObstacles : struct, IObstacles
    {
        var state = new HeadingState(CompassHeading.North, 0, 0);
        var maxDistanceSquared = 0;

        foreach (var command in commands)
        {
            state = command switch
            {
                TurnLeftCommand => state with { Heading = CompassRules.TurnLeft(state.Heading) },
                TurnRightCommand => state with { Heading = CompassRules.TurnRight(state.Heading) },
                _ => MoveForward(state, command, obstacles),
            };

            maxDistanceSquared = Math.Max(maxDistanceSquared, state.DistanceSquared);
        }

        return maxDistanceSquared;
    }

    private static HeadingState MoveForward<TObstacles>(HeadingState state, int steps, TObstacles obstacles)
        where TObstacles : struct, IObstacles
    {
        var position = (X: state.X, Y: state.Y);

        for (var step = 0; step < steps; step++)
        {
            var next = NextCellFacing(position, state.Heading);

            if (obstacles.IsBlocked(next))
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
        => MaxDistanceSquared(commands, new ObstacleSet(blocked));

    // How an arm keeps its obstacles and finds whether a cell holds one: the layer the two
    // strategies swap. Each is a struct the walk is instantiated over, so its test is a direct call;
    // a type parameter for the obstacles themselves would be a reference type, whose shared generic
    // code reaches the test through a runtime lookup on every step.
    private interface IObstacles
    {
        bool IsBlocked((int X, int Y) cell);
    }

    // LeetCode's own obstacle list, scanned whole for every cell.
    private readonly struct ObstacleList(int[][] obstacles) : IObstacles
    {
        public bool IsBlocked((int X, int Y) cell)
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
    }

    // The obstacles hashed once, each cell one membership check.
    private readonly struct ObstacleSet(Set<(int X, int Y)> blocked) : IObstacles
    {
        public bool IsBlocked((int X, int Y) cell) => blocked.Has(cell);
    }

    private readonly record struct HeadingState(CompassHeading Heading, int X, int Y)
    {
        public int DistanceSquared => (X * X) + (Y * Y);
    }
}
