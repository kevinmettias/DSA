namespace DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

// The shared, read-only context a JumpGridNode carries a reference to - the same
// pattern DataStructures.Graph.Grids' Grid uses for GridNode/GridChildren, just
// reporting each cell's own jump distance instead of a bool passability flag.
//
// Answers this problem alone, so it lives beside the solution rather than in Domain/
// or DataStructures/ (ARCHITECTURE.md #17.3/#17.6): DataStructures.Graph.Grids' Grid
// fixes "edge = the 4 orthogonal neighbours", while this one reads a per-cell jump
// budget out of LC 2617's own input. It previously existed as a Tests fixture and
// again in Benchmarks/Fixtures; this is the declaration both harnesses now share.
//
// Doubles as the prepared input of #17.4's hoisted overload: it is not IEnumerable,
// so MinVisitedCellsByReduceGraph's two overloads can never be ambiguous.
internal sealed class JumpGrid(int[][] values)
{
    public int Rows => values.Length;

    public int Cols => values[0].Length;

    public int JumpDistance(int row, int col) => values[row][col];
}
