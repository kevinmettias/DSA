namespace DSAExperimentation.LeetCode.EliminationGame;

// LeetCode 390. Elimination Game: starting from [1..n], repeatedly discard every
// other surviving number - alternating which end each pass starts from - until one
// number remains.
//
// The closed-form strategy tracks only the head element and the current stride
// against each halving of the remaining count, never materializing the list at all
// - the same "lighter repo-primitive fit" already accepted for Pow(x, n) /
// Rectangle Area. The list-simulation strategy is the naive baseline it has to
// beat: rebuild the surviving-numbers list every round. Its internals stay a plain
// List<int> rather than this repo's DynamicArray<int> - a baseline represents "what
// you would write without this repo", and there is no input container here for a
// repo type to occupy (RectangleArea's TotalAreaByUnitGridCoverageCount is the same
// call, made explicitly for the same reason).
internal static class EliminationGameSolution
{
    private const int EliminationStride = 2;

    // Every pass keeps only the numbers at odd (0-indexed) positions of whichever
    // direction it is walking; a right-to-left pass runs that same "keep odd
    // positions" rule against the reversed list, then reverses back.
    public static int LastRemainingByHeadStepArithmetic(int n)
    {
        var head = 1;
        var step = 1;
        var leftToRight = true;
        var remaining = n;

        while (remaining > 1)
        {
            if (leftToRight || remaining % EliminationStride == 1)
            {
                head += step;
            }

            remaining /= EliminationStride;
            step *= EliminationStride;
            leftToRight = !leftToRight;
        }

        return head;
    }

    public static int LastRemainingByListSimulation(int n)
    {
        var current = new List<int>();

        for (var i = 1; i <= n; i++)
        {
            current.Add(i);
        }

        var leftToRight = true;

        while (current.Count > 1)
        {
            (current, leftToRight) = RunPass(current, leftToRight);
        }

        return current[0];
    }

    private static (List<int> Current, bool LeftToRight) RunPass(List<int> current, bool leftToRight)
    {
        if (!leftToRight)
        {
            current = Reverse(current);
        }

        current = KeepOddPositions(current);

        if (!leftToRight)
        {
            current = Reverse(current);
        }

        return (current, !leftToRight);
    }

    private static List<int> KeepOddPositions(List<int> values)
    {
        var kept = new List<int>();

        for (var i = 1; i < values.Count; i += EliminationStride)
        {
            kept.Add(values[i]);
        }

        return kept;
    }

    private static List<int> Reverse(List<int> values)
    {
        var reversed = new List<int>();

        for (var i = values.Count - 1; i >= 0; i--)
        {
            reversed.Add(values[i]);
        }

        return reversed;
    }
}
