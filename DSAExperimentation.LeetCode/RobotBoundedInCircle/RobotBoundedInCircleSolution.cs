using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.RobotBoundedInCircle;

// LeetCode 1041. Robot Bounded In Circle: does a robot repeating one instruction
// string forever stay inside some circle?
//
// Running the instruction string once is enough: every repetition traces the same
// net rotation and displacement, so the path stays bounded iff the robot is back
// at the origin already, or its facing changed at all - a changed facing
// guarantees the rotations cancel and the path closes within at most 4 passes.
//
// The two strategies differ only in how a 'G' turns the current facing into a
// step: a hand-written switch over the four directions, or this repo's own
// HashMap<int,(int,int)> as a direction-to-delta lookup table - the same
// move-to-delta idiom RobotReturnToOrigin establishes, keyed by direction index
// instead of move character because 'G' needs the *current* facing rather than a
// fixed per-character delta.
internal static class RobotBoundedInCircleSolution
{
    private const int North = 0;
    private const int East = 1;
    private const int South = 2;
    private const int West = 3;
    private const int DirectionCount = 4;

    // Turning left is turning right three times, which keeps both turns inside the
    // same modular step and avoids a negative remainder.
    private const int LeftTurnOffset = 3;
    private const int RightTurnOffset = 1;

    private const char Forward = 'G';
    private const char Left = 'L';
    private const char Right = 'R';

    // The textbook answer: a switch per instruction character, and a second switch
    // over the facing to move. Deliberately written with nothing but BCL locals -
    // it is the arm the lookup-table strategy below has to justify itself against.
    public static bool IsRobotBoundedByDirectionSwitch(string instructions)
    {
        var state = new RobotState();

        foreach (var instruction in instructions)
        {
            ApplyBySwitch(ref state, instruction);
        }

        return state.IsBounded;
    }

    private static void ApplyBySwitch(ref RobotState state, char instruction)
    {
        switch (instruction)
        {
            case Forward:
                MoveForward(ref state);
                break;
            case Left:
                state.Turn(LeftTurnOffset);
                break;
            case Right:
                state.Turn(RightTurnOffset);
                break;
        }
    }

    private static void MoveForward(ref RobotState state)
    {
        switch (state.Direction)
        {
            case North:
                state.Y++;
                break;
            case East:
                state.X++;
                break;
            case South:
                state.Y--;
                break;
            default:
                state.X--;
                break;
        }
    }

    // This repo's own HashMap standing in for the inner facing switch: one lookup
    // of the current direction yields the step delta directly.
    public static bool IsRobotBoundedByStepDeltaMap(string instructions)
    {
        var stepDeltas = BuildStepDeltas();
        var state = new RobotState();

        foreach (var instruction in instructions)
        {
            ApplyByStepDeltaMap(stepDeltas, ref state, instruction);
        }

        return state.IsBounded;
    }

    private static HashMap<int, (int Dx, int Dy)> BuildStepDeltas()
    {
        var stepDeltas = new HashMap<int, (int Dx, int Dy)>();
        stepDeltas.Set(North, (0, 1));
        stepDeltas.Set(East, (1, 0));
        stepDeltas.Set(South, (0, -1));
        stepDeltas.Set(West, (-1, 0));
        return stepDeltas;
    }

    private static void ApplyByStepDeltaMap(
        HashMap<int, (int Dx, int Dy)> stepDeltas, ref RobotState state, char instruction)
    {
        switch (instruction)
        {
            case Forward:
                stepDeltas.TryGetValue(state.Direction, out var delta);
                state.X += delta.Dx;
                state.Y += delta.Dy;
                break;
            case Left:
                state.Turn(LeftTurnOffset);
                break;
            case Right:
                state.Turn(RightTurnOffset);
                break;
        }
    }

    // Facing plus position after one pass of the instruction string - everything
    // the boundedness test needs, and the only thing either strategy mutates.
    private struct RobotState
    {
        public int Direction;
        public int X;
        public int Y;

        public readonly bool IsBounded => (X == 0 && Y == 0) || Direction != North;

        public void Turn(int offset) => Direction = (Direction + offset) % DirectionCount;
    }
}
