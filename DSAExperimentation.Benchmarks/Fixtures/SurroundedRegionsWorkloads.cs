namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 130 - a size x size board of 'X'/'O' cells
// with a coin flip per cell, dense enough that most 'O' cells end up in
// interior regions the DFS from the border never reaches, so the flip pass
// at the end has real work to do rather than restoring the board unchanged.
internal static class SurroundedRegionsWorkloads
{
    public static char[][] BuildBoard(int size, int seed)
    {
        var random = new Random(seed);
        var board = new char[size][];

        for (var row = 0; row < size; row++)
        {
            board[row] = new char[size];

            for (var col = 0; col < size; col++)
            {
                var isOpen = random.Next(2) == 0;
                board[row][col] = isOpen ? 'O' : 'X';
            }
        }

        return board;
    }
}
