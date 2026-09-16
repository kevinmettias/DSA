using DSAExperimentation.LeetCode.PermutationsII;

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
// answer). The engine arm is PermutationsIISolution's, called through - see that
// class for why LC 47's driver is the general one - so this class's own writing is
// the specialized recursion it is measured against.
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

        Search(nums, used, path, results);

        return results;
    }

    // This repo's Backtrack.Search primitive drives the identical
    // choose/explore/unchoose shape declaratively. LC 47's class is where that
    // engine's permutation driver is written, because its duplicate-skip rule is
    // the general case and LC 46's input - all elements already distinct - is the
    // one that rule never fires on; this arm calls through. Nothing narrows: both
    // problems answer a List<List<int>> over the same input shape.
    public static List<List<int>> PermuteByBacktracking(int[] nums) =>
        PermutationsIISolution.PermuteUniqueByBacktracking(nums);

    // The choose/explore/unchoose step: a full path is one permutation, otherwise
    // every still-unused index is tried in turn and un-chosen again on the way back.
    private static void Search(int[] nums, bool[] used, List<int> path, List<List<int>> results)
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
            Search(nums, used, path, results);
            path.RemoveAt(path.Count - 1);
            used[i] = false;
        }
    }
}
