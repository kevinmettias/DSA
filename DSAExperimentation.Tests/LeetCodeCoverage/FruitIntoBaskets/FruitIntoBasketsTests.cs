using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FruitIntoBaskets;

// LeetCode 904. Fruit Into Baskets: "pick at most 2 distinct tree types into a
// contiguous run" is the at-most-K-distinct sliding window shape this repo's
// Permutation in String / Find All Anagrams coverage already uses for a fixed-size
// window - here the window instead grows/shrinks, tracked by a HashMap<int,int> of
// type -> count-in-window, whose own Count is exactly the number of distinct types
// currently held. The window only shrinks from the left when a third type appears,
// so every fruit enters and leaves the window at most once - O(n).
public sealed partial class FruitIntoBasketsTests
{
    [Fact]
    public void TotalFruit_LeetCodeExampleOne_ReturnsThree()
        => Assert.Equal(3, TotalFruit([1, 2, 1]));

    [Fact]
    public void TotalFruit_LeetCodeExampleTwo_ReturnsThree()
        => Assert.Equal(3, TotalFruit([0, 1, 2, 2]));

    [Fact]
    public void TotalFruit_LeetCodeExampleThree_ReturnsFour()
        => Assert.Equal(4, TotalFruit([1, 2, 3, 2, 2]));

    [Fact]
    public void TotalFruit_SingleTreeType_ReturnsWholeArray()
        => Assert.Equal(4, TotalFruit([5, 5, 5, 5]));

    private static int TotalFruit(int[] fruits)
    {
        var window = new FruitWindow();
        var longest = 0;

        for (var windowEnd = 0; windowEnd < fruits.Length; windowEnd++)
        {
            var extended = window.Extend(fruits, windowEnd);
            longest = Math.Max(longest, extended);
        }

        return longest;
    }

    private sealed class FruitWindow
    {
        private readonly HashMap<int, int> _basketCounts = new();
        private int _windowStart;

        public int Extend(int[] fruits, int windowEnd)
        {
            _basketCounts.TryGetValue(fruits[windowEnd], out var count);
            _basketCounts.Set(fruits[windowEnd], count + 1);

            while (_basketCounts.Count > 2)
            {
                ShrinkFromLeft(fruits);
            }

            return windowEnd - _windowStart + 1;
        }

        private void ShrinkFromLeft(int[] fruits)
        {
            var leaving = fruits[_windowStart];
            _basketCounts.TryGetValue(leaving, out var leavingCount);

            if (leavingCount == 1)
            {
                _basketCounts.TryRemove(leaving);
            }
            else
            {
                _basketCounts.Set(leaving, leavingCount - 1);
            }

            _windowStart++;
        }
    }
}
