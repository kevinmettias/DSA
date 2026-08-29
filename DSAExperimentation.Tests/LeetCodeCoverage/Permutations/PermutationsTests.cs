using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Permutations;

public sealed partial class PermutationsTests
{
    [Fact]
    public void Permute_ThreeItems_ReturnsSixPermutations() => Assert.Equal(6, Permute([1, 2, 3]).Count);
    private static List<List<int>> Permute(int[] nums) { var results = new List<List<int>>(); var state = new State(nums.Length); Backtrack.Search<State, int>(state, s => s.Values.Count == nums.Length, s => s.Values.Count == nums.Length ? [] : Enumerable.Range(0, nums.Length).Where(i => !s.Used[i]), (s, i) => { s.Used[i] = true; s.Values.Add(nums[i]); }, (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); }, s => results.Add([.. s.Values])); return results; }
    private sealed class State(int length) { public bool[] Used { get; } = new bool[length]; public List<int> Values { get; } = []; }
}

