namespace DSAExperimentation.LeetCode.FindTheWinningPlayerInCoinGame;

// LeetCode 3222. Find the Winning Player in Coin Game: each turn a player must
// pick coins worth exactly 115 from a pile of `seventyFiveCoinCount` 75-coins and
// `tenCoinCount` 10-coins. 75 + 4*10 = 115 is the only combination that sums to 115
// within the given constraints (a second 75-coin alone already overshoots), so every
// turn spends exactly one 75-coin and four 10-coins, and the game is really just
// counting how many such turns are affordable before either pile runs dry.
internal static class FindTheWinningPlayerInCoinGameSolution
{
    private const int CoinsPerTurn = 4;
    private const string Alice = "Alice";
    private const string Bob = "Bob";

    // The textbook answer: actually play it out, one turn at a time, spending
    // one 75-coin and four 10-coins per turn until a pile can't cover the next
    // turn. Deliberately written as a direct simulation rather than the derived
    // formula below - the arm the closed form has to agree with.
    public static string WinningPlayerBySimulation(int seventyFiveCoinCount, int tenCoinCount)
    {
        var turns = 0;

        while (seventyFiveCoinCount >= 1 && tenCoinCount >= CoinsPerTurn)
        {
            seventyFiveCoinCount--;
            tenCoinCount -= CoinsPerTurn;
            turns++;
        }

        var isOddTurnCount = turns % 2 == 1;
        return isOddTurnCount ? Alice : Bob;
    }

    // The game ends after exactly min(seventyFiveCoinCount, tenCoinCount / CoinsPerTurn)
    // turns - whichever pile runs out first - so the winner is a single parity check
    // with no loop at all.
    public static string WinningPlayerByTurnParity(int seventyFiveCoinCount, int tenCoinCount)
    {
        var turns = Math.Min(seventyFiveCoinCount, tenCoinCount / CoinsPerTurn);
        var isOddTurnCount = turns % 2 == 1;
        return isOddTurnCount ? Alice : Bob;
    }
}
