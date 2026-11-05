using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Surfaces;
using DSAExperimentation.Domain.Mathematics.Surfaces.Gluings;

namespace DSAExperimentation.Tests.Domain.Mathematics.Surfaces;

// Each surface is checked as the surface its name claims, from where its steps land and nothing else.
// The cells, their edges and their corners make a cell complex once a step that lands on another cell
// identifies the edge and corners the two share, the corners paired the other way round when the step
// crossed a twist. Its Euler characteristic, V - E + F, and its boundary circles then tell the surfaces
// apart: the rectangle is a disk (1, one circle), the cylinder has characteristic 0 and two circles,
// the Mobius strip 0 and one, and the torus and the Klein bottle 0 and none. Orientation tells those
// last two apart: once round the Klein bottle a walker comes back mirrored.
public sealed partial class SurfaceTests
{
    private const int Rows = 3;
    private const int Cols = 4;

    [Fact]
    public void BoundedRectangle_TryLand_IsADiskWithOneBoundaryCircle() =>
        Assert.Equal((1, 1), CellComplex.Of<BoundedRectangle>(Rows, Cols).EulerAndBoundaries());

    [Fact]
    public void Cylinder_TryLand_HasCharacteristicZeroAndTwoBoundaryCircles() =>
        Assert.Equal((0, 2), CellComplex.Of<Cylinder>(Rows, Cols).EulerAndBoundaries());

    [Fact]
    public void Torus_TryLand_HasCharacteristicZeroAndNoBoundary() =>
        Assert.Equal((0, 0), CellComplex.Of<Torus>(Rows, Cols).EulerAndBoundaries());

    [Fact]
    public void MobiusStrip_TryLand_HasCharacteristicZeroAndOneBoundaryCircle() =>
        Assert.Equal((0, 1), CellComplex.Of<MobiusStrip>(Rows, Cols).EulerAndBoundaries());

    [Fact]
    public void KleinBottle_TryLand_HasCharacteristicZeroAndNoBoundary() =>
        Assert.Equal((0, 0), CellComplex.Of<KleinBottle>(Rows, Cols).EulerAndBoundaries());

    // Off the right edge of a Mobius strip the board comes back upside down: row 0 re-enters as the
    // last row, and the walker's rows run the other way.
    [Fact]
    public void MobiusStrip_TryLand_OffTheRightEdge_ComesInUpsideDown()
    {
        var row = 0;
        var col = Cols;

        Assert.True(MobiusStrip.TryLand(ref row, ref col, (Rows, Cols), out var mirrored));
        Assert.Equal((Rows - 1, 0, MirroredAxes.Rows), (row, col, mirrored));
    }

    // Twice round, the two mirrors cancel and the walker is back as it started.
    [Fact]
    public void MobiusStrip_TryLand_TwiceRound_ComesBackUnmirrored()
    {
        var row = 0;
        var col = 2 * Cols;

        Assert.True(MobiusStrip.TryLand(ref row, ref col, (Rows, Cols), out var mirrored));
        Assert.Equal((0, 0, MirroredAxes.None), (row, col, mirrored));
    }

    // The same step on the Klein bottle and the torus: the Klein bottle reverses the walker, the torus
    // keeps its orientation, which is all that tells two closed surfaces of characteristic 0 apart.
    [Fact]
    public void KleinBottle_TryLand_OnceRound_ComesBackMirrored()
    {
        var row = 0;
        var col = Cols;

        Assert.True(KleinBottle.TryLand(ref row, ref col, (Rows, Cols), out var mirrored));
        Assert.Equal((Rows - 1, 0, MirroredAxes.Rows), (row, col, mirrored));
    }

    [Fact]
    public void Torus_TryLand_OnceRound_ComesBackUnmirrored()
    {
        var row = 0;
        var col = Cols;

        Assert.True(Torus.TryLand(ref row, ref col, (Rows, Cols), out var mirrored));
        Assert.Equal((0, 0, MirroredAxes.None), (row, col, mirrored));
    }

