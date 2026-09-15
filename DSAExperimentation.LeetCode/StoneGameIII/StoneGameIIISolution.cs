using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.StoneGameIII;

// LeetCode 1406. Stone Game III: the player to move from stoneValue[index:] takes a
// prefix of 1, 2, or 3 stones, scoring that prefix's sum minus whatever the opponent
// can force from the state that follows - the same minimax recurrence
// StoneGameIISolution (LC 1140) and StoneGameSolution (LC 877) run, keyed on the
// single index instead of (Index, M) and with a fixed 3-way inner loop instead of a
// bound-M window.
//
// The recurrence's value is the score *difference* the player to move can force; LC
// 1406 asks for the name of the winner, so both strategies map that difference onto
// "Alice"/"Bob"/"Tie" before returning. The two differ only in whether the states
// are cached.
internal static class StoneGameIIISolution
{
    // LC 1406's rule: a turn takes 1, 2, or 3 stones from the front.
    private const int MaxTakePerTurn = 3;

    // LC 1406 reports the winner by name, or "Tie" when neither player can force a
    // positive difference.
    private const string Alice = "Alice";
    private const string Bob = "Bob";
    private const string Tie = "Tie";

    // The textbook baseline: plain minimax recursion over the index with no caching,
    // so the same index recurs through every take-1/2/3 pick sequence that reaches
    // it and the cost grows tribonacci-shaped. Deliberately written without this
    // repo's primitives - it is the arm the memoized strategy has to justify itself
    // against.
    public static string WinnerByUnmemoizedRecursion(int[] stoneValue)
    {
        var difference = Best(stoneValue, 0);
        return DetermineWinner(difference);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed on the exact
    // index the recurrence branches on, collapsing the exponential recursion to one
    // evaluation per reachable index.
    public static string WinnerByMemoizedRecursion(int[] stoneValue)
    {
        var difference = Memoizer.Memoize<int, int>(
            0,
            new PrefixPickOrder(stoneValue));

        return DetermineWinner(difference);
    }

    // The rule, named: the player to move from an index takes a prefix of 1, 2, or 3
    // stones, scoring that prefix minus whatever the opponent can then force from the
    // state it leaves - the best of the three. The stones are the whole of what the rule
    // needs from its caller, so they are the constructor's only input.
    private sealed class PrefixPickOrder(int[] stoneValue) : IRecurrence<int, int>
    {
        public int Replay(int index, IRecurrence<int, int> rest)
        {
            var stoneCount = stoneValue.Length;

            if (index >= stoneCount)
            {
                return 0;
            }

            var result = int.MinValue;
            var takenSum = 0;

            for (var take = 1; take <= MaxTakePerTurn && index + take <= stoneCount; take++)
            {
                takenSum += stoneValue[index + take - 1];
                result = Math.Max(result, takenSum - rest.Replay(index + take, rest));
            }

            return result;
        }
    }

    private static int Best(int[] stoneValue, int index)
    {
        var stoneCount = stoneValue.Length;

        if (index >= stoneCount)
        {
            return 0;
        }

        var result = int.MinValue;
        var takenSum = 0;

        for (var take = 1; take <= MaxTakePerTurn && index + take <= stoneCount; take++)
        {
            takenSum += stoneValue[index + take - 1];
            result = Math.Max(result, takenSum - Best(stoneValue, index + take));
        }

        return result;
    }

    // The recurrence returns Alice's score minus Bob's under optimal play from the
    // full board, so the sign of that difference is the answer LC 1406 asks for.
    private static string DetermineWinner(int difference)
        => difference switch
        {
            > 0 => Alice,
            < 0 => Bob,
            _ => Tie,
        };
}
