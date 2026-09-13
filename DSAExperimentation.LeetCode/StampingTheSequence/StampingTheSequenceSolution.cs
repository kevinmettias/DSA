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
    public static int[] MovesToStampByListPrepend(string stamp, string target)
    {
        var windowCount = target.Length - stamp.Length + 1;

        if (windowCount <= 0)
        {
            return [];
        }

        var canvas = new StampCanvas(target.ToCharArray(), stamp, new bool[windowCount]);
        var order = new List<int>();
        var turnedCount = ReverseStamp(canvas, order);

        return turnedCount == target.Length ? order.ToArray() : [];
    }

    // The same simulation, recording into Stack<int>: pushing in discovery order and
    // popping into the result array reverses it for free.
    public static int[] MovesToStampByStackReverse(string stamp, string target)
    {
        var windowCount = target.Length - stamp.Length + 1;

        if (windowCount <= 0)
        {
            return [];
        }

        var canvas = new StampCanvas(target.ToCharArray(), stamp, new bool[windowCount]);
        var order = new RepoStampStack();
        var turnedCount = ReverseStamp(canvas, order);

        return turnedCount == target.Length ? UnwindToForwardOrder(order) : [];
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
            if (!StampAvailableWindows(canvas, order, ref turnedCount))
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
            if (!StampAvailableWindows(canvas, order, ref turnedCount))
            {
                break;
            }
        }

        return turnedCount;
    }

    // Tries every not-yet-done window once; returns whether any window stamped this
    // round (a false result means the reverse simulation is stuck).
    private static bool StampAvailableWindows(StampCanvas canvas, List<int> order, ref int turnedCount)
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

    private static bool StampAvailableWindows(StampCanvas canvas, RepoStampStack order, ref int turnedCount)
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
        if (!WindowMatchesStamp(canvas, start, out var hasLiveCharacter) || !hasLiveCharacter)
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

    // A window "matches" if every already-visible character agrees with the stamp at
    // that offset; hasLiveCharacter reports whether any character was still visible.
    private static bool WindowMatchesStamp(StampCanvas canvas, int start, out bool hasLiveCharacter)
    {
        hasLiveCharacter = false;

        for (var k = 0; k < canvas.Stamp.Length; k++)
        {
            var current = canvas.Chars[start + k];

            if (current == '?')
            {
                continue;
            }

            if (current != canvas.Stamp[k])
            {
                return false;
            }

            hasLiveCharacter = true;
        }

        return true;
    }

    // The mutable state one reverse-simulation run scans over: the canvas being
    // erased to '?', the stamp it is matched against, and which windows are done.
    private readonly record struct StampCanvas(char[] Chars, string Stamp, bool[] Done);
}
