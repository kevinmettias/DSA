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

        var rows = mat.Length;
        var cols = mat[0].Length;
        var visited = new bool[rows, cols];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Dist)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));

        while (queue.Count > 0)
        {
            var (row, col, dist) = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = row + dRow;
                var nextCol = col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols || visited[nextRow, nextCol])
                {
                    continue;
                }

                if (mat[nextRow][nextCol] == 0)
                {
                    return dist + 1;
                }

                visited[nextRow, nextCol] = true;
                queue.Enqueue((nextRow, nextCol, dist + 1));
            }
        }

        return LeetCodeAnswer.None;
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
            distanceRow[c] = mat[row][c] == 0 ? 0 : LeetCodeAnswer.None;

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

            if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols ||
                distances[nextRow][nextCol] != LeetCodeAnswer.None)
            {
                continue;
            }

            distances[nextRow][nextCol] = distances[cell.Row][cell.Col] + 1;
            frontier.Enqueue((nextRow, nextCol));
        }
    }
}
