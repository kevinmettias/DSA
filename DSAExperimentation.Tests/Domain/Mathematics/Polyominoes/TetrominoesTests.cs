using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes;

// Each tetromino held to its drawing, in the orientation a Tetris well spawns it in, and the two
// sets held to the counts of tetrominoes in every equivalence: 5 free, 7 one-sided and 19 fixed
// (OEIS A000105, A000988 and A001168), each counted from the pieces' own orientations.
public sealed partial class TetrominoesTests
{
    [Fact]
    public void I_IsItsDrawing() => Assert.Equal(Drawing.Polyomino("####"), Tetrominoes.I);

    [Fact]
    public void O_IsItsDrawing() => Assert.Equal(Drawing.Polyomino("##", "##"), Tetrominoes.O);

    [Fact]
    public void T_IsItsDrawing() => Assert.Equal(Drawing.Polyomino(".#.", "###"), Tetrominoes.T);

    [Fact]
    public void S_IsItsDrawing() => Assert.Equal(Drawing.Polyomino(".##", "##."), Tetrominoes.S);

    [Fact]
    public void Z_IsItsDrawing() => Assert.Equal(Drawing.Polyomino("##.", ".##"), Tetrominoes.Z);

    [Fact]
    public void J_IsItsDrawing() => Assert.Equal(Drawing.Polyomino("#..", "###"), Tetrominoes.J);

    [Fact]
    public void L_IsItsDrawing() => Assert.Equal(Drawing.Polyomino("..#", "###"), Tetrominoes.L);

    [Fact]
    public void Z_IsSTurnedOver() => Assert.Equal(Tetrominoes.Z, Tetrominoes.S.Mirrored());

    [Fact]
    public void Free_AreFiveFreeClasses() => Assert.Equal(5, ClassesOf<Free>(Tetrominoes.Free));

    [Fact]
    public void Free_EitherSideUp_AreTheSevenOneSidedTetrominoes() =>
        Assert.Equal(7, ClassesOf<OneSided>(OrientationsOf<Free>(Tetrominoes.Free)));

    [Fact]
    public void Free_TurnedAndTurnedOver_AreTheNineteenFixedTetrominoes() =>
        Assert.Equal(
            (19, 19),
            (ClassesOf<Fixed>(OrientationsOf<Free>(Tetrominoes.Free)),
                Tetrominoes.Free.Sum(piece => piece.OrientationCount<Free>())));

    [Fact]
    public void OneSided_AreSevenOneSidedClasses() =>
        Assert.Equal(7, ClassesOf<OneSided>(Tetrominoes.OneSided));

    [Fact]
    public void OneSided_TurnedOver_AreTheFiveFree() =>
        Assert.Equal(5, ClassesOf<Free>(Tetrominoes.OneSided));

    [Fact]
    public void OneSided_Turned_AreTheNineteenFixedTetrominoes() =>
        Assert.Equal(
            (19, 19),
            (ClassesOf<Fixed>(OrientationsOf<OneSided>(Tetrominoes.OneSided)),
                Tetrominoes.OneSided.Sum(piece => piece.OrientationCount<OneSided>())));

    // How many classes of an equivalence the pieces fall into.
    private static int ClassesOf<TEquivalence>(IEnumerable<Polyform<GridStep, SquareCells>> pieces)
        where TEquivalence : struct, IPolyformEquivalence =>
        pieces.Select(piece => piece.Canonical<TEquivalence>()).Distinct().Count();

    // Every orientation of every piece under an equivalence.
    private static IEnumerable<Polyform<GridStep, SquareCells>> OrientationsOf<TEquivalence>(
        IEnumerable<Polyform<GridStep, SquareCells>> pieces)
        where TEquivalence : struct, IPolyformEquivalence =>
        pieces.SelectMany(piece => piece.Orientations<TEquivalence>());
}
