using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Algorithms.ShortestPaths.Grids;

// The earliest second a walker leaving `start` at second 0 can stand on `target`, moving one
// orthogonal step at a time across a grid whose every cell carries a time. A step has no weight of
// its own: what it costs depends on the second it leaves at, which is one degree more dynamic than
// ShortestPath.Dijkstra's fixed-weight IEdgeTopology can express, so the relaxation is written here
// over the same Heap and ByPriorityOrder frontier Dijkstra uses, with the move priced by TRule.
// Settling cells in nondecreasing arrival order stays correct because a rule is first-in-first-out
// over the departures the search produces (IArrivalRule's laws).
//
// There are no walls and a rule never refuses a move, so every cell is reachable and the answer is
// always a number; a start or target off the grid throws. The search keeps flat arrays indexed
// row * cols + col for each cell's best arrival and whether it has settled, and walks
// GridDirections.Orthogonal with GridSize.HasCell, so nothing on the hot path hashes or allocates.
internal static class GridEarliestArrival
{
    private const string OffTheGrid = "Start and target must both be cells of the grid.";
    private const string NeverSettled = "Every cell is reachable, so the target is always settled.";

    public static int Time<TRule>(int[][] cellTimes, (int Row, int Col) start, (int Row, int Col) target)
        where TRule : struct, IArrivalRule
    {
        var search = Begin(cellTimes, start, target);

        while (search.Frontier.TryPop(out var entry))
        {
            if (entry.Node == target)
            {
                return entry.Priority;
            }

            if (TrySettle(search, entry.Node))
            {
                Relax<TRule>(search, cellTimes, entry);
            }
        }

        throw new InvalidOperationException(NeverSettled);
    }

    // The run's state with only the start reached, at second 0.
    private static Search Begin(int[][] cellTimes, (int Row, int Col) start, (int Row, int Col) target)
    {
        var size = GridSize.Of(cellTimes);

        if (!size.HasCell(start.Row, start.Col) || !size.HasCell(target.Row, target.Col))
        {
            throw new ArgumentOutOfRangeException(nameof(target), OffTheGrid);
        }

        var search = new Search(size, new int[size.Rows * size.Cols], new bool[size.Rows * size.Cols], new());
        Array.Fill(search.Best, int.MaxValue);
        search.Best[IndexOf(search, start)] = 0;
        search.Frontier.Push((start, 0));

        return search;
    }

    // Marks the cell settled, reporting false when an earlier, better copy already settled it.
    private static bool TrySettle(Search search, (int Row, int Col) cell)
    {
        var index = IndexOf(search, cell);

        if (search.Settled[index])
        {
            return false;
        }

        search.Settled[index] = true;

        return true;
    }

    // Offers every unsettled neighbour of the cell just settled the arrival this move reaches it at;
    // a better arrival re-enters the frontier, and the stale copy it leaves behind is skipped when it
    // surfaces, because its cell has settled by then.
    private static void Relax<TRule>(Search search, int[][] cellTimes, ((int Row, int Col) Node, int Priority) entry)
        where TRule : struct, IArrivalRule
    {
        foreach (var (deltaRow, deltaCol) in GridDirections.Orthogonal)
        {
            var neighbor = (Row: entry.Node.Row + deltaRow, Col: entry.Node.Col + deltaCol);

            if (!search.Size.HasCell(neighbor.Row, neighbor.Col))
            {
                continue;
            }

            var index = IndexOf(search, neighbor);
            var arrival = TRule.Arrive(entry.Priority, cellTimes[neighbor.Row][neighbor.Col]);

            if (!search.Settled[index] && arrival < search.Best[index])
            {
                search.Best[index] = arrival;
                search.Frontier.Push((neighbor, arrival));
            }
        }
    }

    private static int IndexOf(Search search, (int Row, int Col) cell) => (cell.Row * search.Size.Cols) + cell.Col;

    // One run's state: the board, each cell's best arrival so far, which cells have settled, and the
    // frontier ordered by arrival.
    private sealed record Search(
        GridSize Size,
        int[] Best,
        bool[] Settled,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> Frontier);
}
