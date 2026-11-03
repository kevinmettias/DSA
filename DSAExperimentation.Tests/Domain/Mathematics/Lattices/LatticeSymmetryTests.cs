using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Mathematics.Lattices;

// Turning and orbits on two lattices: the square one GridStep steps on, and a test-only hexagonal one
// that proves the contract is not the square's alone, as the order-2 Sudoku grid proves Sudoku's.
// On the square lattice each neighbour table is one orbit, so the tables agree with the lattice's
// symmetry; on the hexagonal one a cell has six neighbours, six diagonal neighbours, and a step on no
// mirror line has twelve images.
public sealed partial class LatticeSymmetryTests
{
    private static readonly HexStep HexNeighbour = new(1, 0);
    private static readonly HexStep HexDiagonal = new(2, -1);
    private static readonly HexStep HexOffEveryMirror = new(2, 1);

    [Fact]
    public void Turn_ByAWholeTurn_ReturnsTheStep()
    {
        Assert.All(GridDirections.LShaped, step => Assert.Equal(step, LatticeSymmetry.Turn(step, GridStep.RotationOrder)));
        Assert.Equal(HexOffEveryMirror, LatticeSymmetry.Turn(HexOffEveryMirror, HexStep.RotationOrder));
    }

    [Fact]
    public void Turn_MinusOne_TurnsAnticlockwise() => Assert.Equal(GridStep.Left, LatticeSymmetry.Turn(GridStep.Up, -1));

    [Fact]
    public void Turn_PastAWholeTurn_WrapsAround() => Assert.Equal(GridStep.Right, LatticeSymmetry.Turn(GridStep.Up, 5));

    [Fact]
    public void Turn_HalfAHexTurn_IsTheOppositeStep() => Assert.Equal(-HexOffEveryMirror, LatticeSymmetry.Turn(HexOffEveryMirror, 3));

    [Fact]
    public void Orbit_OfRight_IsTheOrthogonalSteps() =>
        Assert.Equal(Sorted(GridDirections.Orthogonal), Sorted(LatticeSymmetry.Orbit(GridStep.Right)));

    [Fact]
    public void Orbit_OfDownRight_IsTheDiagonalSteps() =>
        Assert.Equal(Sorted(GridDirections.Diagonal), Sorted(LatticeSymmetry.Orbit(GridStep.DownRight)));

    [Fact]
    public void Orbit_OfOneLShapedJump_IsEveryLShapedJump() =>
        Assert.Equal(Sorted(GridDirections.LShaped), Sorted(LatticeSymmetry.Orbit(GridStep.DownDownRight)));

    [Fact]
    public void Orbit_OfStayingPut_IsItselfAlone() =>
        Assert.Equal([GridStep.AdditiveIdentity], LatticeSymmetry.Orbit(GridStep.AdditiveIdentity).ToArray());

    [Fact]
    public void Orbit_ListsTheStepThenItsTurnsThenItsMirrorsTurns() =>
        Assert.Equal(
            [GridStep.DownDownRight, GridStep.DownLeftLeft, GridStep.UpUpLeft, GridStep.UpRightRight, GridStep.DownDownLeft, GridStep.UpLeftLeft, GridStep.UpUpRight, GridStep.DownRightRight],
            LatticeSymmetry.Orbit(GridStep.DownDownRight).ToArray());

    [Fact]
    public void Orbit_OfAHexNeighbour_IsSixStepsOneCellAway()
    {
        var orbit = LatticeSymmetry.Orbit(HexNeighbour);

        Assert.Equal(6, orbit.Length);
        Assert.All(orbit, step => Assert.Equal(1, step.Distance));
    }

    [Fact]
    public void Orbit_OfAHexDiagonal_IsSixStepsTwoCellsAway()
    {
        var orbit = LatticeSymmetry.Orbit(HexDiagonal);

        Assert.Equal(6, orbit.Length);
        Assert.All(orbit, step => Assert.Equal(2, step.Distance));
    }

    [Fact]
    public void Orbit_OfAHexStepOnNoMirrorLine_IsTwelveSteps() => Assert.Equal(12, LatticeSymmetry.Orbit(HexOffEveryMirror).Length);

    [Fact]
    public void Orbit_OfAHexStep_ListsNoStepTwice()
    {
        var orbit = LatticeSymmetry.Orbit(HexOffEveryMirror);

        Assert.Equal(orbit.Length, orbit.Distinct().Count());
    }

    // The hexagonal lattice's laws: a mirror reverses which way a turn goes, and steps add as they turn.
    [Fact]
    public void Orbit_HexLattice_ObeysTheDihedralLaws()
    {
        var step = HexOffEveryMirror;

        Assert.Equal(step, HexStep.Reflect(HexStep.Reflect(step)));
        Assert.Equal(LatticeSymmetry.Turn(step, -1), HexStep.Reflect(HexStep.Rotate(HexStep.Reflect(step))));
        Assert.Equal(HexStep.Rotate(step) + HexStep.Rotate(HexNeighbour), HexStep.Rotate(step + HexNeighbour));
    }

    private static (int DeltaRow, int DeltaCol)[] Sorted(IEnumerable<GridStep> steps) =>
        [.. steps.Select(step => (step.DeltaRow, step.DeltaCol)).Order()];

    // A step on the hexagonal lattice in axial coordinates (q, r), the third cube coordinate being
    // -q - r. A hexagonal grid's cells sit on the triangular lattice A2, whose smallest rotation is a
    // sixth of a turn: in cube coordinates (x, y, z) goes to (-z, -x, -y), and swapping two cube
    // coordinates mirrors it.
    private readonly record struct HexStep(int DeltaQ, int DeltaR) : IPlanarLatticeStep<HexStep>
    {
        private const int SixthTurnsPerTurn = 6;

        public static int RotationOrder => SixthTurnsPerTurn;

        public static HexStep AdditiveIdentity { get; } = new(0, 0);

        // How many cells away the step lands: half the cube coordinates' absolute sum.
        public int Distance => (Math.Abs(DeltaQ) + Math.Abs(DeltaR) + Math.Abs(DeltaQ + DeltaR)) / 2;

        public static HexStep operator +(HexStep left, HexStep right) => new(left.DeltaQ + right.DeltaQ, left.DeltaR + right.DeltaR);

        public static HexStep operator -(HexStep step) => new(-step.DeltaQ, -step.DeltaR);

        public static HexStep Rotate(HexStep step) => new(-step.DeltaR, step.DeltaQ + step.DeltaR);

        public static HexStep Reflect(HexStep step) => new(step.DeltaR, step.DeltaQ);
    }
}
