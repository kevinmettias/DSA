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

        int[]? best = null;
        var bestScore = long.MaxValue;

        SearchPermutations(perm, 0, nums, ref best, ref bestScore);

        return best!;
    }

    private static void SearchPermutations(int[] perm, int start, int[] nums, ref int[]? best, ref long bestScore)
    {
        if (start == perm.Length)
        {
            var score = Score(perm, nums);

            if (score < bestScore || (score == bestScore && IsLexicographicallySmaller(perm, best!)))
            {
                bestScore = score;
                best = (int[])perm.Clone();
            }

            return;
        }

        for (var i = start; i < perm.Length; i++)
        {
            (perm[start], perm[i]) = (perm[i], perm[start]);
            SearchPermutations(perm, start + 1, nums, ref best, ref bestScore);
            (perm[start], perm[i]) = (perm[i], perm[start]);
        }
    }

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

    // This repo's own Memoizer over bitmask-TSP state (Mask, Last): g(mask, last)
    // is the minimum cost to complete the tour from `last`. Reconstruction walks
    // forward from (mask = {0}, last = 0), at each step picking the SMALLEST
    // unvisited candidate whose edge cost plus its own completion cost matches the
    // current state's optimum - since g is exact, any candidate meeting that
    // equality keeps the tour globally optimal, and always preferring the smallest
    // such candidate is what makes the reconstructed permutation lexicographically
    // smallest among every permutation achieving the minimum score.
    public static int[] FindPermutationByBitmaskMemoization(int[] nums)
    {
        var n = nums.Length;
        var fullMask = (1 << n) - 1;

        long CompletionCost((int Mask, int Last) state) =>
            Memoizer.Memoize<(int Mask, int Last), long>(state, (s, best) => Recurrence(s, best, n, fullMask, nums));

        var permutation = new int[n];
        var visited = 1;
        var current = 0;

        for (var position = 1; position < n; position++)
        {
            var target = CompletionCost((visited, current));

            for (var candidate = 1; candidate < n; candidate++)
            {
                if ((visited & (1 << candidate)) != 0)
                {
                    continue;
                }

                var edgeCost = Math.Abs(current - nums[candidate]);
                var completion = CompletionCost((visited | (1 << candidate), candidate));

                if (edgeCost + completion == target)
                {
                    permutation[position] = candidate;
                    visited |= 1 << candidate;
                    current = candidate;
                    break;
                }
            }
        }

        return permutation;
    }

    private static long Recurrence(
        (int Mask, int Last) state, Func<(int Mask, int Last), long> completionCost, int n, int fullMask, int[] nums)
    {
        var (mask, last) = state;

        if (mask == fullMask)
        {
            return Math.Abs(last - nums[0]);
        }

        var best = long.MaxValue;

        for (var candidate = 1; candidate < n; candidate++)
        {
            if ((mask & (1 << candidate)) != 0)
            {
                continue;
            }

            var cost = Math.Abs(last - nums[candidate]) + completionCost((mask | (1 << candidate), candidate));
            best = Math.Min(best, cost);
        }

        return best;
    }
}
