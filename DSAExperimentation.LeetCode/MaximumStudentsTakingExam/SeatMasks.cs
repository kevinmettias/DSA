namespace DSAExperimentation.LeetCode.MaximumStudentsTakingExam;

// LC 1349's seat chart reduced to the bitmasks both strategies actually search over:
// one int per row holding that row's broken seats, and the mask with every column of
// a row occupied. Meaningless outside this problem, so it lives beside the solution
// (ARCHITECTURE.md §17.3) rather than in Domain/.
//
// It is also the prepared-input shape §17.4 calls for: a benchmark builds the masks
// once in [GlobalSetup] and hands this to the measured method, and since it is not
// an IEnumerable it can never be confused with the char[][] overload.
internal sealed class SeatMasks(int[] brokenMask, int fullMask)
{
    // LeetCode marks a broken seat '#' and an available one '.'.
    private const char Broken = '#';

    // brokenMask[r] has bit c set when seats[r][c] is '#', i.e. that seat is broken.
    public int[] BrokenMask { get; } = brokenMask;

    // Every column bit set: the largest row mask the search enumerates up to.
    public int FullMask { get; } = fullMask;

    // The row count is the mask count - one entry per row, broken or not.
    public int Rows => BrokenMask.Length;

    public static SeatMasks Build(char[][] seats)
    {
        var columns = seats[0].Length;

        return new SeatMasks(BuildBrokenMasks(seats, columns), (1 << columns) - 1);
    }

    private static int[] BuildBrokenMasks(char[][] seats, int columns)
    {
        var brokenMask = new int[seats.Length];

        for (var row = 0; row < seats.Length; row++)
        {
            for (var col = 0; col < columns; col++)
            {
                if (seats[row][col] == Broken)
                {
                    brokenMask[row] |= 1 << col;
                }
            }
        }

        return brokenMask;
    }
}
