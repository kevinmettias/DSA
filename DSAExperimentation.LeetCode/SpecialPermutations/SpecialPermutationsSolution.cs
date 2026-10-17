using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.LeetCode.Modular;

namespace DSAExperimentation.LeetCode.SpecialPermutations;

// LeetCode 2741. Special Permutations: count the orderings of a distinct array
// in which every adjacent pair divides one another in one direction or the
// other, reported modulo 1e9+7.
//
// The two strategies differ in when the adjacency rule is applied. The brute
// force builds each of the n! orderings in full and tests the rule only once a
// complete ordering exists. The bitmask DP threads (Remaining, Last) - "these
// elements are still unplaced, Last is the element placed immediately before
// them" - through this repo's own Memoizer, so a suffix is counted once for
// every distinct (set, predecessor) it can follow rather than once per prefix
// that reaches it.
internal static class SpecialPermutationsSolution
{
    // Seeds the recurrence's start state: nothing has been placed yet, so the
    // first element is unconstrained and every element may go first.
    private const int NoPreviousElement = -1;

    // Enumerate every one of the n! full permutations by plain choose/recurse/unchoose
    // recursion - the textbook permutations walk - and check the adjacency rule only
    // after a complete ordering has been built. O(n! * n), the arm the bitmask DP has
    // to beat. The count is reported unreduced: this arm is only viable at
    // lengths where n! cannot come close to the modulus.
    public static int CountByBruteForceBacktracking(int[] nums)
    {
        var state = new PermutationState(nums.Length);

        return CountSpecialOrderings(state, nums);
    }

    // A single Memoizer call over state (Remaining, Last): Remaining is the set
    // of elements not yet placed, Last is the element most recently placed
    // (NoPreviousElement for the empty prefix). Base case Remaining == 0 is the
    // empty completion - one way to place nothing more. Every reachable state
    // is visited at most once, so the whole call tree is O(n^2 * 2^n)
    // regardless of how many of the n! orderings turn out to be legal - the
    // same (Mask, Last) TSP-style state FindTheShortestSuperstringSolution
    // establishes for this repo's bitmask DP over permutations, here summing
    // completions instead of tracking a single best predecessor.
    public static int CountByBitmaskMemo(int[] nums)
    {
        var fullMask = (1 << nums.Length) - 1;

        return (int)Memoizer.Memoize<(int Remaining, int Last), long>(
            (fullMask, NoPreviousElement),
            new PermutationCompletion(nums));
    }

    // A candidate is skipped when it is no longer unplaced, or when it cannot follow
    // the last placed element under the pair rule.
    private static bool ShouldSkipCandidate((int Remaining, int Last) state, int next, int[] nums) =>
        (state.Remaining & (1 << next)) == 0 || IsPairRuleBroken(state.Last, next, nums);

    // A candidate cannot follow the last placed element unless one of the two
    // divides the other; with nothing placed yet there is no pair to break.
    private static bool IsPairRuleBroken(int last, int next, int[] nums)
        => last != NoPreviousElement && !CanFollow(nums[last], nums[next]);

    // One node of the permutation tree: a complete ordering counts once when it is
    // special, a shorter one sums the counts below each element it can place next.
    private static int CountSpecialOrderings(PermutationState state, int[] nums)
    {
        if (state.Values.Count != nums.Length)
        {
            return CountByPlacingEachUnplaced(state, nums);
        }

        var isSpecial = IsSpecial(state.Values);

        return isSpecial ? 1 : 0;
    }

    // Every element still unplaced, in index order: place it, count below it, take it
    // back.
    private static int CountByPlacingEachUnplaced(PermutationState state, int[] nums)
    {
        var count = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (state.Used[i])
            {
                continue;
            }

            state.Used[i] = true;
            state.Values.Add(nums[i]);
            count += CountSpecialOrderings(state, nums);
            state.Used[i] = false;
            state.Values.RemoveAt(state.Values.Count - 1);
        }

        return count;
    }

    private static bool IsSpecial(List<int> permutation)
    {
        for (var i = 0; i < permutation.Count - 1; i++)
        {
            if (!CanFollow(permutation[i], permutation[i + 1]))
            {
                return false;
            }
        }

        return true;
    }

    // The adjacency rule itself, stated once for both arms: two elements may sit
    // next to each other exactly when one divides the other.
    private static bool CanFollow(int placed, int next) => placed % next == 0 || next % placed == 0;

    // How many legal completions follow a state, named: every still-unplaced element
    // that the pair rule allows may go next, and each one's own completions are added
    // in modulo-reduced. The elements are the whole of what the rule needs from its
    // caller, so they are the constructor's only input.
    private sealed class PermutationCompletion(int[] nums) : IRecurrence<(int Remaining, int Last), long>
    {
        public long Replay((int Remaining, int Last) state, IRecurrence<(int Remaining, int Last), long> rest)
        {
            var (remaining, _) = state;
            if (remaining == 0)
            {
                return 1L;
            }

            var total = 0L;

            for (var next = 0; next < nums.Length; next++)
            {
                if (ShouldSkipCandidate(state, next, nums))
                {
                    continue;
                }

                total = (total + rest.Replay((remaining & ~(1 << next), next), rest)) % ModularArithmetic.Modulo;
            }

            return total;
        }
    }

    private sealed record PermutationState
    {
        public bool[] Used { get; }
        public List<int> Values { get; } = [];

        public PermutationState(int length) => Used = new bool[length];
    }
}
