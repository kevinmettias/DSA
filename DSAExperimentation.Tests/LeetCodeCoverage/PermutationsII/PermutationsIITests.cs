using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationsII;

public sealed partial class PermutationsIITests
{
    [Fact]
    public void PermuteUnique_WithDuplicate_ReturnsThreePermutations() => Assert.Equal(3, PermuteUnique([1, 1, 2]).Count);
    private static List<List<int>> PermuteUnique(int[] nums) { Array.Sort(nums); var results = new List<List<int>>(); var state = new State(nums.Length); Backtrack.Search<State, int>(state, s => s.Values.Count == nums.Length, s => s.Values.Count == nums.Length ? [] : Enumerable.Range(0, nums.Length).Where(i => !s.Used[i] && (i == 0 || nums[i] != nums[i - 1] || s.Used[i - 1])), (s, i) => { s.Used[i] = true; s.Values.Add(nums[i]); }, (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); }, s => results.Add([.. s.Values])); return results; }
    private sealed class State(int length) { public bool[] Used { get; } = new bool[length]; public List<int> Values { get; } = []; }
}
