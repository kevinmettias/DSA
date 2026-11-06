using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

// The 4,3,3 game, test-only: tic-tac-toe with one column more, which the first player wins with
// perfect play where tic-tac-toe is a draw (the known result for k = 3 on any board larger than 3
// by 3), so solving it shows the lines decide the game.
internal readonly struct FourByThreeBoard : IMnkShape
{
    public static int Columns => 4;

    public static int Rows => 3;

    public static int InARow => 3;
}