    // Why no projective plane is offered: with both pairs twisted, a diagonal step out of a corner
    // comes straight back to its own cell, the cone point a flat grid cannot have.
    [Fact]
    public void GluedRectangle_TryLand_BothPairsTwisted_SendsACornersDiagonalStepBackToItself()
    {
        var row = GridStep.UpLeft.DeltaRow;
        var col = GridStep.UpLeft.DeltaCol;

        Assert.True(GluedRectangle<Twisted, Twisted>.TryLand(ref row, ref col, (Rows, Cols), out _));
        Assert.Equal((0, 0), (row, col));
    }

    [Fact]
    public void Torus_TryLand_TakesACornersNeighboursToTheOppositeEdges()
    {
        var landed = GridDirections.Orthogonal
            .Select(step => Land<Torus>(step.DeltaRow, step.DeltaCol))
            .ToArray();

        Assert.Equal([(Rows - 1, 0), (1, 0), (0, Cols - 1), (0, 1)], landed);
    }

    // A knight's jump from (1, 0) on a board two cells each way goes round a row and lands at (1, 1).
    [Fact]
    public void Torus_TryLand_AJumpLongerThanTheBoard_GoesRoundAndLands()
    {
        var row = 1 + GridStep.DownDownRight.DeltaRow;
        var col = GridStep.DownDownRight.DeltaCol;

        Assert.True(Torus.TryLand(ref row, ref col, (2, 2), out _));
        Assert.Equal((1, 1), (row, col));
    }

    [Fact]
    public void Cylinder_TryLand_RunsRoundTheRowsButNotOffTheTop()
    {
        var row = 0;
        var col = -1;
        var aboveRow = -1;
        var aboveCol = 0;

        Assert.True(Cylinder.TryLand(ref row, ref col, (Rows, Cols), out _));
        Assert.Equal((0, Cols - 1), (row, col));
        Assert.False(Cylinder.TryLand(ref aboveRow, ref aboveCol, (Rows, Cols), out _));
    }

    // GridSize's bounds rule is the bounded rectangle's, so the two answer alike everywhere.
    [Fact]
    public void BoundedRectangle_TryLand_AgreesWithGridSizeOnEveryNearbyCell()
    {
        var size = new GridSize(Rows, Cols);

        for (var row = -2; row <= Rows + 1; row++)
        {
            for (var col = -2; col <= Cols + 1; col++)
            {
                var (landedRow, landedCol) = (row, col);
                Assert.Equal(size.HasCell(row, col), BoundedRectangle.TryLand(ref landedRow, ref landedCol, (Rows, Cols), out _));
            }
        }
    }

    private static (int Row, int Col) Land<TSurface>(int row, int col)
        where TSurface : struct, ISurface
    {
        Assert.True(TSurface.TryLand(ref row, ref col, (Rows, Cols), out _));
        return (row, col);
    }

    // A rows x cols board's cells, with its edges and corners identified wherever a step from a cell to
    // its right or lower neighbour lands. Each side and corner of each cell starts as its own element,
    // and a landed step merges the two cells' shared side and the corners at its ends.
    private sealed class CellComplex
    {
        private const int Sides = 4;
        private const int Top = 0;
        private const int Bottom = 1;
        private const int Left = 2;
        private const int Right = 3;
        private const int TopLeft = 0;
        private const int TopRight = 1;
        private const int BottomLeft = 2;
        private const int BottomRight = 3;

        // What a step right shares with the cell it lands on, and what a step down shares. A step right
        // shares a side whose ends are a top and a bottom corner, which a twist mirroring the rows swaps;
        // a step down shares one whose ends a twist mirroring the columns swaps.
        private static readonly SharedBorder RightNeighbour = new((Right, Left), (TopRight, TopLeft), (BottomRight, BottomLeft), MirroredAxes.Rows);
        private static readonly SharedBorder LowerNeighbour = new((Bottom, Top), (BottomLeft, TopLeft), (BottomRight, TopRight), MirroredAxes.Columns);

        private readonly int[] _sideParent;
        private readonly int[] _cornerParent;
        private readonly int _rows;
        private readonly int _cols;

        private CellComplex(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _sideParent = [.. Enumerable.Range(0, rows * cols * Sides)];
            _cornerParent = [.. Enumerable.Range(0, rows * cols * Sides)];
        }

