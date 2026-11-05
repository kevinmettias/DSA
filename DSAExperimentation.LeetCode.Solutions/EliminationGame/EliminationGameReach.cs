namespace DSAExperimentation.LeetCode.Solutions.EliminationGame;

// The reach of EliminationGameSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. LastRemainingByListSimulation takes 236.3 ms over the 66 of the catalog's
// 77 cases with a value of numberCount up to 7,812,500 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class EliminationGameReach
{
    private const int LargestNumberCount = 7_812_500;

    public static bool IsWithin(int numberCount) =>
        numberCount <= LargestNumberCount;
}
