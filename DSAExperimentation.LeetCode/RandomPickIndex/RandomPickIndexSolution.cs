using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.RandomPickIndex;

// LeetCode 398. Random Pick Index: given nums (which may hold duplicates) and a
// target, return a uniformly-random index whose value equals target.
//
// A Design problem's whole point is a sequence of calls against one
// instance - Pick alone, this time - so "every strategy for the problem" (§17.3)
// takes the form of two classes implementing a shared IRandomPickIndex surface,
// the same shape InsertDeleteGetRandomO1Solution already uses for its own
// Design-category problem.
internal static class RandomPickIndexSolution
{
    internal interface IRandomPickIndex
    {
        int Pick(int target);
    }

    // The textbook no-extra-memory answer: reservoir sampling re-scans the whole
    // array on every single Pick call, keeping O(1) extra space beyond the array
    // itself - deliberately without this repo's primitives, the arm the
    // hashmap-grouping strategy below has to justify itself against.
    internal sealed class RandomPickIndexByReservoirSampling : IRandomPickIndex
    {
        private readonly int[] _nums;
        private readonly Random _random = new();

        public RandomPickIndexByReservoirSampling(int[] nums) => _nums = nums;

        public int Pick(int target)
        {
            var seenCount = 0;
            var chosen = -1;

            for (var i = 0; i < _nums.Length; i++)
            {
                if (_nums[i] != target)
                {
                    continue;
                }

                seenCount++;
                if (_random.Next(seenCount) == 0)
                {
                    chosen = i;
                }
            }

            return chosen;
        }
    }

    // This repo's own HashMap<int, DynamicArray<int>> - group every index by its
    // value once at construction, so each Pick is a single O(1)-expected lookup
    // plus one uniform draw over that value's own index list, instead of a fresh
    // O(n) scan per call.
    internal sealed class RandomPickIndexByHashMapGrouping : IRandomPickIndex
    {
        private readonly HashMap<int, DynamicArray<int>> _indicesByValue = new();
        private readonly Random _random = new();

        public RandomPickIndexByHashMapGrouping(int[] nums)
        {
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
