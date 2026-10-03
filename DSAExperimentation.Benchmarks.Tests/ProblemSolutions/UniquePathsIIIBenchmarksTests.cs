using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniquePathsIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniquePathsIIISolution's competing strategies for the same question - a purpose-built recursion
// against the generic backtracking engine, whose per-branch delegate dispatch is the cost being
// measured - so a harness whose arms disagree is enumerating two different walks.
//
// Setup's grid is fully documented - UniquePathsIIIWorkloads with seeded Random(1083), two rows of
// five at the smallest size, one obstacle - so the expected count is derived here from a rebuilt
// grid by an independent enumeration: a walk that tracks the cells it has used as bits of one
// integer and counts the walks that reach the end having used every walkable cell. It shares
// neither arm's visited grid nor its marker rewriting, so the count is asserted as derived alongside
// the arms' agreement rather than left to the two arms to agree on a shared wrong number.
public sealed partial class UniquePathsIIIBenchmarksTests
{
    private const int SmallestRows = 2;
    private const int Columns = 5;
    private const int ObstacleCount = 1;
    private const int RandomSeed = 1083;

    private static readonly (int Row, int Column)[] Steps = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Fact]
    public void Setup_SameRows_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SpecializedRecursive(), BuildHarness().SpecializedRecursive());

    [Fact]
    public void SpecializedRecursive_SeededGrid_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(IndependentPathCount(RebuildGrid()), harness.SpecializedRecursive());
        Assert.Equal(harness.BacktrackEngine(), harness.SpecializedRecursive());
    }

    [Fact]
    public void BacktrackEngine_SeededGrid_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(IndependentPathCount(RebuildGrid()), harness.BacktrackEngine());
        Assert.Equal(harness.SpecializedRecursive(), harness.BacktrackEngine());
    }

    private static UniquePathsIIIBenchmarks BuildHarness()
    {
        var harness = new UniquePathsIIIBenchmarks { Rows = SmallestRows };
        harness.Setup();

        return harness;
    }

    // The benchmark's own grid, rebuilt from its documented shape.
    private static int[][] RebuildGrid() =>
        UniquePathsIIIWorkloads.BuildGrid(SmallestRows, Columns, ObstacleCount, new Random(RandomSeed));

    // Cell (row, column) is bit row * Columns + column. Obstacles start out used, so a walk is complete
    // exactly when every bit is set.
    private static int IndependentPathCount(int[][] grid)
    {
        var cells = grid.SelectMany(row => row).ToArray();
        var obstacles = Enumerable.Range(0, cells.Length)
            .Where(cell => cells[cell] == UniquePathsIIIWorkloads.Obstacle)
            .Aggregate(0, (used, cell) => used | (1 << cell));
        var start = Array.IndexOf(cells, UniquePathsIIIWorkloads.Start);

        return WalksFrom(start, obstacles | (1 << start));

        int WalksFrom(int cell, int used)
        {
            if (cells[cell] == UniquePathsIIIWorkloads.End)
            {
                var everyCell = (1 << cells.Length) - 1;

                return used == everyCell ? 1 : 0;
            }

            var (row, column) = (cell / Columns, cell % Columns);

            return Steps
                .Select(step => (Row: row + step.Row, Column: column + step.Column))
                .Where(next => next.Row >= 0 && next.Row < grid.Length && next.Column >= 0 && next.Column < Columns)
                .Select(next => (next.Row * Columns) + next.Column)
                .Where(next => (used & (1 << next)) == 0)
                .Sum(next => WalksFrom(next, used | (1 << next)));
        }
    }
}
