using DSAExperimentation.Domain.Games.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.Games.SlidingPuzzle;

public sealed partial class EightPuzzleTests
{
    [Fact]
    public void Rows_AreThree() => Assert.Equal(3, EightPuzzle.Rows);

    [Fact]
    public void Columns_AreThree() => Assert.Equal(3, EightPuzzle.Columns);
}
