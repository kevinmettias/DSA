using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.NumberOfSquarefulArrays;

// LeetCode 996. Number of Squareful Arrays: how many distinct permutations of nums
// have every adjacent pair summing to a perfect square.
//
// Both strategies sort a copy first so equal values sit together, which lets them
// share PermutationsII's duplicate rule: at a given depth, skip a candidate equal
// to its predecessor unless that predecessor is already placed. They differ in when
// the squareful property is checked - the baseline builds every distinct
// permutation and only then tests it, while the composed strategy folds the test
// into Backtrack.Search's candidate enumeration, abandoning a partial arrangement
// the moment its last placed pair fails instead of after every remaining position
// has been filled in.
internal static class NumberOfSquarefulArraysSolution
{
    // The textbook answer: plain recursion over a BCL used[] flag array, generating
    // every distinct permutation in full and filtering at the leaves. Deliberately
    // without this repo's backtracking engine - it is the arm the pruned strategy
    // is measured against.
    public static int NumSquarefulPermsByFullPermutationFilter(int[] nums)
    {
        var sorted = SortedCopy(nums);
        var state = new PermutationState(sorted);

        return Permute(state, depth: 0);
    }

    private static int Permute(PermutationState state, int depth)
    {
        if (depth == state.Nums.Length)
        {
            return IsSquareful(state.Current) ? 1 : 0;
        }

        var count = 0;

        for (var i = 0; i < state.Nums.Length; i++)
        {
            count += TryPlace(state, depth, i);
        }

        return count;
    }

    private static int TryPlace(PermutationState state, int depth, int i)
    {
        var used = state.Used;
        var nums = state.Nums;

        if (used[i] || (i > 0 && nums[i] == nums[i - 1] && !used[i - 1]))
        {
            return 0;
        }

        used[i] = true;
        state.Current[depth] = nums[i];
        var count = Permute(state, depth + 1);
        used[i] = false;

        return count;
    }

    private static bool IsSquareful(int[] arrangement)
    {
        for (var i = 1; i < arrangement.Length; i++)
        {
            if (!IsPerfectSquare(arrangement[i] + arrangement[i - 1]))
            {
                return false;
            }
        }

        return true;
    }

    // This repo's Backtrack.Search drives the identical choose/explore/unchoose
    // shape declaratively, with both the duplicate skip and the perfect-square
    // adjacency folded into the candidate enumeration so illegal prefixes are never
    // extended.
    public static int NumSquarefulPermsByPrunedBacktracking(int[] nums)
    {
        var sorted = SortedCopy(nums);
        var count = 0;
        var state = new State(sorted.Length);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == sorted.Length,
            s => s.Values.Count == sorted.Length ? [] : NextCandidates(sorted, s),
            (s, i) => { s.Used[i] = true; s.Values.Add(sorted[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            _ => count++);

        return count;
    }

    private static IEnumerable<int> NextCandidates(int[] sorted, State s)
    {
        for (var i = 0; i < sorted.Length; i++)
        {
            if (!s.Used[i] &&
                (i == 0 || sorted[i] != sorted[i - 1] || s.Used[i - 1]) &&
                (s.Values.Count == 0 || IsPerfectSquare(sorted[i] + s.Values[^1])))
            {
                yield return i;
            }
        }
    }

    private static bool IsPerfectSquare(int value)
    {
        var root = (int)Math.Sqrt(value);
        return root * root == value || (root + 1) * (root + 1) == value;
    }

    private static int[] SortedCopy(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    private sealed class State(int length)
    {
        public bool[] Used { get; } = new bool[length];

        public List<int> Values { get; } = [];
    }

    private sealed class PermutationState(int[] nums)
    {
        public int[] Nums { get; } = nums;

        public bool[] Used { get; } = new bool[nums.Length];

        public int[] Current { get; } = new int[nums.Length];
    }
}
