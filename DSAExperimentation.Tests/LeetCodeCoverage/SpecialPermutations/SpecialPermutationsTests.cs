using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpecialPermutations;

// LeetCode 2741. Special Permutations: a permutation is special when every adjacent
// pair divides one another (either direction). CountByBruteForceBacktracking enumerates
// every one of the n! full permutations via this repo's own Backtrack.Search (the same
// choose/candidates/unchoose shape PermutationsTests already proves out) and checks the
// adjacency rule only once a full ordering exists - the O(n! * n) arm the bitmask DP has
// to beat. CountByBitmaskMemo instead threads (Remaining, Last) - "these elements are
// still unplaced, Last is the element most recently placed before them" - through this
// repo's own Memoizer: Last = -1 seeds the single empty-prefix start state (any element
// may go first), and each candidate only extends the running total when it's compatible
// with Last, so one Memoize call sums every valid completion of every valid first choice
// - the same (Mask, Last) TSP-style state FindTheShortestSuperstringTests already
// establishes for this repo's bitmask DP over permutations, here summing completions
// instead of tracking a single best predecessor.
public sealed class SpecialPermutationsTests
{
    private const int Modulo = 1_000_000_007;

    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 3, 6], 2 }, // [3,6,2] and [2,6,3]
            { [1, 4, 3], 2 }, // [3,1,4] and [4,1,3] - 1 divides everything either way
            { [2, 4], 2 }, // [2,4] and [4,2] both valid: 4 % 2 == 0 regardless of order
            { [2, 3], 0 }, // neither divides the other, in either order
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForceBacktracking_LeetCodeExamples_ReturnsSpecialPermutationCount(
        int[] nums, int expected)
        => Assert.Equal(expected, CountByBruteForceBacktracking(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBitmaskMemo_LeetCodeExamples_ReturnsSpecialPermutationCount(int[] nums, int expected)
        => Assert.Equal(expected, (int)CountByBitmaskMemo(nums));

    private static int CountByBruteForceBacktracking(int[] nums)
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

    // Single Memoizer call over state (Remaining, Last): Remaining is the set of
    // elements not yet placed, Last is the element most recently placed (-1 for the
    // empty prefix, unconstrained). Base case Remaining == 0 is the empty completion -
    // one way to place nothing more. Every reachable state is visited at most once,
    // so the whole call tree is O(n * 2^n) regardless of how many of the n!
    // orderings are actually legal.
    private static long CountByBitmaskMemo(int[] nums)
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

                if (last != -1 && nums[last] % nums[next] != 0 && nums[next] % nums[last] != 0)
                {
                    continue;
                }

                total = (total + ways((remaining & ~(1 << next), next))) % Modulo;
            }

            return total;
        }

        return Memoizer.Memoize<(int Remaining, int Last), long>((fullMask, -1), Recurrence);
    }

    private sealed class PermutationState(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }
}
