using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.EscapeALargeMaze;

// LeetCode 1036. Escape a Large Maze: on a 10^6 x 10^6 board with at most 200
// blocked cells, can a walk from source reach target?
//
// The board is never materialized. DepthFirstSearch.Traverse already models an
// implicit graph through a bare Func<TNode, IEnumerable<TNode>> (its own doc
// comment names exactly this "board too large to store" shape), and
// Set<(int, int)> holds the blocked cells for O(1) membership. The successor
// closure caps itself at the standard blocked.Count * (blocked.Count - 1) / 2
// bound - the largest area that many blocked cells can wall off against a corner -
// by counting its own invocations (Traverse calls successors exactly once per
// newly visited node, so the closure's counter tracks the visited count without
// needing access to Traverse's internals) and returning no further neighbors past
// that bound. Reaching the bound means the start cell escaped into open space
// rather than a sealed pocket; running that capped search from both source and
// target, and checking whether either reaches the other, is the accepted
// algorithm.
//
// Both strategies take the board's side length. For the capped search it is only
// the coordinate clamp, so the LeetCode-shaped overloads default it to the real
// 10^6. The flood fill allocates two boardSize x boardSize grids and cannot be run
// at that size at all - which is precisely the point of the comparison - so it has
// no defaulted overload and is measured, and tested, on a reduced board.
internal static class EscapeALargeMazeSolution
{
    private const int PairCountDivisor = 2; // n choose 2: n * (n - 1) / PairCountDivisor
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer once the board is small enough to store: materialize the
    // blocked cells and a visited grid, flood fill from source, then ask whether
    // target was reached. O(boardSize^2) in both time and space, independent of how
    // few cells are actually blocked. Deliberately BCL throughout - it is the arm
    // the capped search below has to justify itself against.
    public static bool CanEscapeByFullBoardFloodFill(int[][] blockedCells, int[] source, int[] target, int boardSize) =>
        CanEscapeByFullBoardFloodFill(ToCells(blockedCells), ToCell(source), ToCell(target), boardSize);

    public static bool CanEscapeByFullBoardFloodFill(
        (int Row, int Col)[] blockedCells, (int Row, int Col) source, (int Row, int Col) target, int boardSize)
    {
        var blocked = new bool[boardSize, boardSize];

        foreach (var (row, col) in blockedCells)
        {
            blocked[row, col] = true;
        }

        var visited = new bool[boardSize, boardSize];
        var stack = new Stack<(int Row, int Col)>();
        stack.Push(source);
        visited[source.Row, source.Col] = true;

        var state = new FloodFillState(blocked, visited, stack, boardSize);

        while (stack.Count > 0)
        {
            VisitNeighbors(state, stack.Pop());
        }

        return visited[target.Row, target.Col];
    }

    private static void VisitNeighbors(FloodFillState state, (int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = cell.Row + dRow;
            var nextCol = cell.Col + dCol;

            if (!IsInBounds(nextRow, nextCol, state.BoardSize))
            {
                continue;
            }

            if (state.Visited[nextRow, nextCol] || state.Blocked[nextRow, nextCol])
            {
                continue;
            }

            state.Visited[nextRow, nextCol] = true;
            state.Stack.Push((nextRow, nextCol));
        }
    }

    private readonly record struct FloodFillState(
        bool[,] Blocked,
        bool[,] Visited,
        Stack<(int Row, int Col)> Stack,
        int BoardSize);

    // This repo's own implicit-graph search, capped at the pocket bound so its cost
    // depends on the blocked-cell count alone and not on the board size at all.
    public static bool CanEscapeByCappedTraversal(int[][] blockedCells, int[] source, int[] target) =>
        CanEscapeByCappedTraversal(blockedCells, source, target, EscapeALargeMazeBoard.Size);

    public static bool CanEscapeByCappedTraversal(int[][] blockedCells, int[] source, int[] target, int boardSize) =>
        CanEscapeByCappedTraversal(BuildBlocked(blockedCells), ToCell(source), ToCell(target), boardSize);

    public static bool CanEscapeByCappedTraversal(
        Set<(int Row, int Col)> blocked, (int Row, int Col) source, (int Row, int Col) target, int boardSize)
    {
        // Every helper below reads the blocked cells and the board's side length
        // together - they are the board, so they travel as one argument.
        var board = (Blocked: blocked, Size: boardSize);

        return CanEscapeOrReach(source, target, board)
            && CanEscapeOrReach(target, source, board);
    }

    private static bool CanEscapeOrReach(
        (int Row, int Col) start,
        (int Row, int Col) other,
        (Set<(int Row, int Col)> Blocked, int Size) board)
    {
        // The pocket bound is a property of the blocked set alone, so the search
        // derives it here instead of being handed a value computed from the same set.
        var threshold = board.Blocked.Count * (board.Blocked.Count - 1) / PairCountDivisor;
        var budget = new VisitBudget();

        var reached = DepthFirstSearch.Traverse(start, cell => Successors(cell, board, threshold, budget));

        return reached.Count > threshold || reached.Contains(other);
    }

    // Mutable per-search counter: Traverse calls the successors func exactly once
    // per newly visited node, so this tracks the visited count across calls without
    // needing access to Traverse's own internals. A captured local cannot do this
    // once Successors is a real method rather than a closure, hence the holder.
    private sealed class VisitBudget
    {
        public int VisitedCount { get; set; }
    }

    private static IEnumerable<(int Row, int Col)> Successors(
        (int Row, int Col) cell,
        (Set<(int Row, int Col)> Blocked, int Size) board,
        int threshold,
        VisitBudget budget)
    {
        budget.VisitedCount++;

        if (budget.VisitedCount > threshold)
        {
            yield break;
        }

        foreach (var direction in Directions)
        {
            if (TryGetOpenNeighbor(cell, direction, board, out var next))
            {
                yield return next;
            }
        }
    }

    private static bool TryGetOpenNeighbor(
        (int Row, int Col) cell,
        (int DRow, int DCol) direction,
        (Set<(int Row, int Col)> Blocked, int Size) board,
        out (int Row, int Col) next)
    {
        next = (Row: cell.Row + direction.DRow, Col: cell.Col + direction.DCol);

        return IsInBounds(next.Row, next.Col, board.Size) && !board.Blocked.Has(next);
    }

    private static bool IsInBounds(int row, int col, int boardSize) =>
        row >= 0 && row < boardSize && col >= 0 && col < boardSize;

    private static Set<(int Row, int Col)> BuildBlocked(int[][] blockedCells)
    {
        var blocked = new Set<(int Row, int Col)>();

        foreach (var cell in blockedCells)
        {
            blocked.TryAdd(ToCell(cell));
        }

        return blocked;
    }

    private static (int Row, int Col)[] ToCells(int[][] cells) => [.. cells.Select(ToCell)];

    private static (int Row, int Col) ToCell(int[] cell) => (cell[0], cell[1]);
}
