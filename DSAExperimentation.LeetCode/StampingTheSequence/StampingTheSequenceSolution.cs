using RepoStampStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.StampingTheSequence;

// LeetCode 936. Stamping The Sequence: recover a sequence of stamp start indices
// that turns a blank canvas into target, or report that none exists. Both strategies
// run the identical reverse simulation - repeatedly find a window that could have
// been the LAST stamp applied (every still-visible character matches the stamp at
// that offset, with at least one character still live), turn it to '?', and record
// its start index - which discovers moves in reverse chronological order, so every
// discovered index has to be put back into forward order.
//
// MovesToStampByListPrepend does that with a plain List<int>.Insert(0, i): O(k) of
// shifting per insertion, O(m^2) over m discovered stamps. MovesToStampByStackReverse
// records into this repo's own Stack<int> and unwinds it once at the end, since LIFO
// order IS forward chronological order - O(m) total. Both return the same move
// sequence; LeetCode accepts any sequence that replays onto target.
internal static class StampingTheSequenceSolution
{
    // The textbook arm: a BCL List that is kept in forward order as it grows, so no
    // reversal pass is needed and the shifting cost is paid instead. Deliberately
    // written without this repo's primitives - it is the arm the stack below has to
    // justify itself against.
    public static int[] MovesToStampByListPrepend(StampPattern stamp, TargetText target)
    {
        var windowCount = target.Text.Length - stamp.Text.Length + 1;

        if (windowCount <= 0)
        {
            return [];
        }

        var canvas = new StampCanvas(target.Text.ToCharArray(), stamp.Text, new bool[windowCount]);
        var order = new List<int>();
        var turnedCount = ReverseStamp(canvas, order);

        return turnedCount == target.Text.Length ? order.ToArray() : NoMoves();
    }

    // The same simulation, recording into Stack<int>: pushing in discovery order and
    // popping into the result array reverses it for free.
    public static int[] MovesToStampByStackReverse(StampPattern stamp, TargetText target)
    {
        var windowCount = target.Text.Length - stamp.Text.Length + 1;

        if (windowCount <= 0)
        {
            return [];
        }

        var canvas = new StampCanvas(target.Text.ToCharArray(), stamp.Text, new bool[windowCount]);
        var order = new RepoStampStack();
        var turnedCount = ReverseStamp(canvas, order);

        return turnedCount == target.Text.Length ? UnwindToForwardOrder(order) : NoMoves();
    }

    // The stack pops in reverse discovery order, which is exactly forward
    // chronological stamping order.
    private static int[] UnwindToForwardOrder(RepoStampStack order)
    {
        var moves = new int[order.Count];

        for (var k = 0; k < moves.Length; k++)
        {
            order.TryPop(out moves[k]);
        }

        return moves;
    }

    // Runs rounds until every character of target has been turned to '?' (success) or
    // a round stamps nothing (stuck); the count of turned characters reports which.
    private static int ReverseStamp(StampCanvas canvas, List<int> order)
    {
        var turnedCount = 0;

        for (var round = 0; round < canvas.Done.Length && turnedCount < canvas.Chars.Length; round++)
        {
            if (!TryStampAvailableWindows(canvas, order, ref turnedCount))
            {
                break;
            }
        }

        return turnedCount;
    }

    private static int ReverseStamp(StampCanvas canvas, RepoStampStack order)
    {
        var turnedCount = 0;

        for (var round = 0; round < canvas.Done.Length && turnedCount < canvas.Chars.Length; round++)
        {
            if (!TryStampAvailableWindows(canvas, order, ref turnedCount))
            {
                break;
            }
        }

        return turnedCount;
    }

    // Tries every not-yet-done window once; returns whether any window stamped this
    // round (a false result means the reverse simulation is stuck).
    private static bool TryStampAvailableWindows(StampCanvas canvas, List<int> order, ref int turnedCount)
    {
        var stampedThisRound = false;

        for (var i = 0; i < canvas.Done.Length; i++)
        {
            if (canvas.Done[i] || !TryStampWindow(canvas, i, ref turnedCount))
            {
                continue;
            }

            canvas.Done[i] = true;
            stampedThisRound = true;
            order.Insert(0, i);
        }

        return stampedThisRound;
    }

    private static bool TryStampAvailableWindows(StampCanvas canvas, RepoStampStack order, ref int turnedCount)
    {
        var stampedThisRound = false;

        for (var i = 0; i < canvas.Done.Length; i++)
        {
            if (canvas.Done[i] || !TryStampWindow(canvas, i, ref turnedCount))
            {
                continue;
            }

            canvas.Done[i] = true;
            stampedThisRound = true;
            order.Push(i);
        }

        return stampedThisRound;
    }

    // A window is stampable only if every character still visible (not yet '?')
    // matches the stamp at that offset, and at least one character is still visible
    // (otherwise this window is a no-op re-stamp of already-done work).
    private static bool TryStampWindow(StampCanvas canvas, int start, ref int turnedCount)
    {
        if (ProbeWindow(canvas, start) != WindowProbe.Stampable)
        {
            return false;
        }

        for (var k = 0; k < canvas.Stamp.Length; k++)
        {
            if (canvas.Chars[start + k] != '?')
            {
                canvas.Chars[start + k] = '?';
                turnedCount++;
            }
        }

        return true;
    }

    // Probing one window answers one question with three outcomes, and the caller
    // needs all three told apart: a visible character can disagree with the stamp,
    // every visible character can agree with nothing left to erase, or the window
    // can be genuinely stampable. That is a verdict, not a bool plus a flag.
    private static WindowProbe ProbeWindow(StampCanvas canvas, int start)
    {
        var hasLiveCharacter = false;

        for (var k = 0; k < canvas.Stamp.Length; k++)
        {
            var current = canvas.Chars[start + k];

            if (current == '?')
            {
                continue;
            }

            if (current != canvas.Stamp[k])
            {
                return WindowProbe.ConflictingCharacter;
            }

            hasLiveCharacter = true;
        }

        return hasLiveCharacter ? WindowProbe.Stampable : WindowProbe.AlreadyErased;
    }

    // The empty move sequence a caller gets back when target cannot be stamped at all.
    private static int[] NoMoves() => [];

    // The mutable state one reverse-simulation run scans over: the canvas being
    // erased to '?', the stamp it is matched against, and which windows are done.
    private readonly record struct StampCanvas(char[] Chars, string Stamp, bool[] Done);

    // The three outcomes of probing one window. Only Stampable lets the caller turn
    // characters to '?' and record the stamp; the other two are the ways it declines.
    private enum WindowProbe
    {
        ConflictingCharacter,
        AlreadyErased,
        Stampable,
    }

    // The two sides of a stamping run, named for what each is in this problem rather
    // than left as two adjacent `string` positions a caller could hand over the wrong
    // way round with the compiler none the wiser: `stamp` is the pattern pressed onto
    // the canvas, `target` the string the canvas has to end up spelling.
    internal readonly record struct StampPattern(string Text);

    internal readonly record struct TargetText(string Text);
}