        public static CellComplex Of<TSurface>(int rows, int cols)
            where TSurface : struct, ISurface
        {
            var complex = new CellComplex(rows, cols);

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    complex.GlueIfLanded<TSurface>((row, col), (row, col + 1), RightNeighbour);
                    complex.GlueIfLanded<TSurface>((row, col), (row + 1, col), LowerNeighbour);
                }
            }

            return complex;
        }

        // The Euler characteristic V - E + F, and how many circles the boundary edges make.
        public (int Euler, int BoundaryCircles) EulerAndBoundaries()
        {
            var sides = Enumerable.Range(0, _sideParent.Length).GroupBy(side => Find(_sideParent, side)).ToList();
            var corners = Enumerable.Range(0, _cornerParent.Length).Select(corner => Find(_cornerParent, corner)).Distinct().Count();
            var euler = corners - sides.Count + (_rows * _cols);

            return (euler, BoundaryCircles(sides.Where(edge => edge.Count() == 1).Select(edge => edge.Key)));
        }

        // Boundary edges that share a corner lie on one circle.
        private int BoundaryCircles(IEnumerable<int> boundarySides)
        {
            var circles = new Dictionary<int, int>();

            foreach (var side in boundarySides)
            {
                circles[side] = side;
            }

            foreach (var side in circles.Keys.ToList())
            {
                foreach (var other in circles.Keys.ToList())
                {
                    var isTouching = EndCornersOf(side).Overlaps(EndCornersOf(other));

                    if (isTouching)
                    {
                        circles[Find(circles, side)] = Find(circles, other);
                    }
                }
            }

            return circles.Keys.Select(side => Find(circles, side)).Distinct().Count();
        }

        private HashSet<int> EndCornersOf(int side)
        {
            var cell = side / Sides * Sides;
            int[] ends = (side % Sides) switch
            {
                Top => [TopLeft, TopRight],
                Bottom => [BottomLeft, BottomRight],
                Left => [TopLeft, BottomLeft],
                _ => [TopRight, BottomRight],
            };

            return [.. ends.Select(corner => Find(_cornerParent, cell + corner))];
        }

        // A step from the cell toward the neighbour that lands merges the border they share.
        private void GlueIfLanded<TSurface>((int Row, int Col) cell, (int Row, int Col) stepped, SharedBorder border)
            where TSurface : struct, ISurface
        {
            var (row, col) = stepped;

            if (!TSurface.TryLand(ref row, ref col, (_rows, _cols), out var mirrored))
            {
                return;
            }

            var mine = ((cell.Row * _cols) + cell.Col) * Sides;
            var theirs = ((row * _cols) + col) * Sides;
            var isSwapped = mirrored.HasFlag(border.SwappedBy);
            var firstTheirs = isSwapped ? border.SecondCorner.Theirs : border.FirstCorner.Theirs;
            var secondTheirs = isSwapped ? border.FirstCorner.Theirs : border.SecondCorner.Theirs;

            Union(_sideParent, mine + border.Side.Mine, theirs + border.Side.Theirs);
            Union(_cornerParent, mine + border.FirstCorner.Mine, theirs + firstTheirs);
            Union(_cornerParent, mine + border.SecondCorner.Mine, theirs + secondTheirs);
        }

        private static void Union(int[] parent, int element, int other) => parent[Find(parent, element)] = Find(parent, other);

        private static int Find(int[] parent, int element)
        {
            while (parent[element] != element)
            {
                element = parent[element];
            }

            return element;
        }

        private static int Find(Dictionary<int, int> parent, int element)
        {
            while (parent[element] != element)
            {
                element = parent[element];
            }

            return element;
        }

        // The side two neighbouring cells share, the corners at its ends, each as this cell's and the
        // neighbour's own side or corner, and the mirror that pairs those corners the other way round.
        private readonly record struct SharedBorder(
            (int Mine, int Theirs) Side, (int Mine, int Theirs) FirstCorner, (int Mine, int Theirs) SecondCorner, MirroredAxes SwappedBy);
    }
}
