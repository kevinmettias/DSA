using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

// A board 16 columns wide, test-only: one column too wide for a 256-bit board with a column to
// spare between rows, which InARowLines must refuse.
internal readonly struct SixteenByTwoBoard : IMnkShape
{
    public static int Columns => 16;

    public static int Rows => 2;

    public static int InARow => 2;
}
