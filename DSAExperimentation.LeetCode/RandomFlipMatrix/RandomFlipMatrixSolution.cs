using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.RandomFlipMatrix;

// LeetCode 519. Random Flip Matrix: uniformly pick a not-yet-flipped cell of an m x n
// matrix, without replacement, until Reset() clears the picks. A Design problem (an
// instance API, not a pure function), so - like InsertDeleteGetRandomO1Solution -
// every strategy is a class implementing the shared IFlipMatrix surface below rather
// than a static method sharing an <Operation>By<Strategy> name.
internal static class RandomFlipMatrixSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay the same Flip/Reset script against either strategy without
    // restating it.
    internal interface IFlipMatrix
    {
        int[] Flip();

        void Reset();
    }

    // The textbook answer: a materialized BCL List<int> of every not-yet-flipped flat
    // index, each flip picking a random List position and RemoveAt-ing it - an O(n)
    // shift of every subsequent element. Deliberately without this repo's primitives -
    // it is the arm the hashmap-swap strategy below has to justify itself against.
    internal sealed class FlipMatrixByListScan : IFlipMatrix
    {
        private readonly int _cols;
        private readonly int _totalCells;
        private readonly Random _random;
        private List<int> _remaining;

        public FlipMatrixByListScan(int rows, int cols, Random random)
        {
            _cols = cols;
            _totalCells = rows * cols;
            _random = random;
            _remaining = BuildRemaining(_totalCells);
        }

        public int[] Flip()
        {
            var pick = _random.Next(_remaining.Count);
            var value = _remaining[pick];
            _remaining.RemoveAt(pick);

            return [value / _cols, value % _cols];
        }

        public void Reset() => _remaining = BuildRemaining(_totalCells);

        private static List<int> BuildRemaining(int totalCells)
        {
            var remaining = new List<int>(totalCells);
            for (var i = 0; i < totalCells; i++)
            {
                remaining.Add(i);
            }

            return remaining;
        }
    }

    // This repo's own HashMap<int,int> (a sparse "logical value currently sitting at
    // this flat index" override map) composed with a plain Random - the
    // InsertDeleteGetRandomO1 precedent's "swap the picked slot with the last slot"
    // trick, applied without ever materializing the full m*n array. That sparsity is
    // what makes this practical at all: LeetCode's own constraints allow up to
    // 10^4 x 10^4 = 10^8 cells, far too large to allocate, while flip calls are capped
    // at 1000 - so the HashMap only ever grows to the number of cells actually
    // flipped. Reset drops the swap map and restores the full remaining count; it
    // never needs to revisit or clear any individual entry, since every index beyond
    // the current _remaining boundary is treated as untouched again regardless of what
    // it held before.
    internal sealed class FlipMatrixByHashMapSwapRemove : IFlipMatrix
    {
        private readonly int _cols;
        private readonly int _totalCells;
        private readonly Random _random;
        private HashMap<int, int> _swapped = new();
        private int _remaining;

        public FlipMatrixByHashMapSwapRemove(int rows, int cols, Random random)
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
