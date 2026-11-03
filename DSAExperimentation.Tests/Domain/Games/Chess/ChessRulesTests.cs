using DSAExperimentation.Domain.Games.Chess;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

public sealed partial class ChessRulesTests
{
    [Fact]
    public void BoardSize_IsEight() => Assert.Equal(8, ChessRules.BoardSize);
}
