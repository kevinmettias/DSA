using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomFlipMatrix;

// LeetCode 519. Random Flip Matrix: this repo's own HashMap<int,int> (a sparse
// "logical value currently sitting at this flat index" override map) composed with a
// plain Random - the InsertDeleteGetRandomO1 precedent's "swap the picked slot with
// the last slot" trick, applied without ever materializing the full m*n array. That
// sparsity is what makes this practical at all: LeetCode's own constraints allow up
// to 10^4 x 10^4 = 10^8 cells, far too large to allocate, while flip calls are capped
// at 1000 - so the HashMap only ever grows to the number of cells actually flipped.
// Reset drops the swap map and restores the full remaining count; it never needs to
// revisit or clear any individual entry, since every index beyond the current
// _remaining boundary is treated as untouched again regardless of what it held before.
public sealed partial class RandomFlipMatrixTests
{
    [Fact]
    public void Flip_UntilMatrixFull_VisitsEveryCellExactlyOnce()
    {
        var matrix = new FlipMatrix(2, 3, new Random(1));
        var seen = new HashSet<(int Row, int Col)>();

        for (var i = 0; i < 6; i++)
        {
            var cell = matrix.Flip();

            Assert.InRange(cell[0], 0, 1);
            Assert.InRange(cell[1], 0, 2);
            Assert.True(seen.Add((cell[0], cell[1])), "Flip returned an already-flipped cell before the matrix was full.");
        }

        Assert.Equal(6, seen.Count);
    }

    [Fact]
    public void Reset_AfterPartialFlips_AllowsFullCoverageAgain()
    {
        var matrix = new FlipMatrix(2, 2, new Random(2));
        matrix.Flip();
        matrix.Flip();

        matrix.Reset();

        var seen = new HashSet<(int Row, int Col)>();
        for (var i = 0; i < 4; i++)
        {
            var cell = matrix.Flip();
            seen.Add((cell[0], cell[1]));
        }

        Assert.Equal(4, seen.Count);
    }

    private sealed class FlipMatrix
    {
        private readonly int _cols;
        private readonly int _totalCells;
        private readonly Random _random;
        private HashMap<int, int> _swapped = new();
        private int _remaining;

        public FlipMatrix(int rows, int cols, Random random)
        {
            _cols = cols;
            _totalCells = rows * cols;
            _random = random;
            _remaining = _totalCells;
        }

        public int[] Flip()
        {
            var pick = _random.Next(_remaining);
            var value = _swapped.TryGetValue(pick, out var mapped) ? mapped : pick;

            _remaining--;
            var lastValue = _swapped.TryGetValue(_remaining, out var lastMapped) ? lastMapped : _remaining;
            _swapped.Set(pick, lastValue);

            return [value / _cols, value % _cols];
        }

        public void Reset()
        {
            _swapped = new HashMap<int, int>();
            _remaining = _totalCells;
        }
    }
}
