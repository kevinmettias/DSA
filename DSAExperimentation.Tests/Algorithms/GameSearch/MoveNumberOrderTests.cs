using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

public sealed partial class MoveNumberOrderTests
{
    [Fact]
    public void Order_WritesTheMovesInTheGamesNumbering()
    {
        var moves = new int[5];
        default(MoveNumberOrder<int>).Order(0, moves);

        Assert.Equal([0, 1, 2, 3, 4], moves);
    }
}
