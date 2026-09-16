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
// The two strategies differ only in how an instruction turns the current facing
// into a step: a hand-written switch over the four facings, or this repo's own
// HashMap<RobotDirection,(int,int)> as a facing-to-delta lookup table - the same
// move-to-delta idiom RobotReturnToOrigin establishes, keyed by facing instead of
// move character because going forward needs the *current* facing rather than a
// fixed per-character delta.
internal static class RobotBoundedInCircleSolution
{
    // The textbook answer: a switch per instruction, and a second switch over the
    // facing to move. Deliberately written with nothing but BCL locals - it is the
    // arm the lookup-table strategy below has to justify itself against.
    public static bool IsRobotBoundedByDirectionSwitch(string instructions)
    {
        var state = new RobotState();

        foreach (var symbol in instructions)
        {
            var instruction = (RobotInstruction)symbol;
            ApplyBySwitch(ref state, instruction);
        }

        return state.IsBounded;
    }

    private static void ApplyBySwitch(ref RobotState state, RobotInstruction instruction)
    {
        switch (instruction)
        {
            case RobotInstruction.Forward:
                MoveForward(ref state);
                break;
            case RobotInstruction.Left:
                state.Turn(RobotMotion.LeftTurnOffset);
                break;
            case RobotInstruction.Right:
                state.Turn(RobotMotion.RightTurnOffset);
                break;
        }
    }

    private static void MoveForward(ref RobotState state)
    {
        switch (state.Direction)
        {
            case RobotDirection.North:
                state.RowPosition++;
                break;
            case RobotDirection.East:
                state.ColumnPosition++;
                break;
            case RobotDirection.South:
                state.RowPosition--;
                break;
            case RobotDirection.West:
                state.ColumnPosition--;
                break;
        }
    }

    // This repo's own HashMap standing in for the inner facing switch: one lookup
    // of the current facing yields the step delta directly.
    public static bool IsRobotBoundedByStepDeltaMap(string instructions)
    {
        var stepDeltas = BuildStepDeltas();
        var state = new RobotState();

        foreach (var symbol in instructions)
        {
            var instruction = (RobotInstruction)symbol;
            ApplyByStepDeltaMap(stepDeltas, ref state, instruction);
        }

        return state.IsBounded;
    }

    private static HashMap<RobotDirection, (int Dx, int Dy)> BuildStepDeltas()
    {
        var stepDeltas = new HashMap<RobotDirection, (int Dx, int Dy)>();
        stepDeltas.Set(RobotDirection.North, (0, 1));
        stepDeltas.Set(RobotDirection.East, (1, 0));
        stepDeltas.Set(RobotDirection.South, (0, -1));
        stepDeltas.Set(RobotDirection.West, (-1, 0));
        return stepDeltas;
    }

    private static void ApplyByStepDeltaMap(
        HashMap<RobotDirection, (int Dx, int Dy)> stepDeltas, ref RobotState state,
        RobotInstruction instruction)
    {
        switch (instruction)
        {
            case RobotInstruction.Forward:
                AdvanceByStepDelta(stepDeltas, ref state);
                break;
            case RobotInstruction.Left:
                state.Turn(RobotMotion.LeftTurnOffset);
                break;
            case RobotInstruction.Right:
                state.Turn(RobotMotion.RightTurnOffset);
                break;
        }
    }

    // Forward steps the robot one unit along whichever facing it currently holds; the map
    // turns that facing into the delta, so no per-facing arm is needed here.
    private static void AdvanceByStepDelta(
        HashMap<RobotDirection, (int Dx, int Dy)> stepDeltas, ref RobotState state)
    {
        stepDeltas.TryGetValue(state.Direction, out var delta);
        state.ColumnPosition += delta.Dx;
        state.RowPosition += delta.Dy;
    }

    // Facing plus position after one pass of the instruction string - everything
    // the boundedness test needs, and the only thing either strategy mutates.
    private sealed class RobotState
    {
        public RobotDirection Direction { get; set; }
        public int ColumnPosition { get; set; }
        public int RowPosition { get; set; }

        public bool IsBounded => (ColumnPosition == 0 && RowPosition == 0) || Direction != RobotDirection.North;

        // A facing is read as its index for the arithmetic and written back as a
        // RobotDirection: a right turn is one step along the enum's own order and a
        // left turn is three, so both stay inside one modular step and neither can
        // land outside the four facings.
        public void Turn(int offset) =>
            Direction = (RobotDirection)(((int)Direction + offset) % RobotMotion.DirectionCount);
    }
}
