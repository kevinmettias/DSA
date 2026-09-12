using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.IPO;

// LeetCode 502. IPO: pick up to k projects, greedily maximizing final capital,
// where a project can only be started once its required capital is affordable.
//
// The composed strategy is the classic two-heap greedy: a min-heap of projects
// ordered by required capital (this repo's Heap<(int Node, int Priority),
// ByPriorityOrder<int,int>>, the same (node, priority) projection used for
// Dijkstra/A* frontiers, here projecting (profit, capital) instead of
// (node, distance)) unlocks newly-affordable projects into a max-heap of their
// profits (Heap<int, MaxHeapOrder<int>>). Each of the k rounds moves every
// now-affordable project's profit into the max-heap, then greedily takes the
// best one, growing capital for the next round. The baseline instead rescans
// every remaining project each round for the best affordable one.
internal static class IPOSolution
{
    // The textbook answer: an O(k*n) linear rescan of every remaining project
    // each round, deliberately written without this repo's primitives - it is
    // the arm the composed two-heap greedy below has to justify itself against.
    public static int FindMaximizedCapitalByLinearScan(int k, int w, int[] profits, int[] capitals)
    {
        var used = new bool[profits.Length];

        for (var round = 0; round < k; round++)
        {
            var bestIndex = FindBestAffordableIndex(used, w, profits, capitals);

            if (bestIndex == -1)
            {
                break;
            }

            used[bestIndex] = true;
            w += profits[bestIndex];
        }

        return w;
    }

    private static int FindBestAffordableIndex(bool[] used, int capital, int[] profits, int[] capitals)
    {
        var bestIndex = -1;

        for (var i = 0; i < profits.Length; i++)
        {
            if (!used[i] && capitals[i] <= capital && (bestIndex == -1 || profits[i] > profits[bestIndex]))
            {
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    // This repo's own two-heap greedy: a min-heap by required capital feeding a
    // max-heap of unlocked profits.
    public static int FindMaximizedCapitalByTwoHeapGreedy(int k, int w, int[] profits, int[] capitals)
    {
        var byCapital = BuildCapitalHeap(profits, capitals);
        var byProfit = new Heap<int, MaxHeapOrder<int>>();

        for (var round = 0; round < k; round++)
        {
            UnlockAffordableProjects(byCapital, byProfit, w);

            if (!byProfit.TryPop(out var bestProfit))
            {
                break;
            }

            w += bestProfit;
        }

        return w;
    }

    private static Heap<(int Node, int Priority), ByPriorityOrder<int, int>> BuildCapitalHeap(int[] profits, int[] capitals)
    {
        var byCapital = new Heap<(int Node, int Priority), ByPriorityOrder<int, int>>();

        for (var i = 0; i < profits.Length; i++)
        {
            byCapital.Push((profits[i], capitals[i]));
        }

        return byCapital;
    }

    private static void UnlockAffordableProjects(
        Heap<(int Node, int Priority), ByPriorityOrder<int, int>> byCapital,
        Heap<int, MaxHeapOrder<int>> byProfit,
        int capital)
    {
        while (byCapital.TryPeek(out var cheapest) && cheapest.Priority <= capital)
        {
            byCapital.TryPop(out var popped);
            byProfit.Push(popped.Node);
        }
    }
}
