namespace DSAExperimentation.LeetCode.NumberOfWaysOfCuttingAPizza;

// LC 1444's pizza, stored as the suffix-sum table the recurrence actually reads:
// ApplesFrom(row, col) is the number of apples in the surviving bottom-right
// rectangle starting at (row, col), so "does this slice still contain an apple"
// becomes one subtraction instead of a scan - the same running-total-array
// technique RangeSumQuery2DImmutable uses, oriented bottom-right-first because
// every cut this problem makes keeps the bottom-right remainder.
//
// This lives beside the solution rather than in Domain/ or DataStructures/ because
// it fixes one problem's semantics - apples, and only the bottom-right suffix, the
// single orientation LC 1444 ever asks about (§17.3's problem-local witness rule).
// It is also the prepared-input type the benchmark's hoisted overload takes, so
// building the table is charged to [GlobalSetup] rather than to the measured cut
// count (§17.4).
internal readonly struct AppleGrid
{
    private const char Apple = 'A';

    // One row and one column of zero padding past the pizza, so the recurrence
    // below can read apples[row + 1, col] at the last row without a bounds test.
    private const int SuffixPadding = 1;

    private readonly int[,] _apples;

    public int Rows { get; }

    public int Cols { get; }

    public AppleGrid(string[] pizza)
    {
        Rows = pizza.Length;
        Cols = pizza[0].Length;
        _apples = new int[Rows + SuffixPadding, Cols + SuffixPadding];

        for (var row = Rows - 1; row >= 0; row--)
        {
            for (var col = Cols - 1; col >= 0; col--)
            {
                var isApple = pizza[row][col] == Apple;
                _apples[row, col] = (isApple ? 1 : 0)
                    + _apples[row + 1, col] + _apples[row, col + 1] - _apples[row + 1, col + 1];
            }
        }
    }

    // Apples in the bottom-right rectangle anchored at (row, col). Valid for
    // row in [0, Rows] and col in [0, Cols] - the padded row/column read as zero.
    public int ApplesFrom(int row, int col) => _apples[row, col];
}
