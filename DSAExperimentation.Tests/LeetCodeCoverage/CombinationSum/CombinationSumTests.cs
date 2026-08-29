using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSum;

public sealed partial class CombinationSumTests
{
    [Fact]
    public void CombinationSum_ClassicExample_ReturnsExpectedCombinations()
    {
        var results = Find([2, 3, 6, 7], 7).Select(x => x.ToArray()).ToArray();
        Assert.Contains(results, x => x.SequenceEqual([2, 2, 3]));
        Assert.Contains(results, x => x.SequenceEqual([7]));
    }

    private static List<List<int>> Find(int[] candidates, int target)
    {
        Array.Sort(candidates);
        var results = new List<List<int>>();
        var state = new State();
        Backtrack.Search<State, int>(state, s => s.Sum == target, s => s.Sum == target ? [] : Enumerable.Range(s.Start, candidates.Length - s.Start).Where(i => s.Sum + candidates[i] <= target), (s, i) => { s.Values.Add(candidates[i]); s.Sum += candidates[i]; s.Start = i; }, (s, i) => { s.Sum -= candidates[i]; s.Values.RemoveAt(s.Values.Count - 1); s.Start = s.Values.Count == 0 ? 0 : Array.IndexOf(candidates, s.Values[^1]); }, s => results.Add([.. s.Values]));
        return results;
    }

    private sealed class State
    {
        public List<int> Values { get; } = [];
        public int Sum { get; set; }
        public int Start { get; set; }
    }
}

