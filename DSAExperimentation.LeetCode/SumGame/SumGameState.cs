namespace DSAExperimentation.LeetCode.SumGame;

// LC 1927's board collapsed to the only three numbers the game's outcome depends
// on: how many '?' remain on each half, and how far ahead the left half's known
// digits already are. WHICH '?' position a player fills never matters - only which
// half it is on and which digit is written there - so every distinct fill-in order
// that leaves the same three numbers is the same position, which is what turns an
// exponential move-order space into a cache small enough for a plain dictionary.
//
// A prepared instance is also what every strategy's hoisted overload takes
// (ARCHITECTURE.md section 17.4), so a benchmark charges the digit scan to
// [GlobalSetup] rather than to the search it measures. It is a record struct, so it
// carries the structural equality Memoizer's cache keys on for free, and it is not
// a string, so the two overloads can never be ambiguous.
internal readonly record struct SumGameState(int LeftBlanks, int RightBlanks, int SumDifference)
{
    private const char Blank = '?';

    // The board is split into two halves, which is the only thing the game
    // compares - a digit's own position within its half never matters.
    private const int Halves = 2;

    // Turn parity is derived from how many blanks are left versus how many the
    // board started with, so the total is the one thing a resolved state still
    // needs from the original board.
    public int TotalBlanks => LeftBlanks + RightBlanks;

    // num has even length (LeetCode's own constraint), so the halves split cleanly.
    public static SumGameState Of(string num)
    {
        var half = num.Length / Halves;
        var (leftSum, leftBlanks) = Tally(num, 0, half);
        var (rightSum, rightBlanks) = Tally(num, half, num.Length);

        return new SumGameState(leftBlanks, rightBlanks, leftSum - rightSum);
    }

    private static (int Sum, int Blanks) Tally(string num, int start, int end)
    {
        var sum = 0;
        var blanks = 0;

        for (var i = start; i < end; i++)
        {
            if (num[i] == Blank)
            {
                blanks++;
            }
            else
            {
                sum += num[i] - '0';
            }
        }

        return (sum, blanks);
    }

    // Writing a digit on the left moves the difference in the left half's favor;
    // writing the same digit on the right moves it the other way. Both consume the
    // blank they filled, which is what makes the state space well-founded.
    public SumGameState FillLeft(int digit) => new(LeftBlanks - 1, RightBlanks, SumDifference + digit);

    public SumGameState FillRight(int digit) => new(LeftBlanks, RightBlanks - 1, SumDifference - digit);
}
