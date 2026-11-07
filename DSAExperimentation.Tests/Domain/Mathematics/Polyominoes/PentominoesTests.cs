using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes;

// Each pentomino held to its drawing, and the set held to the counts of pentominoes in every
// equivalence: 12 free, 18 one-sided and 63 fixed (OEIS A000105, A000988 and A001168), each
// counted from the pieces' own orientations.
public sealed partial class PentominoesTests
{
    [Fact]
    public void F_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino(".##", "##.", ".#."), Pentominoes.F);

    [Fact]
    public void I_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("#", "#", "#", "#", "#"), Pentominoes.I);

    [Fact]
    public void L_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("#.", "#.", "#.", "##"), Pentominoes.L);

    [Fact]
    public void N_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino(".#", "##", "#.", "#."), Pentominoes.N);

    [Fact]
    public void P_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("##", "##", "#."), Pentominoes.P);

    [Fact]
    public void T_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("###", ".#.", ".#."), Pentominoes.T);

    [Fact]
    public void U_IsItsDrawing() => Assert.Equal(Drawing.Polyomino("#.#", "###"), Pentominoes.U);

    [Fact]
    public void V_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("#..", "#..", "###"), Pentominoes.V);

    [Fact]
    public void W_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("#..", "##.", ".##"), Pentominoes.W);

    [Fact]
    public void X_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino(".#.", "###", ".#."), Pentominoes.X);

    [Fact]
    public void Y_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino(".#", "##", ".#", ".#"), Pentominoes.Y);

    [Fact]
    public void Z_IsItsDrawing() =>
        Assert.Equal(Drawing.Polyomino("##.", ".#.", ".##"), Pentominoes.Z);

    [Fact]
    public void Free_AreTwelveFreeClasses() => Assert.Equal(12, FreeClasses().Count());

    // F, L, N, P, Y and Z differ from their mirror images, and the other six do not.
    [Fact]
    public void Free_EitherSideUp_AreTheEighteenOneSidedPentominoes() =>
        Assert.Equal(
            18,
            Pentominoes.Free
                .SelectMany(piece => piece.Orientations<Free>())
                .Select(image => image.Canonical<OneSided>())
                .Distinct()
                .Count());

    [Fact]
    public void Free_TurnedAndTurnedOver_AreTheSixtyThreeFixedPentominoes() =>
        Assert.Equal(
            (63, 63),
            (Pentominoes.Free.SelectMany(piece => piece.Orientations<Free>()).Distinct().Count(),
                Pentominoes.Free.Sum(piece => piece.OrientationCount<Free>())));

    private static IEnumerable<Polyform<GridStep, SquareCells>> FreeClasses() =>
        Pentominoes.Free.Select(piece => piece.Canonical<Free>()).Distinct();
}
