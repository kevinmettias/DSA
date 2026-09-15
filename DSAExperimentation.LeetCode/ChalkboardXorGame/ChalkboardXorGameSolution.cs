using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ChalkboardXorGame;

// LeetCode 810. Chalkboard XOR Game: players alternately erase one number; a player
// loses if the xor of every remaining number is 0 at the START of their turn (and
// wins immediately if it is already 0 when they are handed the board). The state is
// "which subset of indices is still on the board", so the full game is a minimax
// recursion over a bitmask.
//
// Three strategies over the same game: the textbook unmemoized recursion, that same
// recursion keyed on the remaining-elements bitmask through this repo's own
// Memoizer, and the O(n) closed form the recursion provably reduces to - the same
// "recursion reduces to a closed form" pairing NimGameSolution already makes.
internal static class ChalkboardXorGameSolution
{
    private const int EvenCountModulus = 2;

    // The textbook baseline: the same minimax recursion with no cache, so every
    // erase ORDER that reaches a given remaining subset re-explores it (O(n!) worst
    // case). Deliberately written without this repo's primitives - it is the arm the
    // other two strategies have to justify themselves against.
    public static bool AliceWinsByBruteForceRecursion(int[] nums) =>
        CurrentPlayerWinsUnmemoized(FullMask(nums), nums);

    // The same recursion routed through this repo's own Memoizer, keyed on the
    // remaining-elements bitmask, so each of the O(2^n) subsets is evaluated once no
    // matter how many erase orders reach it - the same int-bitmask memo state
    // CanIWinSolution uses for its own used-numbers mask.
    public static bool AliceWinsByMemoizedRecursion(int[] nums) =>
        Memoizer.Memoize<int, bool>(FullMask(nums), new CurrentPlayerWinsFromSubset(nums));

    // The closed form the recursion reduces to: the player to move wins iff the
    // board's xor is already 0, or an even number of elements is on it.
    public static bool AliceWinsByXorParityFormula(int[] nums)
    {
        var xor = 0;
        foreach (var num in nums)
        {
            xor ^= num;
        }

        return xor == 0 || nums.Length % EvenCountModulus == 0;
    }

    private static bool CurrentPlayerWinsUnmemoized(int mask, int[] nums)
    {
        if (XorOf(mask, nums) == 0)
        {
            return true;
        }

        for (var i = 0; i < nums.Length; i++)
        {
            var bit = 1 << i;
            if ((mask & bit) == 0)
            {
                continue;
            }

            var remaining = mask & ~bit;
            if (XorOf(remaining, nums) != 0 && !CurrentPlayerWinsUnmemoized(remaining, nums))
            {
                return true;
            }
        }

        return false;
    }

    private static int FullMask(int[] nums) => (1 << nums.Length) - 1;

    private static int XorOf(int mask, int[] nums)
    {
        var result = 0;
        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                result ^= nums[i];
            }
        }

        return result;
    }

    // The recurrence, named: one position of the game, where the player to move loses
    // at a xor of 0 and otherwise wins as soon as some erase leaves the opponent a
    // position this same rule reports as lost. The board's numbers are the whole of
    // what the rule needs from its caller, so they are the constructor's only input.
    private sealed class CurrentPlayerWinsFromSubset(int[] nums) : IRecurrence<int, bool>
    {
        public bool Replay(int mask, IRecurrence<int, bool> rest)
        {
            if (XorOf(mask, nums) == 0)
            {
                return true;
            }

            for (var i = 0; i < nums.Length; i++)
            {
                var bit = 1 << i;
                if ((mask & bit) == 0)
                {
                    continue;
                }

                var remaining = mask & ~bit;
                if (XorOf(remaining, nums) != 0 && !rest.Replay(remaining, rest))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
