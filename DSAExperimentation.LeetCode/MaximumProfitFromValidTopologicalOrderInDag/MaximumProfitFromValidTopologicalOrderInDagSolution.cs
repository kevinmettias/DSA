using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumProfitFromValidTopologicalOrderInDag;

// LeetCode 3530. Maximum Profit from Valid Topological Order in DAG: assign each
// node a 1-based position in SOME topological order of the DAG so that
// sum(score[node] * position) is maximised. The node count is at most 22, so
// "which nodes have already been placed" fits in one int bitmask - the same Held-Karp-shaped
// state FindTheMinimumCostArrayPermutation's own bitmask DP already tracks over
// this repo's Memoizer, just without a "last node" half: profit only depends on
// WHICH nodes are placed and how many, never on the order among them beyond that
// count.
internal static class MaximumProfitFromValidTopologicalOrderInDagSolution
{
    // Textbook baseline: recursive backtracking that, at every position, tries
    // every not-yet-placed node whose prerequisites are already satisfied and
    // keeps the best total - the same search tree the composed strategy prunes
    // by memoizing, just walked here with no cache, so a mask reachable through
    // more than one ordering is recomputed from scratch every time it's reached.
    public static long MaxProfitByBacktracking(int nodeCount, int[][] edges, int[] score)
    {
        var predecessorMasks = BuildPredecessorMasks(nodeCount, edges);

        return SearchBestProfit(0, 0, predecessorMasks, score);
    }

    private static long SearchBestProfit(int mask, int placed, int[] predecessorMasks, int[] score)
    {
        if (placed == score.Length)
        {
            return 0;
        }

        var best = long.MinValue;

        for (var node = 0; node < score.Length; node++)
        {
            if (!CanPlace(mask, node, predecessorMasks))
            {
                continue;
            }

            var profit = ((long)score[node] * (placed + 1))
                + SearchBestProfit(mask | (1 << node), placed + 1, predecessorMasks, score);
            best = Math.Max(best, profit);
        }

        return best;
    }

    private static int[] BuildPredecessorMasks(int nodeCount, int[][] edges)
    {
        var predecessorMasks = new int[nodeCount];

        foreach (var edge in edges)
        {
            predecessorMasks[edge[1]] |= 1 << edge[0];
        }

        return predecessorMasks;
    }

    // Composed: this repo's own Memoizer over one bitmask state - "profit still
    // achievable by optimally placing every node NOT in mask" - so every mask
    // reachable from more than one ordering is solved once, not once per
    // ordering that reaches it.
    public static long MaxProfitByBitmaskMemoization(int nodeCount, int[][] edges, int[] score)
    {
        var predecessors = PrecedenceMasks.Build(nodeCount, edges);
        return MaxProfitByBitmaskMemoization(predecessors, score);
    }

    public static long MaxProfitByBitmaskMemoization(PrecedenceMasks predecessors, int[] score)
        => Memoizer.Memoize<int, long>(0, new ProfitFromMask(predecessors.Masks, score));

    // `node` is placeable once every node its predecessorMasks bit names is already
    // in mask: not yet placed itself, and no required predecessor missing.
    private static bool CanPlace(int mask, int node, int[] predecessorMasks)
        => (mask & (1 << node)) == 0 && (predecessorMasks[node] & ~mask) == 0;

    // The placement rule, named: the profit still achievable by optimally placing every
    // node the mask does not yet hold is the best over every placeable node of its score
    // times the position that node would take, plus what the rule reports for the mask that
    // placement leaves behind. The precedence masks and scores are fixed for the whole
    // search and arrive once through the primary constructor; `rest` is the memo run's own
    // handle on this rule.
    private sealed class ProfitFromMask(int[] predecessorMasks, int[] score) : IRecurrence<int, long>
    {
        /// <inheritdoc/>
        public long Replay(int mask, IRecurrence<int, long> rest)
        {
            var placed = BitOperations.PopCount((uint)mask);

            if (placed == score.Length)
            {
                return 0;
            }

            var best = long.MinValue;

            for (var node = 0; node < score.Length; node++)
            {
                if (!CanPlace(mask, node, predecessorMasks))
                {
                    continue;
                }

                var profit = ((long)score[node] * (placed + 1)) + rest.Replay(mask | (1 << node), rest);
                best = Math.Max(best, profit);
            }

            return best;
        }
    }
}
