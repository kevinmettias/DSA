using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubsetsII;

public sealed partial class SubsetsIITests
{
    [Fact]
    public void SubsetsWithDup_ClassicExample_ReturnsUniqueSubsets()
    {
        var subsets = SubsetsWithDup([1, 2, 2]).Select(s => string.Join(",", s)).ToArray();
        Assert.Equal(6, subsets.Length);
        Assert.Contains("", subsets);
        Assert.Contains("1,2,2", subsets);
    }

    private static List<List<int>> SubsetsWithDup(int[] nums)
    {
        Array.Sort(nums);
        var results = new List<List<int>>();
        var state = new State();
        Backtrack.Search<State, int>(state, _ => true, s => Enumerable.Range(s.Start, nums.Length - s.Start).Where(i => i == s.Start || nums[i] != nums[i - 1]), (s, i) => { s.Starts.Push(s.Start); s.Values.Add(nums[i]); s.Start = i + 1; }, (s, _) => { s.Start = s.Starts.Pop(); s.Values.RemoveAt(s.Values.Count - 1); }, s => results.Add([.. s.Values]));
        return results;
    }

    private sealed class State { public List<int> Values { get; } = []; public int Start { get; set; } public Stack<int> Starts { get; } = new(); }
}
