using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FruitIntoBaskets;

// LeetCode 904. Fruit Into Baskets: two baskets each hold one tree type, so the
// answer is the longest contiguous run containing at most two distinct values.
//
// TotalFruitByPerStartRescan is the textbook O(n^2) restart: from every start,
// extend right until a third distinct type appears. TotalFruitBySlidingWindow is
// the at-most-K-distinct window this repo's Permutation in String / Find All
// Anagrams coverage already uses for a fixed-size window, here grown and shrunk
// instead, tracked by a HashMap<int,int> of type -> count-in-window whose own
// Count is exactly the number of distinct types currently held. The window only
// shrinks from the left when a third type appears, so every fruit enters and
// leaves at most once - O(n).
internal static class FruitIntoBasketsSolution
{
    private const int MaxBasketTypes = 2;

    // Deliberately written without this repo's primitives - the baseline the
    // sliding window below has to justify itself against.
    public static int TotalFruitByPerStartRescan(int[] fruits)
    {
        var longest = 0;

        for (var start = 0; start < fruits.Length; start++)
        {
            var runLength = LongestRunFrom(fruits, start);

            longest = Math.Max(longest, runLength);
        }

        return longest;
    }

    // Extends right from one start until a third distinct tree type appears.
    private static int LongestRunFrom(int[] fruits, int start)
    {
        var seen = new HashSet<int>();
        var longest = 0;

        for (var end = start; end < fruits.Length; end++)
        {
            seen.Add(fruits[end]);

            if (seen.Count > MaxBasketTypes)
            {
                break;
            }

            longest = end - start + 1;
        }

        return longest;
    }

    public static int TotalFruitBySlidingWindow(int[] fruits)
    {
        var window = new FruitWindow(fruits);
        var longest = 0;

        for (var windowEnd = 0; windowEnd < fruits.Length; windowEnd++)
        {
            longest = Math.Max(longest, window.Extend(windowEnd));
        }

        return longest;
    }

    // The window itself: admits one more fruit on the right, then evicts from the
    // left for as long as a third tree type is held, and reports its own length.
    private sealed class FruitWindow(int[] fruits)
    {
        private readonly HashMap<int, int> _basketCounts = new();
        private int _windowStart;

        public int Extend(int windowEnd)
        {
            _basketCounts.TryGetValue(fruits[windowEnd], out var count);
            _basketCounts.Set(fruits[windowEnd], count + 1);

            while (_basketCounts.Count > MaxBasketTypes)
            {
                ShrinkFromLeft();
            }

            return windowEnd - _windowStart + 1;
        }

        private void ShrinkFromLeft()
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
