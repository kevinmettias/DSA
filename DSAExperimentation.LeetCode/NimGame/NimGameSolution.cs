using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.NimGame;

// LeetCode 292. Nim Game: a player wins from `stoneCount` stones exactly when some
// move of 1-3 stones leaves the opponent facing a losing position - natural-looking
// recursion via this repo's Memoizer, no hand-rolled cache, the same shape
// ClimbingStairsSolution/HouseRobberSolution already use for their own recurrences.
// The recursion reduces to the well-known `stoneCount % 4 != 0` closed form, which
// the second strategy computes directly.
internal static class NimGameSolution
{
    // LC 292 lets a turn remove 1, 2, or 3 stones; the losing positions are exactly
    // the multiples of (MaxStonesPerTurn + 1).
    private const int TwoStoneRemoval = 2;
    private const int ThreeStoneRemoval = 3;
    private const int LosingPositionModulus = 4;

    public static bool CanWinByMemoizedRecursion(int stoneCount) =>
        Memoizer.Memoize<int, bool>(stoneCount, new WinFromStoneRemoval());

    public static bool CanWinByModuloFormula(int stoneCount) => stoneCount % LosingPositionModulus != 0;

    // The recurrence, as a named type: the mover wins from a pile exactly when some
    // removal of one to three stones leaves the opponent facing a losing pile, and
    // loses outright once there are no stones left to take.
    private sealed class WinFromStoneRemoval : IRecurrence<int, bool>
    {
        public bool Replay(int stones, IRecurrence<int, bool> rest) => stones switch
        {
            <= 0 => false,
            _ => !rest.Replay(stones - 1, rest)
                || (stones >= TwoStoneRemoval && !rest.Replay(stones - TwoStoneRemoval, rest))
                || (stones >= ThreeStoneRemoval && !rest.Replay(stones - ThreeStoneRemoval, rest)),
        };
    }
}
