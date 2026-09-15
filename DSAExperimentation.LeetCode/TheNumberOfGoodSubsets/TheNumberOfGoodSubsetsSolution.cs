using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.TheNumberOfGoodSubsets;

// LeetCode 1994. The Number of Good Subsets: count the non-empty subsets of nums whose
// product is a product of DISTINCT primes, modulo 1e9+7. Subsets are distinguished by
// the indices chosen, so equal values count separately.
//
// GoodSubsetCandidates reduces nums to the (prime mask, occurrence count) pairs of the
// squarefree values in [2, 30]; from there both strategies are the same recursion over
// the state (next candidate index, primes already used) - skip the candidate, or take
// it once per occurrence when its mask does not overlap what is already used, bottoming
// out at 1 for any non-empty accumulated mask. They differ only in whether that state is
// remembered: the brute force re-explores every (index, mask) pair from scratch down
// every branch, since many take/skip orderings over the ~18 candidates reach the
// identical state, while the composed strategy routes the identical recursion through
// this repo's own Memoizer - the same (int, int) tuple-state shape LC 1947's
// (student, mask) recursion uses - collapsing it to one visit per reachable state.
//
// The doubling for 1s is shared framing rather than part of either strategy: 1 has no
// prime factors, so each occurrence independently doubles whatever the recursion counted.
internal static class TheNumberOfGoodSubsetsSolution
{
    // Each 1 in nums may be in or out of any good subset independently.
    private const int OnesDoublingBase = 2;

    private const int NoPrimesUsed = 0;
    private const int FirstCandidate = 0;

    // The textbook answer: plain recursion over (index, usedMask) with no cache at all,
    // deliberately written without this repo's primitives - it is the arm the memoized
    // strategy below has to justify itself against.
    public static int NumberOfGoodSubsetsByBruteForceRecursion(int[] nums) =>
        NumberOfGoodSubsetsByBruteForceRecursion(GoodSubsetCandidates.Build(nums));

    public static int NumberOfGoodSubsetsByBruteForceRecursion(GoodSubsetCandidates candidates)
    {
        var waysWithoutOnes = WaysFrom(candidates, FirstCandidate, NoPrimesUsed);
        return WithOnesDoubling(waysWithoutOnes, candidates.OnesCount);
    }

    private static long WaysFrom(GoodSubsetCandidates candidates, int index, int usedMask)
    {
        if (index == candidates.Count)
        {
            return usedMask != NoPrimesUsed ? 1L : 0L;
        }

        var (mask, weight) = candidates.At(index);
        var skip = WaysFrom(candidates, index + 1, usedMask);

        if ((usedMask & mask) != 0 || weight == 0)
        {
            return skip;
        }

        var take = weight * WaysFrom(candidates, index + 1, usedMask | mask) % ModularArithmetic.Modulo;

        return (skip + take) % ModularArithmetic.Modulo;
    }

    // The same recursion, with Memoizer owning the (index, usedMask) cache: at most one
    // evaluation per reachable state instead of one per distinct take/skip ordering.
    public static int NumberOfGoodSubsetsByMemoizedRecursion(int[] nums) =>
        NumberOfGoodSubsetsByMemoizedRecursion(GoodSubsetCandidates.Build(nums));

    public static int NumberOfGoodSubsetsByMemoizedRecursion(GoodSubsetCandidates candidates) =>
        WithOnesDoubling(MemoizedWays(candidates), candidates.OnesCount);

    private static long MemoizedWays(GoodSubsetCandidates candidates) =>
        Memoizer.Memoize<(int Index, int UsedMask), long>(
            (FirstCandidate, NoPrimesUsed),
            new WaysOverCandidates(candidates));

    // The recurrence, as a named type: skip-or-take over the squarefree candidates for
    // one (index, usedMask) state. The candidate list it reads arrives through the
    // primary constructor and the memoized continuation through `rest`, so the recursive
    // call-back is a method on a named type rather than an anonymous delegate.
    private sealed class WaysOverCandidates(GoodSubsetCandidates candidates)
        : IRecurrence<(int Index, int UsedMask), long>
    {
        public long Replay(
            (int Index, int UsedMask) state, IRecurrence<(int Index, int UsedMask), long> rest)
        {
            var (index, usedMask) = state;

            if (index == candidates.Count)
            {
                return usedMask != NoPrimesUsed ? 1L : 0L;
            }

            var (mask, weight) = candidates.At(index);
            var skip = rest.Replay((index + 1, usedMask), rest);

            if ((usedMask & mask) != 0 || weight == 0)
            {
                return skip;
            }

            var take = weight * rest.Replay((index + 1, usedMask | mask), rest)
                % ModularArithmetic.Modulo;

            return (skip + take) % ModularArithmetic.Modulo;
        }
    }

    // 2^onesCount, via the 1e9+7 modular exponentiation this repo already declares once
    // - shared by both arms, since the choice being measured is the recursion, not this.
    private static int WithOnesDoubling(long waysWithNonEmptyMask, int onesCount) =>
        (int)(waysWithNonEmptyMask
            * ModularArithmetic.Power(OnesDoublingBase, onesCount)
            % ModularArithmetic.Modulo);
}
