using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;
using DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;
using Polyomino = DSAExperimentation.Domain.Mathematics.Polyominoes.Polyform<
    DSAExperimentation.Domain.Mathematics.Lattices.GridStep,
    DSAExperimentation.Domain.Mathematics.Polyominoes.SquareCells>;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes;

// Normalization, the images under the lattice's symmetries, and the orbit-stabilizer count, on
// tetrominoes and pentominoes drawn as text, and on the hexagonal lattice, whose straight
// tri-hex lies along three axes rather than two.
public sealed partial class PolyformTests
{
    // The seven one-sided tetrominoes, then the twelve free pentominoes.
    private static readonly Polyform<GridStep, SquareCells>[] Pieces =
        [.. Tetrominoes.OneSided, .. Pentominoes.Free];

    // Each of Pieces with its orientations under the fixed, one-sided and free equivalences. A
    // piece's own symmetries number 8 / free orientations, by the orbit-stabilizer theorem.
    public static TheoryData<int, int, int, int> OrientationCounts => new()
    {
        { 0, 1, 2, 2 }, { 1, 1, 1, 1 }, { 2, 1, 4, 4 }, { 3, 1, 2, 4 }, { 4, 1, 2, 4 },
        { 5, 1, 4, 8 }, { 6, 1, 4, 8 },
        { 7, 1, 4, 8 }, { 8, 1, 2, 2 }, { 9, 1, 4, 8 }, { 10, 1, 4, 8 }, { 11, 1, 4, 8 },
        { 12, 1, 4, 4 }, { 13, 1, 4, 4 }, { 14, 1, 4, 4 }, { 15, 1, 4, 4 }, { 16, 1, 1, 1 },
        { 17, 1, 4, 8 }, { 18, 1, 2, 4 },
    };

    [Fact]
    public void Of_TranslatedSquares_IsTheSamePolyform() =>
        Assert.Equal(Polyomino.Of([new(0, 0), new(0, 1)]), Polyomino.Of([new(5, 7), new(5, 8)]));

    [Fact]
    public void Of_SquaresInAnyOrder_ListsThemByRowFromTheOrigin() =>
        Assert.Equal(
            [new(0, 0), new(1, -1), new(1, 0), new(1, 1)],
            Polyomino.Of([new(4, 4), new(3, 4), new(4, 3), new(4, 5)]).Cells.ToArray());

    [Fact]
    public void Of_ASquareTwice_Throws() =>
        Assert.Throws<ArgumentException>(() => Polyomino.Of([new(0, 0), new(0, 1), new(0, 0)]));

    [Fact]
    public void Of_SquaresApart_Throws() =>
        Assert.Throws<ArgumentException>(() => Polyomino.Of([new(0, 0), new(0, 2)]));

    [Fact]
    public void Of_SquaresMeetingAtACorner_Throws() =>
        Assert.Throws<ArgumentException>(() => Polyomino.Of([new(0, 0), new(1, 1)]));

    [Fact]
    public void Of_SquaresMeetingAtACornerJoinedAsAKing_IsOnePiece() =>
        Assert.Equal(2, Polyform<GridStep, KingCells>.Of([new(0, 0), new(1, 1)]).Size);

    [Fact]
    public void Of_NoCells_IsTheEmptyPolyform() => Assert.Equal(default, Polyomino.Of([]));

    [Fact]
    public void Cells_Default_IsEmpty() =>
        Assert.Empty(default(Polyform<GridStep, SquareCells>).Cells);

    [Fact]
    public void Size_Pentomino_IsFive() => Assert.Equal(5, Pentominoes.X.Size);

    // "..#/###" turned a quarter clockwise stands on its foot.
    [Fact]
    public void Turned_AQuarterTurn_TurnsClockwise() =>
        Assert.Equal(Drawing.Polyomino("#.", "#.", "##"), Tetrominoes.L.Turned(1));

    [Fact]
    public void Turned_MinusOne_IsThreeQuarterTurns() =>
        Assert.Equal(Tetrominoes.L.Turned(3), Tetrominoes.L.Turned(-1));

    [Fact]
    public void Turned_AWholeTurn_IsThePiece() =>
        Assert.Equal(Pentominoes.F, Pentominoes.F.Turned(4));

    [Fact]
    public void Mirrored_L_IsJ() => Assert.Equal(Tetrominoes.J, Tetrominoes.L.Mirrored());

    [Fact]
    public void Mirrored_Twice_IsThePiece() =>
        Assert.Equal(Pentominoes.N, Pentominoes.N.Mirrored().Mirrored());

    // A mirror is no turn: no turn of the L is the J.
    [Fact]
    public void Mirrored_L_IsNoTurnOfIt() =>
        Assert.DoesNotContain(Tetrominoes.L.Mirrored(), Tetrominoes.L.Orientations<OneSided>());

    [Fact]
    public void Orientations_Fixed_IsThePieceAlone() =>
        Assert.Equal([Pentominoes.F], Pentominoes.F.Orientations<Fixed>().ToArray());

    [Fact]
    public void Orientations_OneSided_IsThePieceThenItsTurns() =>
        Assert.Equal(
            [
                Tetrominoes.L, Tetrominoes.L.Turned(1), Tetrominoes.L.Turned(2),
                Tetrominoes.L.Turned(3),
            ],
            Tetrominoes.L.Orientations<OneSided>().ToArray());

