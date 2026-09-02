using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

// nums has even length. Two players, starting with player 1, alternate turns:
// each turn, mark the smallest still-unmarked value (ties broken by the smallest
// index), plus its two array neighbors if they exist and are still unmarked, and
// add the CHOSEN value (not the neighbors') to the current player's score. Every
// element ends up marked - the smallest-value rule is a fixed simulation, not a
// choice either player makes - so the answer is simply player1Score minus
// player2Score.
internal static class FindTheScoreDifferenceInAGameSolution
{
    private const int PlayerCount = 2;

    // The textbook answer: a fresh O(n) scan for the smallest unmarked value on
    // every turn, O(n^2) overall - the arm the min-heap strategy below has to
    // beat.
    public static int ScoreDifferenceByLinearScan(int[] nums)
    {
        var marked = new bool[nums.Length];
        var scores = new int[PlayerCount];
        var turn = 0;
        var remaining = nums.Length;

        while (remaining > 0)
        {
            var index = SmallestUnmarkedIndex(nums, marked);
            remaining -= MarkAndCountNewlyMarked(marked, index);
            scores[turn] += nums[index];
            turn = 1 - turn;
        }

        return scores[0] - scores[1];
    }

    private static int SmallestUnmarkedIndex(int[] nums, bool[] marked)
    {
        var best = -1;

        for (var i = 0; i < nums.Length; i++)
        {
            if (!marked[i] && (best == -1 || nums[i] < nums[best]))
            {
                best = i;
            }
        }

        return best;
    }

    private static int MarkAndCountNewlyMarked(bool[] marked, int index) =>
        Mark(marked, index) + Mark(marked, index - 1) + Mark(marked, index + 1);

    private static int Mark(bool[] marked, int index)
    {
        if (index < 0 || index >= marked.Length || marked[index])
        {
            return 0;
        }

        marked[index] = true;
        return 1;
    }

    // Composed: push every (value, index) pair into this repo's Heap once -
    // MinHeapOrder needs nothing beyond ValueTuple's own built-in lexicographic
    // IComparable, which already orders by Value first and Index second, exactly
    // this game's own tie-break rule - then repeatedly pop the true global
    // minimum. A popped entry whose index is already marked is stale (its value
    // was the smallest UNMARKED at push time, not necessarily at pop time) and
    // simply discarded; every entry is pushed once and popped at most once, so
    // this is the standard lazy-deletion heap pattern PowerGridMaintenanceSolution
    // also uses, O(n log n) overall against the scan's O(n^2). The heap is built
    // fresh inside this method rather than hoisted to a prepared-input overload:
    // TryPop drains it, so a shared instance could not survive a second benchmark
    // iteration the way a read-only prepared input can.
    public static int ScoreDifferenceByMinHeap(int[] nums)
    {
        var heap = new Heap<(int Value, int Index), MinHeapOrder<(int Value, int Index)>>();

        for (var i = 0; i < nums.Length; i++)
        {
            heap.Push((nums[i], i));
        }

        var marked = new bool[nums.Length];
        var scores = new int[PlayerCount];
        var turn = 0;

        while (heap.TryPop(out var entry))
        {
            if (marked[entry.Index])
            {
                continue;
            }

            MarkAndCountNewlyMarked(marked, entry.Index);
            scores[turn] += entry.Value;
            turn = 1 - turn;
        }

        return scores[0] - scores[1];
    }
}
