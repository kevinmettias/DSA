using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Domain.Games.Chess;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

public sealed partial class ChessRulesTests
{
    // Each line set by name, with the steps along it: one square, and in a direction no other line
    // of the set shares.
    public static TheoryData<string> LineSets => new() { nameof(ChessRules.RookLines), nameof(ChessRules.BishopLines), nameof(ChessRules.QueenLines) };

    [Fact]
    public void RookLines_EachStaysOnTheRooksRowOrColumn() =>
        Assert.All(ChessRules.RookLines, line => Assert.True((line.DeltaRow == 0) != (line.DeltaCol == 0)));

    [Fact]
    public void BishopLines_EachStaysOnADiagonal() =>
        Assert.All(ChessRules.BishopLines, line => Assert.True(Math.Abs(line.DeltaRow) == 1 && Math.Abs(line.DeltaCol) == 1));

    [Fact]
    public void QueenLines_AreTheRookAndBishopLinesTogether() =>
        Assert.Equal(ChessRules.RookLines.Concat(ChessRules.BishopLines).ToHashSet(), ChessRules.QueenLines.ToHashSet());

    // LC 1222's answer lists attackers in the order the queen's lines are walked, and its arms are
    // compared in that order.
    [Fact]
    public void QueenLines_RunInTheKingTablesOrder() => Assert.Equal(GridDirections.King, ChessRules.QueenLines);

    [Theory]
    [MemberData(nameof(LineSets))]
    public void Lines_EachIsOneSquareInADistinctDirection(string lineSet)
    {
        var lines = LinesNamed(lineSet);

        Assert.All(lines, line => Assert.Equal(1, Math.Max(Math.Abs(line.DeltaRow), Math.Abs(line.DeltaCol))));
        Assert.Equal(lines.Count, lines.Distinct().Count());
    }

    private static List<(int DeltaRow, int DeltaCol)> LinesNamed(string lineSet) => lineSet switch
    {
        nameof(ChessRules.RookLines) => [.. ChessRules.RookLines],
        nameof(ChessRules.BishopLines) => [.. ChessRules.BishopLines],
        _ => [.. ChessRules.QueenLines],
    };
}
