using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

// A board 5 columns wide and 4 rows tall on which four in a row win, test-only: under gravity
// Connect Four's own rule on a board small enough to solve exactly from the middle of a game.
internal readonly struct FiveByFourBoard : IMnkShape
{
    public static int Columns => 5;

    public static int Rows => 4;

    public static int InARow => 4;
}
