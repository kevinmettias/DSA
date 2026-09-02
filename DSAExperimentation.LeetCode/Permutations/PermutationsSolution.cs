using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.Permutations;

// LeetCode 46. Permutations: every distinct ordering of nums (all elements already
// unique).
//
// The two strategies differ only in what drives the choose/explore/unchoose
// recursion - Backtrack.Search's generic engine mutating a dedicated State, or a
// specialized recursive function tracking a used[] array directly. Both build
// LeetCode's actual answer, a List<List<int>>; the benchmark this migrated out of
// had both arms merely counting completions, which is a valid measurement choice
// but not the question either strategy is proven correct against (ARCHITECTURE.md
// 17.8's WordLadderII precedent for promoting a counting arm back to the real
// answer).
internal static class PermutationsSolution
{
    // The textbook answer: plain recursion over a BCL used[] flag array and a
    // List<int> path buffer, without this repo's backtracking engine. Deliberately
    // BCL-only - the baseline PermuteByBacktracking is measured against.
    public static List<List<int>> PermuteBySpecializedRecursion(int[] nums)
    {
        var results = new List<List<int>>();
        var used = new bool[nums.Length];
        var path = new List<int>();

        void Search()
        {
            if (path.Count == nums.Length)
            {
                results.Add([.. path]);
                return;
            }

            for (var i = 0; i < nums.Length; i++)
            {
                if (used[i])
                {
                    continue;
                }

                used[i] = true;
                path.Add(nums[i]);
                Search();
                path.RemoveAt(path.Count - 1);
                used[i] = false;
            }
        }

        Search();
        return results;
    }

    // This repo's Backtrack.Search primitive drives the identical
    // choose/explore/unchoose shape declaratively.
    public static List<List<int>> PermuteByBacktracking(int[] nums)
    {
        var results = new List<List<int>>();
        var state = new State(nums.Length);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == nums.Length,
            s => s.Values.Count == nums.Length
                ? []
                : Enumerable.Range(0, nums.Length).Where(i => !s.Used[i]),
            (s, i) => { s.Used[i] = true; s.Values.Add(nums[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            s => results.Add([.. s.Values]));

        return results;
    }

    private sealed class State(int length)
    {
        public bool[] Used { get; } = new bool[length];

        public List<int> Values { get; } = [];
    }
}
