using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.PermutationsII;

// LeetCode 47. Permutations II: every distinct ordering of nums, where nums may
// contain duplicate values that must not produce duplicate permutations in the
// output.
//
// Sorting first groups equal values together so both strategies can apply the same
// rule: at a given recursion depth, skip a candidate equal to its predecessor
// unless that predecessor is already placed (choosing the unplaced one first would
// just reorder two equal values into a permutation already produced by choosing
// the other one first). The two strategies differ only in what drives the choose/
// explore/unchoose recursion - Backtrack.Search's generic engine, or a specialized
// recursive function tracking a used[] array directly. Both build LeetCode's
// actual answer, a List<List<int>>; the benchmark this migrated out of had both
// arms merely counting completions - and worse, its two [Benchmark] methods called
// the exact same private Count function, so nothing was actually being compared.
// Promoted to the real answer per ARCHITECTURE.md 17.8's WordLadderII precedent.
internal static class PermutationsIISolution
{
    // The textbook answer: plain recursion over a BCL used[] flag array and a
    // List<int> path buffer, without this repo's backtracking engine. Deliberately
    // BCL-only - the baseline PermuteUniqueByBacktracking is measured against.
    public static List<List<int>> PermuteUniqueBySpecializedRecursion(int[] nums)
    {
        var sorted = SortedCopy(nums);
        var results = new List<List<int>>();
        var used = new bool[sorted.Length];
        var path = new List<int>();

        Search(sorted, results, used, path);

        return results;
    }

    // This repo's Backtrack.Search primitive drives the identical
    // choose/explore/unchoose shape declaratively, with the same duplicate-skip
    // folded into the candidate enumeration.
    public static List<List<int>> PermuteUniqueByBacktracking(int[] nums)
    {
        var sorted = SortedCopy(nums);
        var results = new List<List<int>>();
        var state = new State(sorted.Length);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == sorted.Length,
            s => s.Values.Count == sorted.Length ? Array.Empty<int>() : NextCandidates(sorted, s),
            (s, i) => { s.Used[i] = true; s.Values.Add(sorted[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            s => results.Add([.. s.Values]));

        return results;
    }

    private static IEnumerable<int> NextCandidates(int[] sorted, State s)
    {
        for (var i = 0; i < sorted.Length; i++)
        {
            if (IsEligibleCandidate(sorted, s, i))
            {
                yield return i;
            }
        }
    }

    // The duplicate rule stated positively: a candidate is offered when it is unplaced
    // and either nothing precedes it, or the value before it differs, or that earlier
    // copy has already been placed.
    private static bool IsEligibleCandidate(int[] sorted, State s, int i) =>
        !s.Used[i] && (i == 0 || sorted[i] != sorted[i - 1] || s.Used[i - 1]);

    // The recursion the baseline drives by hand: place one eligible candidate, recurse,
    // then undo the placement, so every completion of `path` is one distinct ordering.
    private static void Search(int[] sorted, List<List<int>> results, bool[] used, List<int> path)
    {
        if (path.Count == sorted.Length)
        {
            results.Add([.. path]);
            return;
        }

        for (var i = 0; i < sorted.Length; i++)
        {
            if (ShouldSkipCandidate(used, sorted, i))
            {
                continue;
            }

            used[i] = true;
            path.Add(sorted[i]);
            Search(sorted, results, used, path);
            path.RemoveAt(path.Count - 1);
            used[i] = false;
        }
    }

    // A candidate is skipped when its value is already placed, or when it repeats the
    // value just before it and that earlier copy is still unplaced - taking this one
    // first would only re-produce an ordering the earlier copy yields anyway.
    private static bool ShouldSkipCandidate(bool[] used, int[] sorted, int i) =>
        used[i] || (i > 0 && sorted[i] == sorted[i - 1] && !used[i - 1]);

    private static int[] SortedCopy(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    private sealed record State
    {
        public bool[] Used { get; }

        public List<int> Values { get; } = [];

        public State(int length) => Used = new bool[length];
    }
}
