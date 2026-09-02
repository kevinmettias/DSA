using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumStudentsTakingExam;

// LeetCode 1349. Maximum Students Taking Exam: bitmask DP over "which seats in this
// row are occupied," row by row, via this repo's own Memoizer - the same (Row,
// PrevMask)-tuple memo state LongestIncreasingPathInAMatrixTests' (Row,Col) state and
// SmallestSufficientTeamTests' bitmask state both already establish, just combined
// into one 2D memo key. A row's own mask is valid only if it avoids broken seats and
// has no two horizontally adjacent bits set (no self-cheating within the row); a
// transition from the previous row's mask is valid only if neither mask has an
// upper-left/upper-right diagonal overlap with the other (prevMask shifted one bit
// each way). The answer maximizes summed popcount across all rows.
public sealed class MaximumStudentsTakingExamTests
{
    [Fact]
    public void MaxStudents_LeetCodeExampleOne_ReturnsFour()
    {
        char[][] seats =
        [
            ['#', '.', '#', '#', '.', '#'],
            ['.', '#', '#', '#', '#', '.'],
            ['#', '.', '#', '#', '.', '#'],
        ];

        Assert.Equal(4, MaxStudents(seats));
    }

    [Fact]
    public void MaxStudents_LeetCodeExampleTwo_ReturnsThree()
    {
        char[][] seats =
        [
            ['.', '#'],
            ['#', '#'],
            ['#', '.'],
            ['#', '#'],
            ['.', '#'],
        ];

        Assert.Equal(3, MaxStudents(seats));
    }

    [Fact]
    public void MaxStudents_LeetCodeExampleThree_ReturnsTen()
    {
        char[][] seats =
        [
            ['#', '.', '.', '.', '#'],
            ['.', '#', '.', '#', '.'],
            ['.', '.', '#', '.', '.'],
            ['.', '#', '.', '#', '.'],
            ['#', '.', '.', '.', '#'],
        ];

        Assert.Equal(10, MaxStudents(seats));
    }

    [Fact]
    public void MaxStudents_EveryRowFullyBroken_ReturnsZero()
    {
        char[][] seats =
        [
            ['#', '#', '#'],
            ['#', '#', '#'],
        ];

        Assert.Equal(0, MaxStudents(seats));
    }

    private static int MaxStudents(char[][] seats)
    {
        var rows = seats.Length;
        var cols = seats[0].Length;

        var brokenMask = BuildBrokenMask(seats, rows, cols);
        var fullMask = (1 << cols) - 1;
        var context = new ExamContext(rows, fullMask, brokenMask);

        return Memoizer.Memoize<(int Row, int PrevMask), int>(
            (0, 0), (state, bestFrom) => BestFrom(state, bestFrom, context));
    }

    private readonly record struct ExamContext(int Rows, int FullMask, int[] BrokenMask);

    private static int[] BuildBrokenMask(char[][] seats, int rows, int cols)
    {
        var brokenMask = new int[rows];
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (seats[row][col] == '#')
                {
                    brokenMask[row] |= 1 << col;
                }
            }
        }

        return brokenMask;
    }

    private static int BestFrom(
        (int Row, int PrevMask) state, Func<(int Row, int PrevMask), int> bestFrom, ExamContext context)
    {
        var (row, prevMask) = state;
        if (row == context.Rows)
        {
            return 0;
        }

        var best = 0;
        for (var mask = 0; mask <= context.FullMask; mask++)
        {
            if (!IsValidRowMask(mask, prevMask, context.BrokenMask[row]))
            {
                continue;
            }

            best = Math.Max(best, PopCount(mask) + bestFrom((row + 1, mask)));
        }

        return best;
    }

    private static bool IsValidRowMask(int mask, int prevMask, int broken)
        => (mask & broken) == 0
            && (mask & (mask << 1)) == 0
            && (mask & (prevMask << 1)) == 0
            && (mask & (prevMask >> 1)) == 0;

    private static int PopCount(int mask)
    {
        var count = 0;
        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }
}
