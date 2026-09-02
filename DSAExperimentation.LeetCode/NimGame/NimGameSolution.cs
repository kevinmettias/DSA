using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.NimGame;

// LeetCode 292. Nim Game: canWin(n) = there exists a move of 1-3 stones that leaves
// the opponent facing a losing position - natural-looking recursion via this repo's
// Memoizer, no hand-rolled cache, the same shape ClimbingStairsSolution/
// HouseRobberSolution already use for their own recurrences. The recursion reduces
// to the well-known n % 4 != 0 closed form, which the second strategy computes
// directly.
internal static class NimGameSolution
{
    // LC 292 lets a turn remove 1, 2, or 3 stones; the losing positions are exactly
    // the multiples of (MaxStonesPerTurn + 1).
    private const int TwoStoneRemoval = 2;
    private const int ThreeStoneRemoval = 3;
    private const int LosingPositionModulus = 4;

    public static bool CanWinByMemoizedRecursion(int n) =>
        Memoizer.Memoize<int, bool>(n, (stones, canWin) => stones switch
        {
            <= 0 => false,
            _ => !canWin(stones - 1)
                || (stones >= TwoStoneRemoval && !canWin(stones - TwoStoneRemoval))
                || (stones >= ThreeStoneRemoval && !canWin(stones - ThreeStoneRemoval)),
        });

    public static bool CanWinByModuloFormula(int n) => n % LosingPositionModulus != 0;
}
