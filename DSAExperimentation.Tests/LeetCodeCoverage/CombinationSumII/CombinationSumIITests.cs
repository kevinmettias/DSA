using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSumII;

public sealed partial class CombinationSumIITests
{
    [Fact]
    public void CombinationSum2_ClassicExample_ReturnsUniqueCombinations()
    {
        var results = Find([10, 1, 2, 7, 6, 1, 5], 8).Select(x => x.ToArray()).ToArray();
        Assert.Contains(results, x => x.SequenceEqual([1, 1, 6]));
        Assert.Contains(results, x => x.SequenceEqual([1, 2, 5]));
        Assert.Contains(results, x => x.SequenceEqual([1, 7]));
        Assert.Contains(results, x => x.SequenceEqual([2, 6]));
    }

    private static List<List<int>> Find(int[] candidates, int target)
    {
        Array.Sort(candidates);
        var results = new List<List<int>>();
        var state = new State();
        Backtrack.Search<State, int>(state, s => s.Sum == target, s => s.Sum == target ? [] : NextCandidates(candidates, target, s), (s, i) => { s.Values.Add(candidates[i]); s.Sum += candidates[i]; s.NextStart.Push(s.Start); s.Start = i + 1; }, (s, i) => { s.Start = s.NextStart.Pop(); s.Sum -= candidates[i]; s.Values.RemoveAt(s.Values.Count - 1); }, s => results.Add([.. s.Values]));
        return results;
    }

    private static IEnumerable<int> NextCandidates(int[] candidates, int target, State s)
    {
        for (var i = s.Start; i < candidates.Length; i++)
        {
            if (i > s.Start && candidates[i] == candidates[i - 1]) continue;
            if (s.Sum + candidates[i] <= target) yield return i;
        }
    }

    private sealed class State
    {
        public List<int> Values { get; } = [];
        public Stack<int> NextStart { get; } = new();
        public int Sum { get; set; }
        public int Start { get; set; }
    }
}
