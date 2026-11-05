using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

public sealed partial class PieceMoveTests
{
    [Fact]
    public void Leap_TakesAStepOnceToAnEmptySquareOrAsACapture()
    {
        var move = PieceMove.Leap(GridDirections.LShaped);

        Assert.Equal(((int?)1, MoveConditions.MovesOrCaptures), (move.Reach, move.When));
    }

    [Fact]
    public void Ride_RepeatsAStepWithoutLimitToAnEmptySquareOrAsACapture()
    {
        var move = PieceMove.Ride(GridDirections.Orthogonal);

        Assert.Equal(((int?)null, MoveConditions.MovesOrCaptures), (move.Reach, move.When));
    }
}
