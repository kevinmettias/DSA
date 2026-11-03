using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Mathematics.Lattices;

// The square lattice's algebra, checked against the named steps: each L-shaped jump is the sum of the
// moves its name spells, a quarter turn takes Up to Right, the mirror swaps Left and Right, and the
// group laws hold on every named step, so the algebra and the names cannot drift apart.
public sealed partial class GridStepTests
{
    private static readonly GridStep[] NamedSteps = [.. GridDirections.Octilinear, .. GridDirections.LShaped];

    [Fact]
    public void RotationOrder_IsFourQuarterTurns() => Assert.Equal(4, GridStep.RotationOrder);

    [Fact]
    public void AdditiveIdentity_AddedToEveryStep_ChangesNothing() =>
        Assert.All(NamedSteps, step => Assert.Equal(step, step + GridStep.AdditiveIdentity));

    [Fact]
    public void AdditionOperator_LShapedJumps_AreTheMovesTheirNamesSpell()
    {
        var up = GridStep.Up;
        var down = GridStep.Down;
        var left = GridStep.Left;
        var right = GridStep.Right;

        Assert.Equal(
            [up + up + left, up + up + right, up + left + left, up + right + right, down + left + left, down + right + right, down + down + left, down + down + right],
            GridDirections.LShaped.ToArray());
    }

    [Fact]
    public void AdditionOperator_DiagonalSteps_AreAnUpOrDownAndALeftOrRight() =>
        Assert.Equal(
            [GridStep.Up + GridStep.Left, GridStep.Up + GridStep.Right, GridStep.Down + GridStep.Left, GridStep.Down + GridStep.Right],
            GridDirections.Diagonal.ToArray());

    [Fact]
    public void UnaryNegationOperator_Up_IsDown() => Assert.Equal(GridStep.Down, -GridStep.Up);

    [Fact]
    public void UnaryNegationOperator_EveryStep_UndoesIt() =>
        Assert.All(NamedSteps, step => Assert.Equal(GridStep.AdditiveIdentity, step + -step));

    [Fact]
    public void Rotate_Up_IsRight() => Assert.Equal(GridStep.Right, GridStep.Rotate(GridStep.Up));

    [Fact]
    public void Rotate_FourQuarterTurns_ReturnEveryStep() =>
        Assert.All(NamedSteps, step => Assert.Equal(step, GridStep.Rotate(GridStep.Rotate(GridStep.Rotate(GridStep.Rotate(step))))));

    [Fact]
    public void Rotate_TwoQuarterTurns_IsTheOppositeStep() =>
        Assert.All(NamedSteps, step => Assert.Equal(-step, GridStep.Rotate(GridStep.Rotate(step))));

    [Fact]
    public void Rotate_OfTwoStepsTaken_IsBothStepsTurned() =>
        Assert.All(NamedSteps, step => Assert.Equal(
            GridStep.Rotate(step) + GridStep.Rotate(GridStep.UpUpLeft),
            GridStep.Rotate(step + GridStep.UpUpLeft)));

    [Fact]
    public void Reflect_Left_IsRight() => Assert.Equal(GridStep.Right, GridStep.Reflect(GridStep.Left));

    [Fact]
    public void Reflect_Up_StaysUp() => Assert.Equal(GridStep.Up, GridStep.Reflect(GridStep.Up));

    [Fact]
    public void Reflect_Twice_ReturnsEveryStep() =>
        Assert.All(NamedSteps, step => Assert.Equal(step, GridStep.Reflect(GridStep.Reflect(step))));

    // A mirror reverses which way a turn goes: mirror, turn clockwise, mirror back is a turn anticlockwise.
    [Fact]
    public void Reflect_AroundAQuarterTurn_TurnsTheOtherWay() =>
        Assert.All(NamedSteps, step => Assert.Equal(
            LatticeSymmetry.Turn(step, -1),
            GridStep.Reflect(GridStep.Rotate(GridStep.Reflect(step)))));
}
