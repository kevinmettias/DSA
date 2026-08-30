using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SlidingWindowMedian;

// LeetCode 480. Sliding Window Median: the classic two-heap approach -
// FindMedianFromDataStreamTests' own MaxHeapOrder<int>/MinHeapOrder<int> pair of
// this repo's Heap<T,TOrder>, extended with lazy deletion (a HashMap<int,int>
// counting balls pending removal) so a value leaving the window is only actually
// popped once it resurfaces to a heap's own root, instead of requiring an O(k)
// scan to remove it from the middle of either heap.
public sealed partial class SlidingWindowMedianTests
{
    [Fact]
    public void MedianSlidingWindow_ClassicExample_ReturnsPerWindowMedians()
    {
        int[] nums = [1, 3, -1, -3, 5, 3, 6, 7];

        var result = MedianSlidingWindow(nums, k: 3);

        Assert.Equal([1.0, -1.0, -1.0, 3.0, 5.0, 6.0], result);
    }

    [Fact]
    public void MedianSlidingWindow_EvenWindowSize_AveragesTheTwoMiddleValues()
    {
        int[] nums = [1, 2, 3, 4, 2, 3, 1, 4, 2];

        var result = MedianSlidingWindow(nums, k: 4);

        Assert.Equal([2.5, 2.5, 3.0, 2.5, 2.5, 2.5], result);
    }

    [Fact]
    public void MedianSlidingWindow_WindowSizeOne_ReturnsInputUnchanged()
    {
        int[] nums = [4, -2, 9];

        var result = MedianSlidingWindow(nums, k: 1);

        Assert.Equal([4.0, -2.0, 9.0], result);
    }

    private static double[] MedianSlidingWindow(int[] nums, int k)
    {
        var result = new double[nums.Length - k + 1];
        var window = new SlidingWindowMedianOperations();

        for (var i = 0; i < nums.Length; i++)
        {
            window.Insert(nums[i]);

            if (i >= k - 1)
            {
                result[i - k + 1] = window.Median();
                window.Erase(nums[i - k + 1]);
            }
        }

        return result;
    }

    private sealed class SlidingWindowMedianOperations
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

        public double Median()
            => _lowerSize == _upperSize
                ? (PeekLower() + PeekUpper()) / 2.0
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

        private void MarkDelayed(int value)
        {
            _delayed.TryGetValue(value, out var count);
            _delayed.Set(value, count + 1);
        }

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
