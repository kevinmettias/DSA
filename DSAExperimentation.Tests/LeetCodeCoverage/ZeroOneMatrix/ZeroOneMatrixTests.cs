namespace DSAExperimentation.Tests.LeetCodeCoverage.ZeroOneMatrix;

// LeetCode 542. 01 Matrix: multi-source BFS - every 0 cell starts the frontier at
// distance 0 simultaneously, so the first time a 1 cell is reached is necessarily
// via its nearest 0 cell. This repo's own Queue<TElement> supplies the FIFO
// frontier, the same primitive TwoSumTests composes a HashMap over - just seeded
// with every source at once instead of a single root.
public sealed partial class ZeroOneMatrixTests
{
    [Fact]
    public void UpdateMatrix_AllZerosExceptCenter_ReturnsRingOfOnes()
    {
        int[][] mat = [[0, 0, 0], [0, 1, 0], [0, 0, 0]];

        var result = UpdateMatrix(mat);

        int[][] expected = [[0, 0, 0], [0, 1, 0], [0, 0, 0]];
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UpdateMatrix_ClassicExample_ReturnsDistanceToNearestZero()
    {
        int[][] mat = [[0, 0, 0], [0, 1, 0], [1, 1, 1]];

        var result = UpdateMatrix(mat);

        int[][] expected = [[0, 0, 0], [0, 1, 0], [1, 2, 1]];
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UpdateMatrix_SingleZeroCell_ReturnsZero()
    {
        int[][] mat = [[0]];

        var result = UpdateMatrix(mat);

        Assert.Equal([[0]], result);
    }

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int[][] UpdateMatrix(int[][] mat)
    {
        var (distances, frontier) = SeedDistancesAndFrontier(mat);

        RunMultiSourceBfs(distances, frontier);

        return distances;
    }

    private static (int[][] Distances, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> Frontier) SeedDistancesAndFrontier(int[][] mat)
    {
        var distances = new int[mat.Length][];
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();

        PopulateDistancesAndFrontier(mat, distances, frontier);

        return (distances, frontier);
    }

    private static void PopulateDistancesAndFrontier(int[][] mat, int[][] distances, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        var cols = mat[0].Length;

        for (var r = 0; r < mat.Length; r++)
        {
            distances[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                distances[r][c] = mat[r][c] == 0 ? 0 : -1;

                if (mat[r][c] == 0)
                {
                    frontier.Enqueue((r, c));
                }
            }
        }
    }

    private static void RunMultiSourceBfs(int[][] distances, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        while (frontier.TryDequeue(out var cell))
        {
            ExploreNeighbors(cell, distances, frontier);
        }
    }

    private static void ExploreNeighbors(
        (int Row, int Col) cell,
        int[][] distances,
        DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        var rows = distances.Length;
        var cols = distances[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = cell.Row + dRow;
            var nextCol = cell.Col + dCol;

            if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols || distances[nextRow][nextCol] != -1)
            {
                continue;
            }

            distances[nextRow][nextCol] = distances[cell.Row][cell.Col] + 1;
            frontier.Enqueue((nextRow, nextCol));
        }
    }
}
