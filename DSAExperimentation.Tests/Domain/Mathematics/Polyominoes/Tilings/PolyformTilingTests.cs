using DSAExperimentation.Algorithms.ExactCover;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;
using DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Tilings;

// The reduction's numbers, then the reduction proved by counting tilings that can be counted by
// hand, by parity, or from the literature:
// - dominoes tile a 2 by n rectangle in Fibonacci many ways, since a tiling ends in one upright
//   domino or two lying ones; 3 by 4 in 11 ways and 4 by 4 in 36;
// - no domino tiling covers an odd number of squares, or a board missing two opposite corners,
//   which are one colour of the chessboard while every domino covers one square of each;
// - the five free tetrominoes tile no 4 by 5 rectangle: the T covers three squares of one colour
//   and the others two of each, while the rectangle has ten of each;
// - the twelve pentominoes tile a 3 by 20 rectangle in 2 ways up to its symmetries (Golomb,
//   Polyominoes), and no such tiling is its own image under the rectangle's group of order 4, so
//   there are 2 x 4 = 8 covers.
// Each pentomino cover is also read back as placements, which must be orientations of their
// pieces, each piece once, covering every square once.
public sealed partial class PolyformTilingTests
{
    private static readonly Polyform<GridStep, SquareCells> Domino = Drawing.Polyomino("##");

    // The 2 by n rectangles dominoes tile, with the Fibonacci number of tilings.
    public static TheoryData<int, long> DominoStrips => new()
    {
        { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 5 }, { 5, 8 }, { 6, 13 }, { 7, 21 }, { 8, 34 },
    };

    // A board missing two opposite corners, of each even side the tests try.
    public static TheoryData<int> CutCornerSides => new() { 4, 6, 8 };

    [Fact]
    public void PrimaryColumnCount_EachPieceOnce_IsTheSquaresThenThePieces() =>
        Assert.Equal(60 + 12, PentominoesOnThreeByTwenty().PrimaryColumnCount);

    [Fact]
    public void PrimaryColumnCount_AtMostOnce_IsTheSquaresAlone() =>
        Assert.Equal(
            20,
            Tiling<Free, AtMostOnce>(SquareCells.Rectangle(4, 5), Tetrominoes.Free)
                .PrimaryColumnCount);

    [Fact]
    public void SecondaryColumnCount_AtMostOnce_IsThePieces() =>
        Assert.Equal(
            5,
            Tiling<Free, AtMostOnce>(SquareCells.Rectangle(4, 5), Tetrominoes.Free)
                .SecondaryColumnCount);

    [Fact]
    public void SecondaryColumnCount_EachPieceOnce_IsZero() =>
        Assert.Equal(0, PentominoesOnThreeByTwenty().SecondaryColumnCount);

    // A domino lies in one of two places in each row of a 2 by 3 rectangle, and stands in one of
    // three columns.
    [Fact]
    public void RowCount_DominoOnTwoByThree_IsEveryPlacementThatFits() =>
        Assert.Equal(7, Tiling<Free, AnyNumber>(SquareCells.Rectangle(2, 3), [Domino]).RowCount);

    // Every orientation of every pentomino at every place it fits in a 3 by 20 rectangle, a count
    // a separate program, a plain Algorithm X, also found.
    [Fact]
    public void RowCount_PentominoesOnThreeByTwenty_IsEveryPlacementThatFits() =>
        Assert.Equal(1236, PentominoesOnThreeByTwenty().RowCount);

    [Fact]
    public void ColumnCountOf_PieceUsedAnyNumberOfTimes_IsItsSquares() =>
        Assert.Equal(
            2, Tiling<Free, AnyNumber>(SquareCells.Rectangle(2, 3), [Domino]).ColumnCountOf(0));

    [Fact]
    public void ColumnCountOf_PieceUsedOnce_IsItsSquaresAndItself() =>
        Assert.Equal(6, PentominoesOnThreeByTwenty().ColumnCountOf(0));

    // The domino as drawn, lying down, with its first square on the rectangle's first, then the
    // domino's own column after the six squares'.
    [Fact]
    public void WriteColumns_FirstPlacement_MeetsItsSquaresThenItsPiece()
    {
        var columns = new int[3];

        Tiling<Free, ExactlyOnce>(SquareCells.Rectangle(2, 3), [Domino]).WriteColumns(0, columns);

        Assert.Equal([0, 1, 6], columns);
    }

