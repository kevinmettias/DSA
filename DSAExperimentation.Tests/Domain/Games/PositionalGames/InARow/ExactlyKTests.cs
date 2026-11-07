using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

public sealed partial class ExactlyKTests
{
    [Fact]
    public void IsOverlineAWin_IsFalse() => Assert.False(ExactlyK.IsOverlineAWin);
}
