using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

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

    // Enumerate every one of the n! full permutations with this repo's own
    // Backtrack.Search - the same choose/candidates/unchoose shape
    // PermutationsSolution uses - and check the adjacency rule only after a
    // complete ordering has been built. O(n! * n), the arm the bitmask DP has
    // to beat. The count is reported unreduced: this arm is only viable at
    // lengths where n! cannot come close to the modulus.
    public static int CountByBruteForceBacktracking(int[] nums)
    {
        var count = 0;
        var state = new PermutationState(nums.Length);

        Backtrack.Search<PermutationState, int>(
            state,
            s => s.Values.Count == nums.Length,
            s => s.Values.Count == nums.Length
                ? []
                : Enumerable.Range(0, nums.Length).Where(i => !s.Used[i]),
            (s, i) => { s.Used[i] = true; s.Values.Add(nums[i]); },
            (s, i) => { s.Used[i] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            s =>
            {
                if (IsSpecial(s.Values))
                {
                    count++;
                }
            });

        return count;
    }

    private static bool IsSpecial(List<int> permutation)
    {
        for (var i = 0; i < permutation.Count - 1; i++)
        {
            var (a, b) = (permutation[i], permutation[i + 1]);
            if (a % b != 0 && b % a != 0)
            {
                return false;
            }
        }

        return true;
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

        long Recurrence((int Remaining, int Last) state, Func<(int Remaining, int Last), long> ways)
        {
            var (remaining, last) = state;
            if (remaining == 0)
            {
                return 1L;
            }

            var total = 0L;

            for (var next = 0; next < nums.Length; next++)
            {
                if ((remaining & (1 << next)) == 0)
                {
                    continue;
                }

                if (last != NoPreviousElement &&
                    nums[last] % nums[next] != 0 &&
                    nums[next] % nums[last] != 0)
                {
                    continue;
                }

                total = (total + ways((remaining & ~(1 << next), next))) % ModularArithmetic.Modulo;
            }

            return total;
        }

        return (int)Memoizer.Memoize<(int Remaining, int Last), long>(
            (fullMask, NoPreviousElement), Recurrence);
    }

    private sealed class PermutationState(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }
}
