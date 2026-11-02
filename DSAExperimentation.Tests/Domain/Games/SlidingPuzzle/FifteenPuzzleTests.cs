using DSAExperimentation.Domain.Games.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.Games.SlidingPuzzle;

public sealed partial class FifteenPuzzleTests
{
    [Fact]
    public void Rows_AreFour() => Assert.Equal(4, FifteenPuzzle.Rows);

    [Fact]
    public void Columns_AreFour() => Assert.Equal(4, FifteenPuzzle.Columns);
}
