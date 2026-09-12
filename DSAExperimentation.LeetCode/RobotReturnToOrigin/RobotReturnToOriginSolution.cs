using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.RobotReturnToOrigin;

// LeetCode 657. Robot Return to Origin: fold a move string into a net (x, y)
// displacement and check it lands back on (0, 0).
//
// JudgeCircleBySwitch is the textbook baseline: a hand-written switch per move
// character, deliberately written without this repo's primitives. It is the arm
// JudgeCircleByHashMapLookup - this repo's own HashMap<char,(int,int)> used as a
// move-to-displacement lookup table - is measured against.
internal static class RobotReturnToOriginSolution
{
    public static bool JudgeCircleBySwitch(string moves)
    {
        var x = 0;
        var y = 0;

        foreach (var move in moves)
        {
            var (dx, dy) = DeltaFor(move);
            x += dx;
            y += dy;
        }

        return x == 0 && y == 0;
    }

    private static (int Dx, int Dy) DeltaFor(char move) => move switch
    {
        'U' => (0, 1),
        'D' => (0, -1),
        'L' => (-1, 0),
        'R' => (1, 0),
        _ => (0, 0),
    };

    public static bool JudgeCircleByHashMapLookup(string moves)
    {
        var deltas = new HashMap<char, (int Dx, int Dy)>();
        deltas.Set('U', (0, 1));
        deltas.Set('D', (0, -1));
        deltas.Set('L', (-1, 0));
        deltas.Set('R', (1, 0));

        var x = 0;
        var y = 0;

        foreach (var move in moves)
        {
            if (deltas.TryGetValue(move, out var delta))
            {
                x += delta.Dx;
                y += delta.Dy;
            }
        }

        return x == 0 && y == 0;
    }
}
