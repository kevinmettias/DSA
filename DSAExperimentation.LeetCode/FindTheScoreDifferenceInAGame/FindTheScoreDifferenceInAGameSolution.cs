namespace DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

// LeetCode 3847. Find the Score Difference in a Game: two players, the first one
// active. For each game i in order, the active and inactive players swap roles if
// nums[i] is odd, swap again if i is a 6th game (indices 5, 11, 17, ...), and then
// the active player gains nums[i]. Return the first player's total minus the
// second's.
//
// Both strategies are one O(n) pass; they differ in what the pass keeps. The
// simulation keeps who is active and both totals, as the statement tells it. The
// parity pass keeps only whether an odd number of swaps has happened, and adds
// nums[i] into one running difference with the sign that parity gives it.
internal static class FindTheScoreDifferenceInAGameSolution
{
    // Every 6th game swaps the players: games 5, 11, 17, ...
    private const int SwapPeriod = 6;

    private const int PlayerCount = 2;

    // The statement's own rules, step for step: the active player's index (0 for the
    // first player, 1 for the second), moved by each swap, and a running total per player.
    public static int ScoreDifferenceByRoleSimulation(int[] nums)
    {
        var totals = new int[PlayerCount];
        var activePlayer = 0;

        for (var game = 0; game < nums.Length; game++)
        {
            activePlayer = ActivePlayerAfterSwaps(activePlayer, nums[game], game);
            totals[activePlayer] += nums[game];
        }

        return totals[0] - totals[1];
    }

    // Each swap hands play to the other player: once for odd points, and once more on a
    // 6th game.
    private static int ActivePlayerAfterSwaps(int activePlayer, int points, int game)
    {
        var player = activePlayer;

        if (IsOdd(points))
        {
            player = OtherPlayer(player);
        }

        if (IsSwapGame(game))
        {
            player = OtherPlayer(player);
        }

        return player;
    }

    private static bool IsOdd(int points) => (points & 1) == 1;

    private static int OtherPlayer(int player) => PlayerCount - 1 - player;

    // The first player is active exactly when an even number of swaps has happened, so
    // game i adds nums[i] to the difference when that count is even and subtracts it
    // when odd: one parity bit and one signed sum, with no branch on who is active.
    public static int ScoreDifferenceBySwapParity(int[] nums)
    {
        var swapParity = 0;
        var difference = 0;

        for (var game = 0; game < nums.Length; game++)
        {
            swapParity ^= nums[game] & 1;

            if (IsSwapGame(game))
            {
                swapParity ^= 1;
            }

            difference += (1 - (2 * swapParity)) * nums[game];
        }

        return difference;
    }

    private static bool IsSwapGame(int game) => (game + 1) % SwapPeriod == 0;
}
