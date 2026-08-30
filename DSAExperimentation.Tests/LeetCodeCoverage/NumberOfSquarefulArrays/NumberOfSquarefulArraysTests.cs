using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSquarefulArrays;

// LeetCode 996. Number of Squareful Arrays: the same PermutationsII dedup shape
// (Backtrack.Search over a Used[] flag array, skipping duplicate values in a
// fixed relative order so equal-valued swaps never produce the same permutation
// twice) with one added Candidates predicate - the next number must sum with the
// last placed number to a perfect square.
public sealed partial class NumberOfSquarefulArraysTests
{
    [Fact]
    public void NumSquarefulPerms_ClassicExample_ReturnsTwoArrangements() =>
        Assert.Equal(2, NumSquarefulPerms([1, 17, 8]));

    [Fact]
    public void NumSquarefulPerms_AllPairsFormAPerfectSquare_CountsEveryDistinctArrangement() =>
        Assert.Equal(1, NumSquarefulPerms([2, 2, 2]));

    private static int NumSquarefulPerms(int[] nums)
    {
        Array.Sort(nums);
        var count = 0;
        var state = new State(nums.Length);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == nums.Length,
            s => s.Values.Count == nums.Length
                ? []
                : Enumerable.Range(0, nums.Length).Where(i =>
                    !s.Used[i] &&
                    (i == 0 || nums[i] != nums[i - 1] || s.Used[i - 1]) &&
                    (s.Values.Count == 0 || IsPerfectSquare(nums[i] + s.Values[^1]))),
            (s, i) => { s.Used[i] = true; s.Values.Add(nums[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            _ => count++);

        return count;
    }

    private static bool IsPerfectSquare(int value)
    {
        var root = (int)Math.Sqrt(value);
        return root * root == value || (root + 1) * (root + 1) == value;
    }

    private sealed class State(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }
}