    [Fact]
    public void Region_IsTheCellsInTheOrderGiven() =>
        Assert.Equal(
            [new(1, 1), new(0, 0), new(0, 1)],
            Tiling<Free, AnyNumber>([new(1, 1), new(0, 0), new(0, 1)], [Domino]).Region.ToArray());

    [Fact]
    public void Placements_FirstRow_IsThePieceAsGivenOnTheFirstCell() =>
        Assert.Equal(
            new PolyformPlacement<GridStep, SquareCells>(0, Domino, new(0, 0)),
            Tiling<Free, AnyNumber>(SquareCells.Rectangle(2, 3), [Domino]).Placements[0]);

    [Fact]
    public void Of_RepeatedRegionCell_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            Tiling<Free, AnyNumber>([new(0, 0), new(0, 0)], [Domino]));

    [Fact]
    public void Of_PieceWithNoCells_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Tiling<Free, AnyNumber>(SquareCells.Rectangle(2, 2), [default]));

    [Theory]
    [MemberData(nameof(DominoStrips))]
    public void CountCovers_DominoesOnTwoByN_IsFibonacci(int width, long tilings) =>
        Assert.Equal(tilings, Covers<Free, AnyNumber>(SquareCells.Rectangle(2, width), [Domino]));

    [Fact]
    public void CountCovers_DominoesOnSquares_IsTheKnownCounts() =>
        Assert.Equal(
            (11L, 36L),
            (Covers<Free, AnyNumber>(SquareCells.Rectangle(3, 4), [Domino]),
                Covers<Free, AnyNumber>(SquareCells.Rectangle(4, 4), [Domino])));

    [Fact]
    public void CountCovers_DominoesOnAnOddCount_IsZero() =>
        Assert.Equal(0, Covers<Free, AnyNumber>(SquareCells.Rectangle(3, 3), [Domino]));

    [Theory]
    [MemberData(nameof(CutCornerSides))]
    public void CountCovers_DominoesOnABoardMissingOppositeCorners_IsZero(int side)
    {
        var corners = new[] { new GridStep(0, 0), new GridStep(side - 1, side - 1) };
        var region = SquareCells.Rectangle(side, side).Except(corners).ToArray();

        Assert.Equal(0, Covers<Free, AnyNumber>(region, [Domino]));
    }

    // An L lying in a 2 by 4 rectangle leaves an L or a J: "###./#..." leaves an L, "###./..#."
    // leaves no piece, and their mirror images leave J's. So free L's tile it twice, one-sided L's
    // once, and two L's used once each tile it in two ways, swapping them.
    [Fact]
    public void CountCovers_LTetrominoesOnTwoByFour_CountMirrorImagesOnlyWhenFree() =>
        Assert.Equal(
            (2L, 1L, 1L),
            (Covers<Free, AnyNumber>(SquareCells.Rectangle(2, 4), [Tetrominoes.L]),
                Covers<OneSided, AnyNumber>(SquareCells.Rectangle(2, 4), [Tetrominoes.L]),
                Covers<OneSided, AnyNumber>(SquareCells.Rectangle(2, 4), [Tetrominoes.J])));

    [Fact]
    public void CountCovers_PiecesUsedOnce_AreLabelled() =>
        Assert.Equal(
            (2L, 0L),
            (Covers<OneSided, ExactlyOnce>(
                    SquareCells.Rectangle(2, 4), [Tetrominoes.L, Tetrominoes.L]),
                Covers<OneSided, ExactlyOnce>(
                    SquareCells.Rectangle(2, 4), [Tetrominoes.L, Tetrominoes.J])));

    // A 2 by 2 square holds an O, and no I: used exactly once both must go in, at most once the O
    // alone does, and any number of times the O alone does. A 2 by 4 rectangle takes two O's, which
    // only any number of times allows.
    [Fact]
    public void CountCovers_EachSupply_UsesPiecesAsOftenAsItAllows()
    {
        var square = SquareCells.Rectangle(2, 2);
        var strip = SquareCells.Rectangle(2, 4);
        Polyform<GridStep, SquareCells>[] oAndI = [Tetrominoes.O, Tetrominoes.I];

        Assert.Equal(
            (0L, 1L, 1L, 0L, 0L, 1L),
            (Covers<Free, ExactlyOnce>(square, oAndI), Covers<Free, AtMostOnce>(square, oAndI),
                Covers<Free, AnyNumber>(square, oAndI),
                Covers<Free, ExactlyOnce>(strip, [Tetrominoes.O]),
                Covers<Free, AtMostOnce>(strip, [Tetrominoes.O]),
                Covers<Free, AnyNumber>(strip, [Tetrominoes.O])));
    }

    [Fact]
    public void CountCovers_FiveFreeTetrominoesOnFourByFive_IsZero() =>
        Assert.Equal(0, Covers<Free, ExactlyOnce>(SquareCells.Rectangle(4, 5), Tetrominoes.Free));

    [Fact]
    public void CountCovers_PentominoesOnThreeByTwenty_AreTwoTilingsTimesFourSymmetries() =>
        Assert.Equal(2 * 4, AlgorithmX.CountCovers(PentominoesOnThreeByTwenty(), []));

    [Fact]
    public void TrySearch_PentominoesOnThreeByTwenty_EveryCoverIsATiling()
    {
        var tiling = PentominoesOnThreeByTwenty();
        var visitor = new CoverList([]);

        AlgorithmX.TrySearch(tiling, [], ref visitor);

        Assert.Equal(8, visitor.Covers.Count);
        Assert.All(
            visitor.Covers,
            cover => Assert.True(
                ExactCover<PolyformTiling<GridStep, SquareCells>>.IsCover(tiling, cover)));
        Assert.All(visitor.Covers, cover => AssertTiles(tiling, cover, Pentominoes.Free));
    }

    // Read back as placements: each piece once, in one of its orientations, and the squares they
    // cover are the region's, each once.
    private static void AssertTiles(
        PolyformTiling<GridStep, SquareCells> tiling,
        int[] cover,
        IReadOnlyList<Polyform<GridStep, SquareCells>> pieces)
    {
        var inReadingOrder = Comparer<GridStep>.Create(SquareCells.Compare);
        var placements = cover.Select(row => tiling.Placements[row]).ToArray();
        var covered = placements.SelectMany(
            placement => placement.Orientation.Cells.Select(cell => cell + placement.Anchor));

        Assert.Equal(
            Enumerable.Range(0, pieces.Count),
            placements.Select(placement => placement.Piece).Order());
        Assert.All(
            placements,
            placement => Assert.True(
                placement.Orientation.IsEquivalentTo<Free>(pieces[placement.Piece])));
        Assert.Equal(tiling.Region.Order(inReadingOrder), covered.Order(inReadingOrder));
    }

    private static PolyformTiling<GridStep, SquareCells> PentominoesOnThreeByTwenty() =>
        Tiling<Free, ExactlyOnce>(SquareCells.Rectangle(3, 20), Pentominoes.Free);

    private static long Covers<TEquivalence, TSupply>(
        IReadOnlyList<GridStep> region, IReadOnlyList<Polyform<GridStep, SquareCells>> pieces)
        where TEquivalence : struct, IPolyformEquivalence
        where TSupply : struct, IPieceSupply =>
        AlgorithmX.CountCovers(Tiling<TEquivalence, TSupply>(region, pieces), []);

    private static PolyformTiling<GridStep, SquareCells> Tiling<TEquivalence, TSupply>(
        IReadOnlyList<GridStep> region, IReadOnlyList<Polyform<GridStep, SquareCells>> pieces)
        where TEquivalence : struct, IPolyformEquivalence
        where TSupply : struct, IPieceSupply =>
        PolyformTiling<GridStep, SquareCells>.Of<TEquivalence, TSupply>([.. region], [.. pieces]);

    // Keeps a copy of every cover.
    private readonly record struct CoverList(List<int[]> Covers) : ICoverVisitor
    {
        public bool ShouldStopAfter(ReadOnlySpan<int> cover)
        {
            Covers.Add(cover.ToArray());

            return false;
        }
    }
}
