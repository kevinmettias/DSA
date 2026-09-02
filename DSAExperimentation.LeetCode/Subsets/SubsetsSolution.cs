using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.Subsets;

// LeetCode 78. Subsets: every node in the choose/explore/unchoose recursion tree
// (including the empty and full selections) IS a valid subset, so IsSolution is
// unconditionally true and OnSolution just records a snapshot - the exhaustive-
// enumeration shape Backtrack.Search's own doc comment names this for.
internal static class SubsetsSolution
{
    public static List<List<int>> FindAllSubsetsByBacktrack(int[] nums)
    {
        var results = new List<List<int>>();
        var state = new SubsetState();

        Backtrack.Search<SubsetState, int>(
            state,
            isSolution: static _ => true,
            candidates: s => Enumerable.Range(s.NextIndex, nums.Length - s.NextIndex),
            choose: (s, index) =>
            {
                s.Chosen.Add(nums[index]);
                s.NextIndex = index + 1;
            },
            unchoose: (s, _) => s.Chosen.RemoveAt(s.Chosen.Count - 1),
            onSolution: s => results.Add(new List<int>(s.Chosen)));

        return results;
    }

    private sealed class SubsetState
    {
        public List<int> Chosen { get; } = [];

        public int NextIndex { get; set; }
    }
}
