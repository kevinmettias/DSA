using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.BaseballGame;

// LeetCode 682. Baseball Game: replay a list of round-scoring operations and sum
// the valid scores that remain after "C"/"D"/"+" are applied.
//
// Both strategies replay the same op list; they differ only in what holds the
// running record - a hand-managed array plus a manually-tracked top-of-record
// index (the baseline every solver reaches for first) vs. this repo's own
// Stack<int> (ValidParentheses/NextGreaterElementI precedent), whose
// Push/TryPop/TryPeek read exactly as the rules do.
internal static class BaseballGameSolution
{
    private const string CancelOp = "C";
    private const string DoubleOp = "D";
    private const string SumOp = "+";
    private const int DoublingMultiplier = 2;
    private const int SecondToLastOffset = 2;

    public static int CalPointsByManualArrayCursor(string[] ops)
    {
        var record = new int[ops.Length];
        var top = 0;

        foreach (var op in ops)
        {
            top = ApplyArrayOperation(record, op, top);
        }

        return SumRange(record, top);
    }

    // One round against the hand-managed array: "C" discards the last score, "D" doubles it,
    // "+" sums the last two, and any other op is the round's own score. `top` is the record's
    // next free slot and comes back updated.
    private static int ApplyArrayOperation(int[] record, string op, int top)
    {
        switch (op)
        {
            case CancelOp:
                top--;
                break;
            case DoubleOp:
                record[top] = record[top - 1] * DoublingMultiplier;
                top++;
                break;
            case SumOp:
                record[top] = record[top - 1] + record[top - SecondToLastOffset];
                top++;
                break;
            default:
                record[top] = int.Parse(op);
                top++;
                break;
        }

        return top;
    }

    private static int SumRange(int[] record, int count)
    {
        var total = 0;

        for (var i = 0; i < count; i++)
        {
            total += record[i];
        }

        return total;
    }

    public static int CalPointsByStackReplay(string[] ops)
    {
        var record = new RepoIntStack();

        foreach (var op in ops)
        {
            ApplyStackOperation(op, record);
        }

        return DrainStackTotal(record);
    }

    private static void ApplyStackOperation(string op, RepoIntStack record)
    {
        switch (op)
        {
            case CancelOp:
                record.TryPop(out _);
                break;
            case DoubleOp:
                record.TryPeek(out var last);
                record.Push(last * DoublingMultiplier);
                break;
            case SumOp:
                PushSumOfLastTwoScores(record);
                break;
            default:
                record.Push(int.Parse(op));
                break;
        }
    }

    // "+" is the one op that reads the record without consuming it: pop the last two
    // scores, then put both back with their sum on top.
    private static void PushSumOfLastTwoScores(RepoIntStack record)
    {
        record.TryPop(out var top);
        record.TryPop(out var second);
        record.Push(second);
        record.Push(top);
        record.Push(top + second);
    }

    private static int DrainStackTotal(RepoIntStack record)
    {
        var total = 0;

        while (record.TryPop(out var value))
        {
            total += value;
        }

        return total;
    }
}
