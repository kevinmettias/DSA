using Microsoft.CodeAnalysis.CSharp;

namespace DSAExperimentation.Tests.Architecture;

// What LatticeTableCopies reads as a copy of a grid table, on snippets written each way a solution
// has written one, and what it leaves alone.
public sealed partial class LatticeTableCopiesTests
{
    [Fact]
    public void CopiesIn_OrthogonalStepsInAnotherOrder_IsACopyOfOrthogonal() =>
        AssertCopies("var steps = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };", "Orthogonal");

    [Fact]
    public void CopiesIn_KnightJumpsAsTwoNumberArrays_IsACopyOfLShaped() =>
        AssertCopies("int[][] jumps = [[1, 2], [1, -2], [-1, 2], [-1, -2], [2, 1], [2, -1], [-2, 1], [-2, -1]];", "LShaped");

    [Fact]
    public void CopiesIn_EightNeighboursAsNewArrays_IsACopyOfOctilinear() =>
        AssertCopies(
            "var steps = new[] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 }, new[] { -1, -1 }, new[] { -1, 1 }, new[] { 1, -1 }, new[] { 1, 1 } };",
            "Octilinear");

    [Fact]
    public void CopiesIn_TheFiveNumberRun_IsACopyOfOrthogonal() =>
        AssertCopies("int[] run = { -1, 0, 1, 0, -1 };", "Orthogonal");

    [Fact]
    public void CopiesIn_PairsNoTableHolds_IsNoCopy() =>
        AssertCopies("var steps = new (int, int)[] { (-1, 0), (1, 0), (0, -1), (1, 1) };");

    [Fact]
    public void CopiesIn_NamedSteps_IsNoCopy() =>
        AssertCopies("GridStep[] cycle = [GridStep.Up, GridStep.Right, GridStep.Down, GridStep.Left];");

    [Fact]
    public void CopiesIn_FewerPairsThanATable_IsNoCopy() =>
        AssertCopies("var steps = new (int, int)[] { (0, 1), (1, 0) };");

    private static void AssertCopies(string statement, params string[] tables)
    {
        var tree = CSharpSyntaxTree.ParseText($"class C {{ void M() {{ {statement} }} }}");

        Assert.Equal(tables, LatticeTableCopies.CopiesIn(tree, "C.cs").Select(copy => copy.Table).ToArray());
    }
}