    [Fact]
    public void Orientations_Free_ListsTheMirrorImagesTurnsAfterThePieces() =>
        Assert.Equal(
            [Tetrominoes.S, Tetrominoes.S.Turned(1), Tetrominoes.Z, Tetrominoes.Z.Turned(1)],
            Tetrominoes.S.Orientations<Free>().ToArray());

    [Theory]
    [MemberData(nameof(OrientationCounts))]
    public void Orientations_EachPiece_AreAsManyAsEachEquivalenceAllows(
        int piece, int fixedCount, int oneSidedCount, int freeCount) =>
        Assert.Equal(
            (fixedCount, oneSidedCount, freeCount),
            (Pieces[piece].Orientations<Fixed>().Length,
                Pieces[piece].Orientations<OneSided>().Length,
                Pieces[piece].Orientations<Free>().Length));

    [Theory]
    [MemberData(nameof(OrientationCounts))]
    public void SymmetryCount_EachPiece_TimesItsOrientationsIsTheEquivalencesOrder(
        int piece, int fixedCount, int oneSidedCount, int freeCount) =>
        Assert.Equal(
            (1, 4, 8),
            (fixedCount * Pieces[piece].SymmetryCount<Fixed>(),
                oneSidedCount * Pieces[piece].SymmetryCount<OneSided>(),
                freeCount * Pieces[piece].SymmetryCount<Free>()));

    [Theory]
    [MemberData(nameof(OrientationCounts))]
    public void OrientationCount_EachPiece_IsTheOrientationsListed(
        int piece, int fixedCount, int oneSidedCount, int freeCount) =>
        Assert.Equal(
            (fixedCount, oneSidedCount, freeCount),
            (Pieces[piece].OrientationCount<Fixed>(), Pieces[piece].OrientationCount<OneSided>(),
                Pieces[piece].OrientationCount<Free>()));

    [Fact]
    public void SymmetryCount_OUnderEachEquivalence_IsTheWholeGroup() =>
        Assert.Equal(
            (1, 4, 8),
            (Tetrominoes.O.SymmetryCount<Fixed>(), Tetrominoes.O.SymmetryCount<OneSided>(),
                Tetrominoes.O.SymmetryCount<Free>()));

    // The straight tri-hex lies along three axes, and is its own image under a half turn and two
    // mirrors: 12 / 4 = 3 orientations.
    [Fact]
    public void OrientationCount_StraightTriHex_IsThreeOfTwelve()
    {
        var bar = Polyform<HexStep, HexCells>.Of([new(0, 0), new(1, 0), new(2, 0)]);

        Assert.Equal(
            (3, 4, 3),
            (bar.OrientationCount<Free>(), bar.SymmetryCount<Free>(),
                bar.Orientations<Free>().Length));
    }

    [Fact]
    public void Canonical_EveryOrientation_IsTheSame() =>
        Assert.All(
            Pentominoes.F.Orientations<Free>(),
            image => Assert.Equal(Pentominoes.F.Canonical<Free>(), image.Canonical<Free>()));

    [Fact]
    public void Canonical_Free_IsTheLeastOrientation() =>
        Assert.Equal(Pentominoes.W.Orientations<Free>().Min(), Pentominoes.W.Canonical<Free>());

    [Fact]
    public void Canonical_OneSided_TellsLFromJ() =>
        Assert.NotEqual(Tetrominoes.L.Canonical<OneSided>(), Tetrominoes.J.Canonical<OneSided>());

    [Fact]
    public void IsEquivalentTo_MirrorImage_IsFreeButNotOneSided() =>
        Assert.Equal(
            (true, false),
            (Tetrominoes.J.IsEquivalentTo<Free>(Tetrominoes.L),
                Tetrominoes.J.IsEquivalentTo<OneSided>(Tetrominoes.L)));

    [Fact]
    public void IsEquivalentTo_Turn_IsOneSidedButNotFixed() =>
        Assert.Equal(
            (true, false),
            (Tetrominoes.T.IsEquivalentTo<OneSided>(Tetrominoes.T.Turned(1)),
                Tetrominoes.T.IsEquivalentTo<Fixed>(Tetrominoes.T.Turned(1))));

    [Fact]
    public void Equals_DifferentShapes_IsFalse() =>
        Assert.False(Tetrominoes.S.Equals(Tetrominoes.Z));

    [Fact]
    public void Equals_Translate_IsTrue() =>
        Assert.True(
            Tetrominoes.O.Equals(Polyomino.Of([new(-3, 9), new(-3, 10), new(-2, 9), new(-2, 10)])));

    [Fact]
    public void GetHashCode_Translates_HashAlike() =>
        Assert.Equal(
            Tetrominoes.I.GetHashCode(),
            Polyomino.Of([new(2, 2), new(2, 3), new(2, 4), new(2, 5)]).GetHashCode());

    [Fact]
    public void CompareTo_FewerCells_ComesFirst() =>
        Assert.True(Tetrominoes.O.CompareTo(Pentominoes.I) < 0);

    // Row by row: the I's second square is (0, 1), the O's too, and its third (0, 2) before (1, 0).
    [Fact]
    public void CompareTo_SameSize_ComparesCellByCell() =>
        Assert.Equal(
            (-1, 1, 0),
            (Math.Sign(Tetrominoes.I.CompareTo(Tetrominoes.O)),
                Math.Sign(Tetrominoes.O.CompareTo(Tetrominoes.I)),
                Tetrominoes.O.CompareTo(Tetrominoes.O)));
}
