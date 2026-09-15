using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.CountAlmostEqualPairsII;

// LeetCode 3267. Count Almost Equal Pairs II: count index pairs (i, j) where
// nums[i] and nums[j] can be made equal using at most two digit swaps total, split
// however between the two numbers. A swap performed on the second number to move it
// toward the first is exactly as good as the same swap performed on the first number
// to move it toward the second, so "at most two swaps split across x and y" collapses
// to "x reaches y within two swaps applied to x alone". Both numbers are zero-padded
// to PaddedWidth first (nums[i] < 1e7, and the problem allows leading zeros after a
// swap), so a swap can also move a digit into or out of the padding - that is exactly
// how "1 and 100" become almost equal in LC's own second example.
//
// CountByBoundedSwapBruteForce checks reachability by materializing every one- and
// two-swap result as a fresh string and comparing - the O(width^4) per-pair walk
// CountByBoundedSwapBacktrack has to justify itself against. Deliberately written
// without this repo's primitives beyond plain strings.
//
// CountByBoundedSwapBacktrack asks the exact same question - "is target reachable
// within budget swaps?" - through Backtrack.TrySearch: SwapBudgetState answers
// Candidates itself (stop offering swaps once the budget is spent), and TrySearch's
// "find one and quit" contract (GenerateParenthesesSolution.GenerateByBacktracking
// uses the enumerate-everything sibling instead, since it wants every solution, not
// just reachability) is exactly a bounded reachability check.
//
// Both arms take a second, string[]-of-PaddedWidth overload so a benchmark can charge
// the zero-padding to [GlobalSetup] rather than either measured pairwise scan.
internal static class CountAlmostEqualPairsIISolution
{
    private const int PaddedWidth = 7; // nums[i] < 1e7, so at most 7 digits.
    private const int SwapBudget = 2;

    public static long CountByBoundedSwapBruteForce(int[] nums) => CountByBoundedSwapBruteForce(Pad(nums));

    public static long CountByBoundedSwapBruteForce(string[] padded)
    {
        var count = 0L;

        for (var i = 0; i < padded.Length; i++)
        {
            for (var j = i + 1; j < padded.Length; j++)
            {
                if (IsWithinSwapBudgetBruteForce(padded[i], padded[j]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsWithinSwapBudgetBruteForce(string a, string b)
    {
        if (a == b)
        {
            return true;
        }

        var width = a.Length;

        for (var i1 = 0; i1 < width; i1++)
        {
            for (var j1 = i1 + 1; j1 < width; j1++)
            {
                var afterOneSwap = Swapped(a, i1, j1);

                if (afterOneSwap == b || IsOneSwapFrom(afterOneSwap, b))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // True when a single further swap anywhere in `value` reaches `target` - the
    // second of the two swaps the budget allows.
    private static bool IsOneSwapFrom(string value, string target)
    {
        for (var i = 0; i < value.Length; i++)
        {
            for (var j = i + 1; j < value.Length; j++)
            {
                if (Swapped(value, i, j) == target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string Swapped(string s, int i, int j)
    {
        var chars = s.ToCharArray();
        (chars[i], chars[j]) = (chars[j], chars[i]);
        return new string(chars);
    }

    public static long CountByBoundedSwapBacktrack(int[] nums) => CountByBoundedSwapBacktrack(Pad(nums));

    public static long CountByBoundedSwapBacktrack(string[] padded)
    {
        var count = 0L;

        for (var i = 0; i < padded.Length; i++)
        {
            for (var j = i + 1; j < padded.Length; j++)
            {
                if (IsWithinSwapBudgetBacktrack(padded[i], padded[j]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsWithinSwapBudgetBacktrack(string a, string b)
    {
        var state = new SwapBudgetState(a.ToCharArray(), b.ToCharArray(), SwapBudget);

        return Backtrack.TrySearch<SwapBudgetState, (int I, int J)>(
            state,
            new BacktrackingSteps<SwapBudgetState, (int I, int J)>(
                IsSolution: s => s.MatchesTarget,
                Candidates: s => s.Candidates(),
                Choose: (s, swap) => s.Choose(swap),
                Unchoose: (s, swap) => s.Unchoose(swap),
                OnSolution: _ => true));
    }

    // internal, not private: the benchmark's [GlobalSetup] calls this directly so
    // padding is charged there, not to either measured pairwise scan.
    internal static string[] Pad(IReadOnlyList<int> nums)
    {
        var padded = new string[nums.Count];

        for (var i = 0; i < nums.Count; i++)
        {
            padded[i] = nums[i].ToString().PadLeft(PaddedWidth, '0');
        }

        return padded;
    }
}
