using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MatrixCellsInDistanceOrderBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the closed-form Manhattan distance plus Array.Sort
// against this repo's Grid BFS distance map plus MergeSort - so a harness whose arms disagree is
// timing two different problems. Answers come back one cell per position in nondecreasing distance
// order, which is the whole of what LC 1030 pins: it accepts any ordering among equidistant cells,
// and the two sorts are not tie-compatible - Array.Sort is unstable, MergeSort is stable, and they
// start from different input orders, so the raw coordinate sequences genuinely differ from the fifth
// cell onward on this fixture. AnswerText.Of on the raw coordinates would therefore compare two
// permutations the problem never fixed. What is compared instead is the quantity the problem does
// fix - each cell's Manhattan distance from the center, in order - with the full grid coverage
// checked separately, the same reading DSAExperimentation.Tests' own LC 1030 coverage uses.
public sealed partial class MatrixCellsInDistanceOrderBenchmarksTests
{
    private const int SmallestSize = 20;

    // The center both arms measure from: Size / 2 in each direction, the benchmark's own arithmetic.
    private const int CenterOffset = SmallestSize / 2;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            AnswerText.Of(DistancesOf(BuildHarness().ManhattanFormulaThenArraySort())),
            AnswerText.Of(DistancesOf(BuildHarness().ManhattanFormulaThenArraySort())));
        Assert.Equal(
            AnswerText.Of(DistancesOf(BuildHarness().GridBfsThenMergeSort())),
            AnswerText.Of(DistancesOf(BuildHarness().GridBfsThenMergeSort())));
    }

    [Fact]
    public void ManhattanFormulaThenArraySort_OpenGrid_AgreesWithGridBfsThenMergeSortOnDistanceOrder()
    {
        var harness = BuildHarness();
        var byFormula = harness.ManhattanFormulaThenArraySort();
        var byGridBfs = harness.GridBfsThenMergeSort();

        Assert.Equal(AnswerText.Of(DistancesOf(byGridBfs)), AnswerText.Of(DistancesOf(byFormula)));
        Assert.Equal(AnswerText.OfUnorderedSet(EveryCell()), AnswerText.OfUnorderedSet(byFormula));
        Assert.Equal(AnswerText.OfUnorderedSet(EveryCell()), AnswerText.OfUnorderedSet(byGridBfs));
    }

    [Fact]
    public void GridBfsThenMergeSort_OpenGrid_AgreesWithManhattanFormulaThenArraySortOnDistanceOrder()
    {
        var harness = BuildHarness();
        var byGridBfs = harness.GridBfsThenMergeSort();
        var byFormula = harness.ManhattanFormulaThenArraySort();

        Assert.Equal(AnswerText.Of(DistancesOf(byFormula)), AnswerText.Of(DistancesOf(byGridBfs)));
        Assert.Equal(AnswerText.OfUnorderedSet(EveryCell()), AnswerText.OfUnorderedSet(byGridBfs));
        Assert.Equal(AnswerText.OfUnorderedSet(EveryCell()), AnswerText.OfUnorderedSet(byFormula));
    }

    private static IEnumerable<int> DistancesOf(int[][] cells) =>
        cells.Select(cell => Math.Abs(cell[0] - CenterOffset) + Math.Abs(cell[1] - CenterOffset));

    // Every cell of the grid, derived from the fixed Size alone - the grid the BFS walks is
    // uniformly passable, so this is the multiset both arms must return.
    private static IEnumerable<int[]> EveryCell() =>
        Enumerable.Range(0, SmallestSize)
            .SelectMany(row => Enumerable.Range(0, SmallestSize).Select(col => new[] { row, col }));

    private static MatrixCellsInDistanceOrderBenchmarks BuildHarness()
    {
        var harness = new MatrixCellsInDistanceOrderBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
