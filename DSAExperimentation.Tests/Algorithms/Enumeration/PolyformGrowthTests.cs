using DSAExperimentation.Algorithms.Enumeration;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;
using DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Enumeration;

// The counts of polyforms by size, from the literature, on three cell structures:
// - polyominoes: fixed OEIS A001168, one-sided A000988, free A000105;
// - polyhexes, on the test-only hexagonal lattice: fixed A001207, one-sided A006535, free A000228;
// - polyplets, squares joined at an edge or a corner: fixed A006770, free A030222.
// Each count was also found by a separate program, by Redelmeier's method for the fixed counts.
// The tetromino and pentomino presets are the classes growth finds, in every equivalence.
public sealed partial class PolyformGrowthTests
{
    public static TheoryData<int, int, int, int> PolyominoCounts => new()
    {
        { 1, 1, 1, 1 }, { 2, 2, 1, 1 }, { 3, 6, 2, 2 }, { 4, 19, 7, 5 }, { 5, 63, 18, 12 },
        { 6, 216, 60, 35 }, { 7, 760, 196, 108 },
    };

    public static TheoryData<int, int, int, int> PolyhexCounts => new()
    {
        { 1, 1, 1, 1 }, { 2, 3, 1, 1 }, { 3, 11, 3, 3 }, { 4, 44, 10, 7 }, { 5, 186, 33, 22 },
        { 6, 814, 147, 82 },
    };

    public static TheoryData<int, int, int> PolypletCounts => new()
    {
        { 1, 1, 1 }, { 2, 4, 2 }, { 3, 20, 5 }, { 4, 110, 22 }, { 5, 638, 94 },
    };

    [Theory]
    [MemberData(nameof(PolyominoCounts))]
    public void Fixed_Polyominoes_AreTheKnownCount(
        int size, int fixedCount, int oneSided, int free) =>
        Assert.Equal(
            (fixedCount, oneSided, free),
            (PolyformGrowth.Fixed<GridStep, SquareCells>(size).Length,
                PolyformGrowth.Classes<GridStep, SquareCells, OneSided>(size).Length,
                PolyformGrowth.Classes<GridStep, SquareCells, Free>(size).Length));

    [Theory]
    [MemberData(nameof(PolyhexCounts))]
    public void Fixed_Polyhexes_AreTheKnownCount(
        int size, int fixedCount, int oneSided, int free) =>
        Assert.Equal(
            (fixedCount, oneSided, free),
            (PolyformGrowth.Fixed<HexStep, HexCells>(size).Length,
                PolyformGrowth.Classes<HexStep, HexCells, OneSided>(size).Length,
                PolyformGrowth.Classes<HexStep, HexCells, Free>(size).Length));

    [Theory]
    [MemberData(nameof(PolypletCounts))]
    public void Fixed_Polyplets_AreTheKnownCount(int size, int fixedCount, int free) =>
        Assert.Equal(
            (fixedCount, free),
            (PolyformGrowth.Fixed<GridStep, KingCells>(size).Length,
                PolyformGrowth.Classes<GridStep, KingCells, Free>(size).Length));

    [Fact]
    public void Fixed_FourSquares_AreTheTetrominoesEveryWayTurned() =>
        Assert.Equal(
            Tetrominoes.Free.SelectMany(piece => piece.Orientations<Free>()).Order(),
            PolyformGrowth.Fixed<GridStep, SquareCells>(4));

    [Fact]
    public void Fixed_AnySize_IsInPolyformOrder()
    {
        var pentominoes = PolyformGrowth.Fixed<GridStep, SquareCells>(5);

        Assert.Equal(pentominoes.Order(), pentominoes);
    }

    [Fact]
    public void Fixed_NoCells_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PolyformGrowth.Fixed<GridStep, SquareCells>(0));

    [Fact]
    public void Classes_FourSquaresFree_AreGolombsFiveTetrominoes() =>
        Assert.Equal(
            Tetrominoes.Free.Select(piece => piece.Canonical<Free>()).Order(),
            PolyformGrowth.Classes<GridStep, SquareCells, Free>(4));

    [Fact]
    public void Classes_FourSquaresOneSided_AreTetrissSeven() =>
        Assert.Equal(
            Tetrominoes.OneSided.Select(piece => piece.Canonical<OneSided>()).Order(),
            PolyformGrowth.Classes<GridStep, SquareCells, OneSided>(4));

    [Fact]
    public void Classes_FiveSquaresFree_AreTheTwelvePentominoes() =>
        Assert.Equal(
            Pentominoes.Free.Select(piece => piece.Canonical<Free>()).Order(),
            PolyformGrowth.Classes<GridStep, SquareCells, Free>(5));
}
