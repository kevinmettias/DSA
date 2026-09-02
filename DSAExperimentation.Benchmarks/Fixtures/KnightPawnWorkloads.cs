namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3283 - a knight start plus a random scattering
// of distinct pawns on the problem's fixed 50 x 50 board, sized within its own
// 1 <= positions.length <= 15 bound.
internal static class KnightPawnWorkloads
{
    private const int BoardSize = 50;

    public static (int Kx, int Ky, int[][] Positions) BuildGame(int pawnCount, int seed)
    {
        var random = new Random(seed);
        var occupied = new HashSet<(int Row, int Col)>();
        var (kx, ky) = NextSquare(random, occupied);
        var positions = new int[pawnCount][];

        for (var i = 0; i < pawnCount; i++)
        {
            var (row, col) = NextSquare(random, occupied);
            positions[i] = [row, col];
        }

        return (kx, ky, positions);
    }

    private static (int Row, int Col) NextSquare(Random random, HashSet<(int Row, int Col)> occupied)
    {
        (int Row, int Col) square;

        do
        {
            square = (random.Next(BoardSize), random.Next(BoardSize));
        }
        while (!occupied.Add(square));

        return square;
    }
}
