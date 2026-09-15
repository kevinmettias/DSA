using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FindTheMinimumCostArrayPermutation;

// LeetCode 3149. Find the Minimum Cost Array Permutation: nums is a permutation of
// 0..n-1; score(perm) sums |perm[i] - nums[perm[(i+1) mod n]]| around the whole
// cycle. Return the minimum-score permutation, lexicographically smallest among
// ties.
//
// The score is a sum over a cyclic arrangement of all n labels, so it is
// rotation-invariant - rotating any minimum-score permutation to start at the
// globally smallest label, 0, cannot raise its score and can only make the array
// lexicographically smaller (0 is <= every other entry). So the overall
// lexicographically smallest optimum always has perm[0] == 0, and searching only
// permutations that start there loses no candidate.
//
// With perm[0] fixed at 0, this is bitmask-TSP: g(mask, last) = the minimum cost to
// finish the tour, having already placed the nodes in mask with `last` placed most
// recently, walking every remaining node exactly once and closing back to 0. That
// is exactly the (Mask, Last) shape FindTheShortestSuperstring's own Hamiltonian-
// path DP uses over Memoizer (there maximizing overlap; here minimizing absolute
// difference), so the composed strategy below reuses this repo's own Memoizer
// rather than hand-rolling a second memoized recursion.
internal static class FindTheMinimumCostArrayPermutationSolution
{
    // Textbook baseline: generates every one of the n! permutations via BCL
    // swap-based backtracking and scores each directly against the problem's own
    // cyclic formula, keeping the lowest-scoring one and, on a tie, whichever is
    // lexicographically smaller - deliberately without the "start at 0" insight or
    // this repo's Memoizer. The arm the bitmask strategy below has to justify
    // itself against.
    public static int[] FindPermutationByBruteForceSearch(int[] nums)
    {
        var n = nums.Length;
        var perm = new int[n];

        for (var i = 0; i < n; i++)
        {
            perm[i] = i;
        }

        (int[]? Perm, long Score) best = (null, long.MaxValue);

        SearchPermutations(perm, 0, nums, ref best);

        return best.Perm!;
    }

    // This repo's own Memoizer over bitmask-TSP state (Mask, Last): g(mask, last)
    // is the minimum cost to complete the tour from `last`.
    public static int[] FindPermutationByBitmaskMemoization(int[] nums)
    {
        var n = nums.Length;
        var fullMask = (1 << n) - 1;

        return ReconstructPermutation(new TourCompletionCost(fullMask, nums), n, nums);
    }

    /// <summary>
    /// The recurrence, named: g(mask, last) is the minimum cost to finish the tour,
    /// walking every node still outside <c>mask</c> exactly once and closing back to 0.
    /// </summary>
    private sealed class TourCompletionCost(int fullMask, int[] nums) : IRecurrence<(int Mask, int Last), long>
    {
        /// <inheritdoc/>
        public long Replay((int Mask, int Last) state, IRecurrence<(int Mask, int Last), long> rest)
        {
            var (mask, last) = state;

            if (mask == fullMask)
            {
                return Math.Abs(last - nums[0]);
            }

            var best = long.MaxValue;

            for (var candidate = 1; candidate < nums.Length; candidate++)
            {
                if ((mask & (1 << candidate)) != 0)
                {
                    continue;
                }

                var cost = Math.Abs(last - nums[candidate]) + rest.Replay((mask | (1 << candidate), candidate), rest);
                best = Math.Min(best, cost);
            }

            return best;
        }
    }

    // Reconstruction walks forward from (mask = {0}, last = 0), at each step picking
    // the SMALLEST unvisited candidate whose edge cost plus its own completion cost
    // matches the current state's optimum - since g is exact, any candidate meeting
    // that equality keeps the tour globally optimal, and always preferring the
    // smallest such candidate is what makes the reconstructed permutation
    // lexicographically smallest among every permutation achieving the minimum score.
    private static int[] ReconstructPermutation(
        IRecurrence<(int Mask, int Last), long> completion, int n, int[] nums)
    {
        var permutation = new int[n];
        var visited = 1;
        var current = 0;

        for (var position = 1; position < n; position++)
        {
            var candidate = SmallestCandidateAchieving((visited, current), completion, n, nums);

            if (candidate is null)
            {
                continue;
            }

            permutation[position] = candidate.Value;
            visited |= 1 << candidate.Value;
            current = candidate.Value;
        }

        return permutation;
    }

    // The smallest unvisited candidate whose edge cost plus its own completion cost
    // equals the state's own optimum, or null when no candidate does.
    private static int? SmallestCandidateAchieving(
        (int Mask, int Last) state, IRecurrence<(int Mask, int Last), long> completion, int n, int[] nums)
    {
        var (mask, last) = state;
        var target = Memoizer.Memoize(state, completion);

        for (var candidate = 1; candidate < n; candidate++)
        {
            if ((mask & (1 << candidate)) != 0)
            {
                continue;
            }

            var edgeCost = Math.Abs(last - nums[candidate]);
            var candidateCompletion = Memoizer.Memoize((mask | (1 << candidate), candidate), completion);

            if (edgeCost + candidateCompletion == target)
            {
                return candidate;
            }
        }

        return null;
    }

    private static void SearchPermutations(
        int[] perm, int start, int[] nums, ref (int[]? Perm, long Score) best)
    {
        if (start == perm.Length)
        {
            var score = Score(perm, nums);

            if (BeatsBestSoFar(score, best.Score, perm, best.Perm!))
            {
                best = ((int[])perm.Clone(), score);
            }

            return;
        }

        for (var i = start; i < perm.Length; i++)
        {
            (perm[start], perm[i]) = (perm[i], perm[start]);
            SearchPermutations(perm, start + 1, nums, ref best);
            (perm[start], perm[i]) = (perm[i], perm[start]);
        }
    }

    // A candidate permutation beats the best one found so far when it scores lower, or
    // ties on score and reads smaller.
    private static bool BeatsBestSoFar(long score, long bestScore, int[] candidate, int[] best) =>
        score < bestScore || (score == bestScore && IsLexicographicallySmaller(candidate, best));

    private static long Score(int[] perm, int[] nums)
    {
        var total = 0L;

        for (var i = 0; i < perm.Length; i++)
        {
            var next = perm[(i + 1) % perm.Length];
            total += Math.Abs(perm[i] - nums[next]);
        }

        return total;
    }

    private static bool IsLexicographicallySmaller(int[] candidate, int[] current)
    {
        for (var i = 0; i < candidate.Length; i++)
        {
            if (candidate[i] != current[i])
            {
                return candidate[i] < current[i];
            }
        }

        return false;
    }
}
