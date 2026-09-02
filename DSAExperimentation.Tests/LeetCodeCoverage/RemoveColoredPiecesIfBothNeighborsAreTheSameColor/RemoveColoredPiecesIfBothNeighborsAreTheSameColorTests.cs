using DSAExperimentation.DataStructures.Buffers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

// LeetCode 2038. Remove Colored Pieces if Both Neighbors are the Same Color: a
// move only ever removes an interior piece from a maximal run of same-colored
// pieces, and removing one never changes how many total removals that run can
// ever yield - a run of length L always yields exactly max(L-2, 0) moves,
// regardless of removal order, since the run just gets shorter by one and stays
// homogeneous. The "game" therefore reduces to counting each player's fixed move
// budget via this repo's own ContiguousGroupBuffer, the same run-grouping
// primitive ContiguousGroupBufferTests exercises directly. Alice moves first and
// only ever has 'A' moves available, Bob only 'B' moves; since the two budgets
// never interact, Alice wins iff her budget is strictly larger than Bob's.
public sealed partial class RemoveColoredPiecesIfBothNeighborsAreTheSameColorTests
{
    [Fact]
    public void WinnerOfGame_ClassicExample_AliceHasMoreMovesAndWins()
    {
        var wins = WinnerOfGame("AAABABB");

        Assert.True(wins);
    }

    [Fact]
    public void WinnerOfGame_OnlyTwoSameColorPieces_NeitherPlayerCanMoveAndAliceLoses()
    {
        var wins = WinnerOfGame("AA");

        Assert.False(wins);
    }

    [Fact]
    public void WinnerOfGame_BobsRunGivesHimMoreMoves_AliceLoses()
    {
        var wins = WinnerOfGame("ABBBBBBBAAA");

        Assert.False(wins);
    }

    private static bool WinnerOfGame(string colors)
    {
        var buffer = new ContiguousGroupBuffer<char, char>();
        var budgets = new Dictionary<char, int> { ['A'] = 0, ['B'] = 0 };

        void Accumulate(IReadOnlyList<char> items, char key) => budgets[key] += Math.Max(0, items.Count - 2);

        foreach (var color in colors)
        {
            buffer.Add(color, color, Accumulate);
        }

        buffer.Flush(Accumulate);

        return budgets['A'] > budgets['B'];
    }
}
