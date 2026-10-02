using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.Subsets;

// LeetCode 78. Subsets: every node in the choose/explore/unchoose recursion tree
// (including the empty and full selections) IS a valid subset, so IsSolution is
// unconditionally true and OnSolution just records a snapshot - the exhaustive-
// enumeration shape Backtrack.Search's own doc comment names this for.
internal static class SubsetsSolution
{
    // The textbook arm the backtracking search is measured against: walk every bitmask
    // from 0 to 2^n - 1 and read off its set bits. Same answer with no recursion and no
    // choose/unchoose bookkeeping, but it always visits all 2^n masks and re-derives each
    // subset independently rather than growing one incrementally.
    public static List<List<int>> FindAllSubsetsByBitmask(int[] nums)
    {
        var subsetCount = 1 << nums.Length;
        var results = new List<List<int>>(subsetCount);

        for (var mask = 0; mask < subsetCount; mask++)
        {
            var subset = new List<int>();

            for (var bit = 0; bit < nums.Length; bit++)
            {
                if ((mask & (1 << bit)) != 0)
                {
                    subset.Add(nums[bit]);
                }
            }

            results.Add(subset);
        }

        return results;
    }

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
