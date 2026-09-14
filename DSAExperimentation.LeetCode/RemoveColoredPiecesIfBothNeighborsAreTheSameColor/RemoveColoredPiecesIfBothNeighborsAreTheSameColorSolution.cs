using DSAExperimentation.DataStructures.Buffers;

namespace DSAExperimentation.LeetCode.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

// LeetCode 2038. Remove Colored Pieces if Both Neighbors are the Same Color: Alice
// removes an 'A' whose two neighbors are both 'A', Bob removes a 'B' whose two
// neighbors are both 'B', Alice moves first, and whoever cannot move loses. Return
// true when Alice wins.
//
// The two strategies differ in whether they play the game or count it. A move only
// ever takes an interior piece out of a maximal run of same-colored pieces, and
// doing so leaves the run homogeneous and one shorter - so a run of length L always
// yields exactly max(L - 2, 0) moves no matter what order they are taken in, and
// the two players' budgets never interact. Alice therefore wins iff her budget is
// strictly larger than Bob's, which is one linear grouping pass over the string.
internal static class RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution
{
    // Alice's pieces and Bob's.
    private const char AliceColor = 'A';
    private const char BobColor = 'B';

    // A run's two endpoints always keep a differently-colored (or absent) neighbor
    // on one side, so they are never removable.
    private const int RunEndpointCount = 2;

    private const int NoMove = -1;

    // The textbook answer: actually play the game out, scanning for the next
    // removable piece and deleting it from a List<char> every turn, O(n^2) overall.
    // Deliberately written without this repo's primitives - it is the arm the
    // counting strategy below has to justify itself against.
    public static bool WinnerOfGameByGameSimulation(string colors)
    {
        var pieces = colors.ToList();
        var aliceTurn = true;

        while (true)
        {
            var moveIndex = FindRemovableIndex(pieces, aliceTurn ? AliceColor : BobColor);

            if (moveIndex == NoMove)
            {
                // The player to move is stuck, so the other one wins.
                return !aliceTurn;
            }

            pieces.RemoveAt(moveIndex);
            aliceTurn = !aliceTurn;
        }
    }

    private static int FindRemovableIndex(List<char> pieces, char target)
    {
        for (var i = 1; i < pieces.Count - 1; i++)
        {
            if (pieces[i] == target && pieces[i - 1] == target && pieces[i + 1] == target)
            {
                return i;
            }
        }

        return NoMove;
    }

    // Group the string into maximal same-color runs with this repo's own
    // ContiguousGroupBuffer - the same primitive ContiguousGroupBufferTests
    // exercises directly - and add max(L - 2, 0) to that color's move budget as each
    // run closes. One O(n) pass, no simulation.
    public static bool WinnerOfGameByRunLengthCounting(string colors)
    {
        var buffer = new ContiguousGroupBuffer<char, char>();
        var budgets = new Dictionary<char, int> { [AliceColor] = 0, [BobColor] = 0 };

        void Accumulate(IReadOnlyList<char> run, char color) =>
            budgets[color] += Math.Max(0, run.Count - RunEndpointCount);

        foreach (var color in colors)
        {
            buffer.Add(color, color, Accumulate);
        }

        buffer.Flush(Accumulate);

        return budgets[AliceColor] > budgets[BobColor];
    }
}
