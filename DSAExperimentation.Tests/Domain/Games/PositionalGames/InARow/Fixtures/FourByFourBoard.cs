using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

// A 4 by 4 board on which three in a row win, test-only: under gravity a Connect Four small enough
// that negamax values every position of it, so a depth-limited search can be checked against the
// exact value from the empty board.
internal readonly struct FourByFourBoard : IMnkShape
{
    public static int Columns => 4;

    public static int Rows => 4;

    public static int InARow => 3;
}
