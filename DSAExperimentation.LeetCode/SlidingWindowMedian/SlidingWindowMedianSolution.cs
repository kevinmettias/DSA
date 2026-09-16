using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.SlidingWindowMedian;

// LeetCode 480. Sliding Window Median: return the median of every contiguous
// window of size windowSize as nums slides across the array.
internal static class SlidingWindowMedianSolution
{
    private const int MedianAverageDivisor = 2;

    // The O(n*k log k) baseline most people reach for first: copy each windowSize-sized
    // window and Array.Sort it from scratch. Deliberately written without this
    // repo's primitives, the arm the two-heap strategy below has to justify
    // itself against.
    public static double[] MedianSlidingWindowBySortEachWindow(int[] nums, int windowSize)
    {
        var result = new double[nums.Length - windowSize + 1];
        var window = new int[windowSize];

        for (var start = 0; start <= nums.Length - windowSize; start++)
        {
            Array.Copy(nums, start, window, 0, windowSize);
            Array.Sort(window);
            result[start] = MedianOfSorted(window, windowSize);
        }

        return result;
    }

    private static double MedianOfSorted(int[] sortedWindow, int windowSize)
    {
        var mid = windowSize / MedianAverageDivisor;
        var isEvenSize = windowSize % MedianAverageDivisor == 0;

        return isEvenSize
            ? Mean(sortedWindow[mid - 1], sortedWindow[mid])
            : MiddleElement(sortedWindow, mid);
    }

    // The mean of two middle values, which is the median of an even-sized window.
    private static double Mean(int left, int right) => (left + right) / (double)MedianAverageDivisor;

    // The median of an odd-sized window: its single middle element.
    private static int MiddleElement(int[] sortedWindow, int mid) => sortedWindow[mid];

    // The classic two-heap approach - FindMedianFromDataStreamSolution's own
    // MaxHeapOrder<int>/MinHeapOrder<int> pair of this repo's Heap<T,TOrder>,
    // extended with lazy deletion (a HashMap<int,int> counting values pending
    // removal) so a value leaving the window is only actually popped once it
    // resurfaces to a heap's own root, instead of requiring an O(k) scan to
    // remove it from the middle of either heap. O(n log k) overall.
    public static double[] MedianSlidingWindowByTwoHeapsLazyDeletion(int[] nums, int windowSize)
    {
        var result = new double[nums.Length - windowSize + 1];
        var window = new TwoHeapWindow();

        for (var i = 0; i < nums.Length; i++)
        {
            window.Insert(nums[i]);

            if (i >= windowSize - 1)
            {
                result[i - windowSize + 1] = window.Median();
                window.Erase(nums[i - windowSize + 1]);
            }
        }

        return result;
    }

    private sealed class TwoHeapWindow
    {
        private readonly Heap<int, MaxHeapOrder<int>> _lower = new();
        private readonly Heap<int, MinHeapOrder<int>> _upper = new();
        private readonly HashMap<int, int> _delayed = new();
        private int _lowerSize;
        private int _upperSize;

        public void Insert(int num)
        {
            if (_lowerSize == 0 || num <= PeekLower())
            {
                _lower.Push(num);
                _lowerSize++;
            }
            else
            {
                _upper.Push(num);
                _upperSize++;
            }

            Rebalance();
        }

        public void Erase(int num)
        {
            MarkDelayed(num);

            if (num <= PeekLower())
            {
                _lowerSize--;

                if (num == PeekLower())
                {
                    PruneLower();
                }
            }
            else
            {
                _upperSize--;

                if (num == PeekUpper())
                {
                    PruneUpper();
                }
            }

            Rebalance();
        }

        private void MarkDelayed(int value)
        {
            _delayed.TryGetValue(value, out var count);
            _delayed.Set(value, count + 1);
        }

        public double Median()
            => _lowerSize == _upperSize
                ? Mean(PeekLower(), PeekUpper())
                : PeekLower();

        private void Rebalance()
        {
            if (_lowerSize > _upperSize + 1)
            {
                var moved = PeekLower();
                _lower.TryPop(out _);
                _lowerSize--;
                _upper.Push(moved);
                _upperSize++;
                PruneLower();
            }
            else if (_upperSize > _lowerSize)
            {
                var moved = PeekUpper();
                _upper.TryPop(out _);
                _upperSize--;
                _lower.Push(moved);
                _lowerSize++;
                PruneUpper();
            }
        }

        private void PruneLower()
        {
            while (_lower.TryPeek(out var top) && IsDelayed(top))
            {
                ClearOneDelayed(top);
                _lower.TryPop(out _);
            }
        }

        private void PruneUpper()
        {
            while (_upper.TryPeek(out var top) && IsDelayed(top))
            {
                ClearOneDelayed(top);
                _upper.TryPop(out _);
            }
        }

        private bool IsDelayed(int value) => _delayed.TryGetValue(value, out var count) && count > 0;

        private void ClearOneDelayed(int value)
        {
            _delayed.TryGetValue(value, out var count);

            if (count <= 1)
            {
                _delayed.TryRemove(value);
            }
            else
            {
                _delayed.Set(value, count - 1);
            }
        }

        private int PeekLower()
        {
            _lower.TryPeek(out var value);
            return value;
        }

        private int PeekUpper()
        {
            _upper.TryPeek(out var value);
            return value;
        }
    }
}
