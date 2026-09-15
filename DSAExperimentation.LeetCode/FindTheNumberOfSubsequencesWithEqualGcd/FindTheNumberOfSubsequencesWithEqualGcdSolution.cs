using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheNumberOfSubsequencesWithEqualGcd;

// LeetCode 3336. Find the Number of Subsequences With Equal GCD: count ordered
// pairs of disjoint, non-empty subsequences (seq1, seq2) of nums whose element
// GCDs are equal, modulo 1e9+7.
//
// Both strategies process nums left to right, deciding for each element whether it
// joins seq1, joins seq2, or joins neither - a state machine over
// (index, gcd-of-seq1-so-far, gcd-of-seq2-so-far), where 0 stands for "still
// empty" (Euclid's own gcd(0, x) = x convention, so the first element added to a
// sequence just becomes its running gcd). The brute-force arm walks that exact
// 3-way choice tree with plain recursion and no cache - genuinely 3^n calls, the
// textbook reading of the problem (CountTheNumberOfSquareFreeSubsets's own
// CountByBruteForce precedent for "the exponential arm the memo has to beat"). The
// composed arm is the identical recurrence run through this repo's
// Algorithms.DynamicProgramming.Memoizer, collapsing the shared
// (index, gcd1, gcd2) states to at most n * maxValue^2 distinct calls, reporting
// through Domain.Modular.ModularArithmetic's shared 1e9+7 convention
// (CountTheNumberOfSquareFreeSubsets's own CountByBitmaskMemo precedent for this
// exact Memoizer-plus-ModularArithmetic composition).
internal static class FindTheNumberOfSubsequencesWithEqualGcdSolution
{
    public static int CountPairsByBruteForce(int[] nums)
    {
        var count = CountFrom(0, 0, 0, nums);

        return (int)(count % ModularArithmetic.Modulo);
    }

    public static int CountPairsByGcdMemoization(int[] nums)
    {
        var count = Memoizer.Memoize<(int Index, int Gcd1, int Gcd2), long>((0, 0, 0), new GcdPairPrefix(nums));

        return (int)count;
    }

    /// <summary>
    /// The recurrence, named: walking nums left to right, each element joins the first
    /// subsequence, the second, or neither, and a state is an answer once every element
    /// has been decided and the two running GCDs are equal and neither is still empty.
    /// </summary>
    private sealed class GcdPairPrefix(int[] nums) : IRecurrence<(int Index, int Gcd1, int Gcd2), long>
    {
        /// <inheritdoc/>
        public long Replay((int Index, int Gcd1, int Gcd2) state, IRecurrence<(int Index, int Gcd1, int Gcd2), long> rest)
        {
            if (state.Index == nums.Length)
            {
                return state.Gcd1 != 0 && state.Gcd1 == state.Gcd2 ? 1 : 0;
            }

            var skip = rest.Replay((state.Index + 1, state.Gcd1, state.Gcd2), rest);
            var addToFirst = rest.Replay((state.Index + 1, Gcd(state.Gcd1, nums[state.Index]), state.Gcd2), rest);
            var addToSecond = rest.Replay((state.Index + 1, state.Gcd1, Gcd(state.Gcd2, nums[state.Index])), rest);

            return (skip + addToFirst + addToSecond) % ModularArithmetic.Modulo;
        }
    }

    private static long CountFrom(int index, int gcd1, int gcd2, int[] nums)
    {
        if (index == nums.Length)
        {
            return gcd1 != 0 && gcd1 == gcd2 ? 1 : 0;
        }

        var skip = CountFrom(index + 1, gcd1, gcd2, nums);
        var gcdWithFirst = Gcd(gcd1, nums[index]);
        var addToFirst = CountFrom(index + 1, gcdWithFirst, gcd2, nums);
        var gcdWithSecond = Gcd(gcd2, nums[index]);
        var addToSecond = CountFrom(index + 1, gcd1, gcdWithSecond, nums);

        return skip + addToFirst + addToSecond;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
