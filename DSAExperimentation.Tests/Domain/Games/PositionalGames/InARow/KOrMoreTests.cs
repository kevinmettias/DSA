using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

public sealed partial class KOrMoreTests
{
    [Fact]
    public void IsOverlineAWin_IsTrue() => Assert.True(KOrMore.IsOverlineAWin);
}
