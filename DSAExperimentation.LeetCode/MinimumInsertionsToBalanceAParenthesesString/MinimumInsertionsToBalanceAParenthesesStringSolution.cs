using OpenerStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MinimumInsertionsToBalanceAParenthesesString;

// LeetCode 1541. Minimum Insertions to Balance a Parentheses String: a balanced unit
// here is '(' matched by TWO consecutive ')', so every ')' has to look one character
// ahead - a ')' with no adjacent partner needs an inserted closer, and a ')' with no
// pending opener needs an inserted opener too.
//
// The two strategies differ only in how the pending openers are held: a scalar
// "closers still owed" counter that never stores an opener at all (the textbook
// baseline), or this repo's Stack<char> holding each unmatched opener explicitly,
// the same LIFO primitive the sibling problem MinimumAddToMakeParenthesesValid uses.
internal static class MinimumInsertionsToBalanceAParenthesesStringSolution
{
    private const int ClosersPerOpener = 2;

    // Baseline: BCL-only running counter of closers still owed by the openers seen
    // so far. An odd count means the previous opener has been half-closed by a lone
    // ')', which is repaired by inserting the missing closer on the spot.
    public static int MinInsertionsByRunningCounter(string text)
    {
        var needed = 0;
        var insertions = 0;

        foreach (var bracket in text)
        {
            needed = bracket == '('
                ? ApplyOpener(needed, ref insertions)
                : ApplyCloser(needed, ref insertions);
        }

        return insertions + needed;
    }

    private static int ApplyOpener(int needed, ref int insertions)
    {
        needed += ClosersPerOpener;

        if (needed % ClosersPerOpener == 1)
        {
            insertions++;
            needed--;
        }

        return needed;
    }

    private static int ApplyCloser(int needed, ref int insertions)
    {
        needed--;

        if (needed == -1)
        {
            insertions++;
            needed = 1;
        }

        return needed;
    }

    // Every unmatched opener is pushed onto Stack<char>, so the count left standing
    // at the end says directly how many openers still owe two closers each.
    public static int MinInsertionsByOpenerStack(string text)
    {
        var scanner = new ParenScanner(text);

        while (scanner.HasNext)
        {
            scanner.Advance();
        }

        return scanner.Insertions + (scanner.RemainingOpeners * ClosersPerOpener);
    }

    // Consumes the string one balanced unit at a time: a ')' followed by a second
    // ')' closes an opener outright, otherwise the missing closer is inserted first.
    private sealed class ParenScanner(string text)
    {
        private readonly OpenerStack _openers = new();
        private int _index;

        public bool HasNext => _index < text.Length;

        public int Insertions { get; private set; }

        public int RemainingOpeners => _openers.Count;

        public void Advance()
        {
            if (text[_index] == '(')
            {
                _openers.Push(text[_index]);
                _index++;
                return;
            }

            var hasAdjacentCloser = _index + 1 < text.Length && text[_index + 1] == ')';

            if (!hasAdjacentCloser)
            {
                Insertions++;
            }

            if (!_openers.TryPop(out _))
            {
                Insertions++;
            }

            _index += hasAdjacentCloser ? ClosersPerOpener : 1;
        }
    }
}
