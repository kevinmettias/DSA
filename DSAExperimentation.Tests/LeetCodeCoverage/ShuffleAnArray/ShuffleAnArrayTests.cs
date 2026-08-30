using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShuffleAnArray;

// LeetCode 384. Shuffle an Array: in-place Fisher-Yates over this repo's own
// DynamicArray<int> - Get/Set swap each position against a random earlier-or-equal
// one, walking from the end down to index 1. O(n), no auxiliary "remaining pool"
// collection (contrast the naive remove-a-random-remaining-element approach the
// benchmark compares this against).
public sealed partial class ShuffleAnArrayTests
{
    [Fact]
    public void Reset_AfterShuffling_ReturnsOriginalConfiguration()
    {
        var solution = new Solution([1, 2, 3]);

        solution.Shuffle();

        Assert.Equal([1, 2, 3], solution.Reset());
    }

    [Fact]
    public void Shuffle_ReturnsAPermutationOfTheOriginalElements()
    {
        int[] original = [1, 2, 3, 4, 5];
        var solution = new Solution(original);

        var shuffled = solution.Shuffle();

        Assert.Equal(original.Length, shuffled.Length);
        Assert.Equal(original.OrderBy(x => x), shuffled.OrderBy(x => x));
    }

    private sealed class Solution
    {
        private readonly int[] _original;
        private readonly Random _random = new(1);

        public Solution(int[] nums) => _original = (int[])nums.Clone();

        public int[] Reset() => (int[])_original.Clone();

        public int[] Shuffle()
        {
            var values = new DynamicArray<int>();
            foreach (var value in _original)
            {
                values.Add(value);
            }

            for (var i = values.Count - 1; i > 0; i--)
            {
                var j = _random.Next(i + 1);
                var temp = values.Get(i);
                values.Set(i, values.Get(j));
                values.Set(j, temp);
            }

            var result = new int[values.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = values.Get(i);
            }

            return result;
        }
    }
}
