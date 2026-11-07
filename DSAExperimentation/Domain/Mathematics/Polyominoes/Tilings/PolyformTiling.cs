using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

// Tiling a region with polyforms as an exact cover problem, the reduction Knuth's dancing-links
// paper makes of Golomb's pentomino puzzles. The region is a set of cells, which may have holes or
// come in several parts, such as a rectangle or a board with squares cut out. A row of the matrix
// puts one piece down in one of its orientations, with the orientation's first cell on a region
// cell, and is a row only when every cell it covers is in the region. Each region cell is a primary
// column, so a cover covers every cell exactly once.
// - Which orientations a piece may take is an equivalence's: a free piece may be turned over, a
//   one-sided one only turned.
// - How often each piece may be used is a supply's: a column per piece, primary for exactly once
//   and secondary for at most once, or none for any number of times.
// A cover lists labelled pieces: two pieces of one shape, each used once, are two pieces, and
// swapping them is another cover. Covers count tilings as placed, not up to the region's
// symmetries: a tiling's images under the region's symmetry group are covers too, so when no tiling
// is its own image the covers are that group's order times the tilings up to symmetry.
//
// The region's cells are numbered in the order given, from column 0, and the pieces' columns come
// after them in the pieces' order. The rows are found once, when Of makes the problem, so a row's
// columns are a slice of one array, and the default value holds no problem to read. The search
// that finds covers is Algorithms/ExactCover's.
/// <summary>
/// Tiling a region with polyforms, as an exact cover: placements as rows, cells and pieces as
/// columns.
/// </summary>
/// <typeparam name="TStep">The lattice's step type.</typeparam>
/// <typeparam name="TCells">
/// How cells are joined and ordered, such as <see cref="SquareCells"/>.
/// </typeparam>
public readonly struct PolyformTiling<TStep, TCells> : IExactCoverProblem
    where TStep : struct, IPlanarLatticeStep<TStep>
    where TCells : struct, IPolyformCells<TStep>
{
    private const string RepeatedCell = "A region cell is listed twice.";

    private readonly ImmutableArray<TStep> _region;
    private readonly ImmutableArray<PolyformPlacement<TStep, TCells>> _placements;

    // Row r meets the columns from Columns[Starts[r]] up to Columns[Starts[r + 1]].
    private readonly (ImmutableArray<int> Starts, ImmutableArray<int> Columns) _rows;
    private readonly (int Primary, int Secondary) _pieceColumns;

    /// <summary>Gets the region's cells, in the order their columns are numbered.</summary>
    public ImmutableArray<TStep> Region => _region;

    /// <summary>Gets each row's placement, by row.</summary>
    public ImmutableArray<PolyformPlacement<TStep, TCells>> Placements => _placements;

    /// <summary>Gets the region's cells, then the pieces when each is used exactly once.</summary>
    public int PrimaryColumnCount => _region.Length + _pieceColumns.Primary;

    /// <summary>Gets the pieces when each is used at most once, or zero.</summary>
    public int SecondaryColumnCount => _pieceColumns.Secondary;

    /// <summary>Gets how many placements fit in the region.</summary>
    public int RowCount => _placements.Length;

    // The rows are written out from the placements once, here, so a row's columns are read by
    // copying a slice.
    private PolyformTiling(
        ReadOnlySpan<TStep> region,
        ImmutableArray<PolyformPlacement<TStep, TCells>> placements,
        Dictionary<TStep, int> columnOf,
        (int Primary, int Secondary) pieceColumns)
    {
        _region = [.. region];
        _placements = placements;
        _rows = RowsOf(placements, columnOf, pieceColumns);
        _pieceColumns = pieceColumns;
    }

    /// <summary>Makes the tiling problem of a region and a set of pieces.</summary>
    /// <typeparam name="TEquivalence">Which orientations a piece may be put down in.</typeparam>
    /// <typeparam name="TSupply">How often each piece may be used.</typeparam>
    /// <param name="region">The cells to cover, each once, in the order their columns take.</param>
    /// <param name="pieces">The pieces, in the order to number their columns.</param>
    /// <returns>The problem, with every placement that fits.</returns>
    /// <exception cref="ArgumentException">
    /// A region cell is listed twice, or a piece has no cells.
    /// </exception>
    public static PolyformTiling<TStep, TCells> Of<TEquivalence, TSupply>(
        ReadOnlySpan<TStep> region, ReadOnlySpan<Polyform<TStep, TCells>> pieces)
        where TEquivalence : struct, IPolyformEquivalence
        where TSupply : struct, IPieceSupply
    {
        var columnOf = ColumnsOf(region);
        var placements = PlacementsOf<TEquivalence>(region, pieces, columnOf);
        (int Primary, int Secondary) pieceColumns =
            (TSupply.PrimaryColumnsFor(pieces.Length), TSupply.SecondaryColumnsFor(pieces.Length));

        return new(region, placements, columnOf, pieceColumns);
    }

    // Each region cell's column, its place in the list.
    private static Dictionary<TStep, int> ColumnsOf(ReadOnlySpan<TStep> region)
    {
        var columnOf = new Dictionary<TStep, int>(region.Length);

        for (var cell = 0; cell < region.Length; cell++)
        {
            if (!columnOf.TryAdd(region[cell], cell))
            {
                throw new ArgumentException(RepeatedCell, nameof(region));
            }
        }

        return columnOf;
    }

    /// <summary>Counts the columns a placement meets: its cells, and its piece's if any.</summary>
    /// <param name="row">The placement's row.</param>
    /// <returns>How many columns it meets.</returns>
    public int ColumnCountOf(int row) => _rows.Starts[row + 1] - _rows.Starts[row];

    /// <summary>Writes the columns a placement meets: its cells', then its piece's.</summary>
    /// <param name="row">The placement's row.</param>
    /// <param name="columns">A span of exactly <see cref="ColumnCountOf"/> entries.</param>
    public void WriteColumns(int row, Span<int> columns)
    {
        var start = _rows.Starts[row];
        _rows.Columns.AsSpan(start, _rows.Starts[row + 1] - start).CopyTo(columns);
    }

    // Every placement that fits: each piece, in each of its orientations, with its first cell on
    // each region cell in turn.
    private static ImmutableArray<PolyformPlacement<TStep, TCells>> PlacementsOf<TEquivalence>(
        ReadOnlySpan<TStep> region,
        ReadOnlySpan<Polyform<TStep, TCells>> pieces,
        Dictionary<TStep, int> columnOf)
        where TEquivalence : struct, IPolyformEquivalence
    {
        var placements = ImmutableArray.CreateBuilder<PolyformPlacement<TStep, TCells>>();

        for (var piece = 0; piece < pieces.Length; piece++)
        {
            ArgumentOutOfRangeException.ThrowIfZero(pieces[piece].Size, nameof(pieces));

            foreach (var orientation in pieces[piece].Orientations<TEquivalence>())
            {
                AddFitting(placements, new(piece, orientation, default), region, columnOf);
            }
        }

        return placements.ToImmutable();
    }

    // The placements of one orientation of a piece whose every cell is in the region.
    private static void AddFitting(
        ImmutableArray<PolyformPlacement<TStep, TCells>>.Builder placements,
        PolyformPlacement<TStep, TCells> unplaced,
        ReadOnlySpan<TStep> region,
        Dictionary<TStep, int> columnOf)
    {
        foreach (var anchor in region)
        {
            var placement = unplaced with { Anchor = anchor };

            if (IsInside(placement, columnOf))
            {
                placements.Add(placement);
            }
        }
    }

    // Whether every cell a placement covers is a region cell.
    private static bool IsInside(
        PolyformPlacement<TStep, TCells> placement, Dictionary<TStep, int> columnOf)
    {
        foreach (var cell in placement.Orientation.Cells)
        {
            if (!columnOf.ContainsKey(cell + placement.Anchor))
            {
                return false;
            }
        }

        return true;
    }

    // Each placement's columns, one after another: its cells' columns, then its piece's when the
    // pieces have columns, numbered after the region's cells.
    private static (ImmutableArray<int> Starts, ImmutableArray<int> Columns) RowsOf(
        ImmutableArray<PolyformPlacement<TStep, TCells>> placements,
        Dictionary<TStep, int> columnOf,
        (int Primary, int Secondary) pieceColumns)
    {
        var hasPieceColumns = pieceColumns.Primary + pieceColumns.Secondary > 0;
        var starts = ImmutableArray.CreateBuilder<int>(placements.Length + 1);
        var columns = ImmutableArray.CreateBuilder<int>();
        starts.Add(0);

        foreach (var placement in placements)
        {
            foreach (var cell in placement.Orientation.Cells)
            {
                columns.Add(columnOf[cell + placement.Anchor]);
            }

            if (hasPieceColumns)
            {
                columns.Add(columnOf.Count + placement.Piece);
            }

            starts.Add(columns.Count);
        }

        return (starts.MoveToImmutable(), columns.ToImmutable());
    }
}
