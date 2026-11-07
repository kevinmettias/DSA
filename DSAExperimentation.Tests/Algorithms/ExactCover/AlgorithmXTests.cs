using DSAExperimentation.Algorithms.ExactCover;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;
using DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.ExactCovers.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ExactCover;

// Algorithm X against matrices whose covers are known by hand: Knuth's example, which has one;
// one with a secondary column, which may be met once or not at all; and two with none. Rows chosen
// in advance start the cover, and two that clash leave none. The n-queens and Sudoku reductions'
// own tests solve the puzzles the algorithm is for.
public sealed partial class AlgorithmXTests
{
    // Rows chosen in advance of WithSecondary, and the covers holding them.
    public static TheoryData<int[], long> ChosenRowsAndCoverCounts => new()
    {
        { [], 3 },
        { [2], 2 },
        { [0], 1 },
        { [0, 3], 1 },
        { [0, 1], 0 },
        { [2, 2], 0 },
    };

    [Fact]
    public void TryFindCover_KnuthsExample_FindsRowsOneFourAndFiveInTheOrderChosen()
    {
        var isFound = AlgorithmX.TryFindCover(CoverMatrices.Knuth, [], out var cover);

        Assert.True(isFound);
        Assert.Equal(CoverMatrices.KnuthCover, cover);
    }

    [Fact]
    public void TryFindCover_KnuthsExample_KeepsTheExactCoverRule()
    {
        AlgorithmX.TryFindCover(CoverMatrices.Knuth, [], out var cover);

        Assert.True(ExactCover<MatrixProblem>.IsCover(CoverMatrices.Knuth, cover));
    }

    [Fact]
    public void TryFindCover_NoCover_ReturnsFalseAndAnEmptyCover()
    {
        var isFound = AlgorithmX.TryFindCover(CoverMatrices.Clashing, [], out var cover);

        Assert.False(isFound);
        Assert.Empty(cover);
    }

    [Fact]
    public void TryFindCover_ChosenRows_StartTheCover()
    {
        AlgorithmX.TryFindCover(CoverMatrices.WithSecondary, [3], out var cover);

        Assert.Equal([3, 0], cover);
    }

    [Fact]
    public void TryFindCover_ChosenRowOutOfRange_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AlgorithmX.TryFindCover(CoverMatrices.Knuth, [6], out _));

    [Fact]
    public void CountCovers_KnuthsExample_IsOne() =>
        Assert.Equal(1, AlgorithmX.CountCovers(CoverMatrices.Knuth, []));

    [Theory]
    [MemberData(nameof(ChosenRowsAndCoverCounts))]
    public void CountCovers_SecondaryColumn_MetOnceOrNotAtAll(int[] chosenRows, long expected) =>
        Assert.Equal(expected, AlgorithmX.CountCovers(CoverMatrices.WithSecondary, chosenRows));

    [Fact]
    public void CountCovers_ColumnNoRowMeets_IsZero() =>
        Assert.Equal(0, AlgorithmX.CountCovers(CoverMatrices.Uncoverable, []));

    [Fact]
    public void CountCovers_NoPrimaryColumns_IsOneEmptyCover() =>
        Assert.Equal(1, AlgorithmX.CountCovers(new MatrixProblem(0, 1, []), []));

    [Fact]
    public void CountCovers_RowMeetingAColumnTwice_Throws() =>
        Assert.Throws<ArgumentException>(
            () => AlgorithmX.CountCovers(new MatrixProblem(2, 0, [[0, 1, 0]]), []));

    [Fact]
    public void CountCovers_RowMeetingAColumnPastTheLast_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => AlgorithmX.CountCovers(new MatrixProblem(2, 0, [[0, 2]]), []));

    [Fact]
    public void TrySearch_VisitorThatGoesOn_SeesEveryCoverOnce()
    {
        var visitor = new CoverCollector([], StopAfter: int.MaxValue);

        var isStopped = AlgorithmX.TrySearch(CoverMatrices.WithSecondary, [], ref visitor);

        Assert.False(isStopped);
        Assert.Equal(["0,3", "1,2", "2,3"], visitor.Covers.Order());
    }

    [Fact]
    public void TrySearch_VisitorThatStops_EndsTheSearch()
    {
        var visitor = new CoverCollector([], StopAfter: 2);

        var isStopped = AlgorithmX.TrySearch(CoverMatrices.WithSecondary, [], ref visitor);

        Assert.True(isStopped);
        Assert.Equal(2, visitor.Covers.Count);
    }

    [Fact]
    public void TrySearch_ClashingChosenRows_VisitsNothing()
    {
        var visitor = new CoverCollector([], StopAfter: int.MaxValue);

        var isStopped = AlgorithmX.TrySearch(CoverMatrices.WithSecondary, [0, 1], ref visitor);

        Assert.False(isStopped);
        Assert.Empty(visitor.Covers);
    }

    // Records each cover as its sorted rows, and stops after a given number.
    private readonly record struct CoverCollector(List<string> Covers, int StopAfter)
        : ICoverVisitor
    {
        public bool ShouldStopAfter(ReadOnlySpan<int> cover)
        {
            var rows = cover.ToArray();
            Array.Sort(rows);
            Covers.Add(string.Join(',', rows));

            return Covers.Count >= StopAfter;
        }
    }
}
