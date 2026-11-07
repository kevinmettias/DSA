using DSAExperimentation.Algorithms.ExactCover;
using DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.ExactCovers.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ExactCover;

// The links over Knuth's example, read through each column's rows top to bottom. Its columns A to
// G are headers 1 to 7, and its rows are {C, E, F}, {A, D, G}, {B, C, F}, {A, D}, {B, G} and
// {D, E, G}.
public sealed partial class DancingLinksTests
{
    private const int ColumnA = 1;
    private const int ColumnB = 2;
    private const int ColumnC = 3;
    private const int ColumnD = 4;
    private const int ColumnE = 5;
    private const int ColumnG = 7;
    private const int Columns = 7;

    // Each column of Knuth's example and the rows meeting it, top to bottom.
    public static TheoryData<int, int[]> RowsByColumn => new()
    {
        { ColumnA, [1, 3] },
        { ColumnB, [2, 4] },
        { ColumnC, [0, 2] },
        { ColumnD, [1, 3, 5] },
        { ColumnE, [0, 5] },
        { 6, [0, 2] },
        { ColumnG, [1, 4, 5] },
    };

    [Theory]
    [MemberData(nameof(RowsByColumn))]
    public void Of_KnuthsExample_LinksEachColumnsRowsInOrder(int column, int[] expected) =>
        Assert.Equal(expected, RowsOf(Knuth(), column));

    [Fact]
    public void Below_LastRowOfAColumn_IsTheHeaderAgain()
    {
        var links = Knuth();
        var last = links.Below(links.Below(ColumnA));

        Assert.Equal(ColumnA, links.Below(last));
    }

    [Fact]
    public void RowOf_EveryNodeOfAColumn_IsItsRow()
    {
        var links = Knuth();
        var first = links.Below(ColumnG);
        var second = links.Below(first);
        var third = links.Below(second);

        Assert.Equal([1, 4, 5], [links.RowOf(first), links.RowOf(second), links.RowOf(third)]);
    }

    [Fact]
    public void IsComplete_NothingCovered_IsFalse() => Assert.False(Knuth().IsComplete);

    [Fact]
    public void IsComplete_EveryPrimaryColumnCovered_IsTrue()
    {
        var links = Knuth();

        for (var column = 1; column <= Columns; column++)
        {
            links.Cover(column);
        }

        Assert.True(links.IsComplete);
    }

    [Fact]
    public void IsComplete_OnlySecondaryColumnsLeft_IsTrue()
    {
        var links = DancingLinks.Of(CoverMatrices.WithSecondary);
        links.Cover(1);
        links.Cover(2);

        Assert.True(links.IsComplete);
    }

    [Fact]
    public void ColumnWithFewestRows_Tie_IsTheFirst() =>
        Assert.Equal(ColumnA, Knuth().ColumnWithFewestRows());

    [Fact]
    public void ColumnWithFewestRows_AfterACover_CountsOnlyRowsLeft()
    {
        var links = Knuth();
        links.Cover(ColumnA);

        Assert.Equal(ColumnD, links.ColumnWithFewestRows());
    }

    // Three rows meet the primary column and one the secondary, which is still not chosen.
    [Fact]
    public void ColumnWithFewestRows_SecondaryColumnMetLeast_IsNeverChosen()
    {
        var links = DancingLinks.Of(new MatrixProblem(1, 1, [[0], [0, 1], [0]]));

        Assert.Equal(1, links.ColumnWithFewestRows());
    }

    [Fact]
    public void Cover_Column_TakesEveryRowMeetingItOutOfTheOtherColumns()
    {
        var links = Knuth();
        links.Cover(ColumnA);

        Assert.Equal([5], RowsOf(links, ColumnD));
        Assert.Equal([4, 5], RowsOf(links, ColumnG));
        Assert.Equal([1, 3], RowsOf(links, ColumnA));
    }

    [Fact]
    public void Uncover_CoversUndoneInReverse_RestoresEveryColumn()
    {
        var links = Knuth();
        var before = AllRows(links);

        var after = AllRowsAfterCoveringAndUncovering(links, [ColumnA, ColumnD, ColumnE]);

        Assert.Equal(before, after);
    }

    [Fact]
    public void CoverRestOfRow_ChosenRow_CoversItsOtherColumns()
    {
        var links = Knuth();
        links.Cover(ColumnA);
        links.CoverRestOfRow(links.Below(ColumnA));

        Assert.Equal([2], RowsOf(links, ColumnB));
        Assert.Equal([0], RowsOf(links, ColumnE));
    }

    [Fact]
    public void UncoverRestOfRow_AfterCoverRestOfRow_RestoresEveryColumn()
    {
        var links = Knuth();
        var before = AllRows(links);

        var after = AllRowsAfterChoosingAndUnchoosingTheFirstRowOf(links, ColumnA);

        Assert.Equal(before, after);
    }

    [Fact]
    public void TryChooseAll_DisjointRows_CoversTheirColumns()
    {
        var links = Knuth();

        Assert.True(links.TryChooseAll([3, 0]));
        Assert.Equal([4], RowsOf(links, ColumnB));
        Assert.False(links.IsComplete);
    }

    [Fact]
    public void TryChooseAll_ACover_LeavesNothingToCover()
    {
        var links = Knuth();

        Assert.True(links.TryChooseAll(CoverMatrices.KnuthCover));
        Assert.True(links.IsComplete);
    }

    [Fact]
    public void TryChooseAll_RowsSharingAColumn_ReturnsFalse() =>
        Assert.False(Knuth().TryChooseAll([0, 2]));

    [Fact]
    public void TryChooseAll_RowsSharingASecondaryColumn_ReturnsFalse() =>
        Assert.False(DancingLinks.Of(CoverMatrices.WithSecondary).TryChooseAll([0, 1]));

    [Fact]
    public void TryChooseAll_NegativeRow_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Knuth().TryChooseAll([-1]));

    private static DancingLinks Knuth() => DancingLinks.Of(CoverMatrices.Knuth);

    // Covers the columns in order, uncovers them in the reverse order, and reads every column.
    private static List<List<int>> AllRowsAfterCoveringAndUncovering(
        DancingLinks links, int[] columns)
    {
        foreach (var column in columns)
        {
            links.Cover(column);
        }

        for (var index = columns.Length - 1; index >= 0; index--)
        {
            links.Uncover(columns[index]);
        }

        return AllRows(links);
    }

    // Chooses a column's first row as Algorithm X does and unchooses it, and reads every column.
    private static List<List<int>> AllRowsAfterChoosingAndUnchoosingTheFirstRowOf(
        DancingLinks links, int column)
    {
        var row = links.Below(column);

        links.Cover(column);
        links.CoverRestOfRow(row);
        links.UncoverRestOfRow(row);
        links.Uncover(column);

        return AllRows(links);
    }

    // The rows meeting a column that are still in the matrix, top to bottom.
    private static List<int> RowsOf(DancingLinks links, int column)
    {
        var rows = new List<int>();

        for (var node = links.Below(column); node != column; node = links.Below(node))
        {
            rows.Add(links.RowOf(node));
        }

        return rows;
    }

    private static List<List<int>> AllRows(DancingLinks links) =>
        [.. Enumerable.Range(1, Columns).Select(column => RowsOf(links, column))];
}
