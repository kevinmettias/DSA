using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickIndex;

// LeetCode 398. Random Pick Index: the InsertDeleteGetRandomO1 precedent (HashMap +
// DynamicArray + seeded Random) applied to a grouping problem instead of a swap-with-
// -tail removal one - HashMap<int, DynamicArray<int>> groups every index by its value
// once at construction, so each Pick(target) is a single O(1)-expected lookup plus one
// uniform draw over that value's own index list, instead of a fresh O(n) scan per call.
public sealed partial class RandomPickIndexTests
{
    [Fact]
    public void Pick_TargetWithMultipleOccurrences_AlwaysReturnsAMatchingIndex()
    {
        var solution = new Solution([1, 2, 3, 3, 3], seed: 1);

        for (var i = 0; i < 50; i++)
        {
            var picked = solution.Pick(3);
            Assert.Contains(picked, new[] { 2, 3, 4 });
        }
    }

    [Fact]
    public void Pick_TargetWithSingleOccurrence_AlwaysReturnsThatIndex()
    {
        var solution = new Solution([5, 1, 5, 2, 5, 5], seed: 1);

        Assert.Equal(3, solution.Pick(2));
    }

    [Fact]
    public void Pick_ManyCalls_EventuallyReturnsEveryMatchingIndex()
    {
        var solution = new Solution([5, 1, 5, 2, 5, 5], seed: 1);
        var seen = new HashSet<int>();

        for (var i = 0; i < 200; i++)
        {
            seen.Add(solution.Pick(5));
        }

        Assert.Equal(new[] { 0, 2, 4, 5 }, seen.OrderBy(index => index));
    }

    private sealed class Solution
    {
        private readonly HashMap<int, DynamicArray<int>> _indicesByValue = new();
        private readonly Random _random;

        public Solution(int[] nums, int seed)
        {
            _random = new Random(seed);

            for (var i = 0; i < nums.Length; i++)
            {
                if (!_indicesByValue.TryGetValue(nums[i], out var indices))
                {
                    indices = new DynamicArray<int>();
                    _indicesByValue.Set(nums[i], indices);
                }

                indices.Add(i);
            }
        }

        public int Pick(int target)
        {
            _indicesByValue.TryGetValue(target, out var indices);
            return indices.Get(_random.Next(indices.Count));
        }
    }
}
