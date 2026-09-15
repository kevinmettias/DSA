using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>;

namespace DSAExperimentation.LeetCode.ZeroOneMatrix;

// LeetCode 542. 01 Matrix: for every cell, the distance to its nearest 0 cell.
//
// The textbook baseline runs an independent single-source BFS outward from every
// 1 cell until it hits a 0 - O(rows*cols) work times rows*cols starting cells. The
// composed strategy instead seeds one shared frontier with every 0 cell at once, so
// a single multi-source BFS (this repo's own Queue<TElement> as the FIFO frontier)
// discovers each cell exactly once, via its true nearest source - O(rows*cols)
// total.
internal static class ZeroOneMatrixSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The naive baseline: a fresh BCL Queue + visited grid per starting cell,
    // re-walked outward until the nearest 0 is found. Deliberately written without
    // this repo's primitives - it is the arm the composed solution has to justify
    // itself against.
    public static int[][] UpdateMatrixByPerCellBfs(int[][] mat)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                result[r][c] = NearestZeroDistance(mat, r, c);
            }
        }

        return result;
    }

    private static int NearestZeroDistance(int[][] mat, int startRow, int startCol)
    {
        if (mat[startRow][startCol] == 0)
        {
            return 0;
        }

        var (visited, queue) = SeedSingleSourceSearch(mat, startRow, startCol);

        return RunSingleSourceBfs(mat, visited, queue);
    }

    // The single-source arm's counterpart to SeedDistancesAndFrontier: the search's own
    // visited grid and its one-cell frontier, already holding the starting cell at
    // distance 0.
    private static (bool[,] Visited, System.Collections.Generic.Queue<(int Row, int Col, int Dist)> Queue)
        SeedSingleSourceSearch(int[][] mat, int startRow, int startCol)
    {
        var visited = new bool[mat.Length, mat[0].Length];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Dist)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));

        return (visited, queue);
    }

    // The single-source BFS loop, the counterpart to RunMultiSourceBfs: walk the
    // frontier out from the one starting cell and answer with the distance the moment a
    // 0 cell is reached.
    private static int RunSingleSourceBfs(
        int[][] mat, bool[,] visited, System.Collections.Generic.Queue<(int Row, int Col, int Dist)> queue)
    {
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var distance = DistanceThroughNeighbor((mat, visited, queue), cell, (dRow, dCol));

                if (distance is int found)
                {
                    return found;
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // One neighbor of the cell just dequeued: off the board or already visited offers
    // nothing, a 0 cell is the answer, and anything else is claimed and enqueued one
    // step further out.
    private static int? DistanceThroughNeighbor(
        (int[][] Mat, bool[,] Visited, System.Collections.Generic.Queue<(int Row, int Col, int Dist)> Queue) search,
        (int Row, int Col, int Dist) cell,
        (int DRow, int DCol) delta)
    {
        var nextRow = cell.Row + delta.DRow;
        var nextCol = cell.Col + delta.DCol;

        if (IsOffBoard(nextRow, nextCol, search.Mat.Length, search.Mat[0].Length) ||
            search.Visited[nextRow, nextCol])
        {
            return null;
        }

        if (search.Mat[nextRow][nextCol] == 0)
        {
            return cell.Dist + 1;
        }

        search.Visited[nextRow, nextCol] = true;
        search.Queue.Enqueue((nextRow, nextCol, cell.Dist + 1));

        return null;
    }

    // This repo's own multi-source BFS: seed the frontier with every 0 cell at
    // distance 0 simultaneously, using Queue<TElement> as the FIFO frontier, so the
    // first time a 1 cell is reached is necessarily via its nearest 0 cell.
    public static int[][] UpdateMatrixByMultiSourceBfs(int[][] mat)
    {
        var (distances, frontier) = SeedDistancesAndFrontier(mat);

        RunMultiSourceBfs(distances, frontier);

        return distances;
    }

    private static (int[][] Distances, RepoQueue Frontier) SeedDistancesAndFrontier(int[][] mat)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;
        var distances = new int[rows][];
        var frontier = new RepoQueue();

        for (var r = 0; r < rows; r++)
        {
            distances[r] = SeedRow(mat, r, cols, frontier);
        }

        return (distances, frontier);
    }

    private static int[] SeedRow(int[][] mat, int row, int cols, RepoQueue frontier)
    {
        var distanceRow = new int[cols];

        for (var c = 0; c < cols; c++)
        {
            var isSeedCell = mat[row][c] == 0;

            distanceRow[c] = isSeedCell ? 0 : LeetCodeAnswer.None;

            if (mat[row][c] == 0)
            {
                frontier.Enqueue((row, c));
            }
        }

        return distanceRow;
    }

    private static void RunMultiSourceBfs(int[][] distances, RepoQueue frontier)
    {
        while (frontier.TryDequeue(out var cell))
        {
            ExploreNeighbors(cell, distances, frontier);
        }
    }

    private static void ExploreNeighbors((int Row, int Col) cell, int[][] distances, RepoQueue frontier)
    {
        var rows = distances.Length;
        var cols = distances[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = cell.Row + dRow;
            var nextCol = cell.Col + dCol;

            if (IsOffBoard(nextRow, nextCol, rows, cols) ||
                distances[nextRow][nextCol] != LeetCodeAnswer.None)
            {
                continue;
            }

            distances[nextRow][nextCol] = distances[cell.Row][cell.Col] + 1;
            frontier.Enqueue((nextRow, nextCol));
        }
    }

    // Both coordinates outside the matrix is one idea, and both BFS walks ask it of
    // the cell ahead before anything else.
    private static bool IsOffBoard(int row, int col, int rows, int cols)
        => row < 0 || row >= rows || col < 0 || col >= cols;
}
