using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindBuildingWhereAliceAndBobCanMeet;

// LeetCode 2940. Find Building Where Alice and Bob Can Meet: from building i you
// may only move to a later, taller building j (i < j, heights[i] < heights[j]).
// Normalizing a query to lo = min(a, b), hi = max(a, b): lo == hi needs no move
// at all; heights[lo] < heights[hi] lets the person at lo jump straight to hi in
// one hop; otherwise both need the first index j > hi whose height beats
// heights[lo] (already >= heights[hi] in this branch, so beating it beats
// heights[hi] too).
internal static class FindBuildingWhereAliceAndBobCanMeetSolution
{
    // Per query, scan forward from hi for the first building taller than
    // heights[lo]. The arm the offline heap sweep below has to beat: O(n) worst
    // case per query, O(n * queries.Length) overall.
    public static int[] FindMeetingBuildingsByBruteForce(int[] heights, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            answers[q] = FindMeetingBuildingByBruteForce(heights, queries[q][0], queries[q][1]);
        }

        return answers;
    }

    private static int FindMeetingBuildingByBruteForce(int[] heights, int a, int b)
    {
        var lo = Math.Min(a, b);
        var hi = Math.Max(a, b);

        if (lo == hi)
        {
            return lo;
        }

        if (heights[lo] < heights[hi])
        {
            return hi;
        }

        for (var j = hi + 1; j < heights.Length; j++)
        {
            if (heights[j] > heights[lo])
            {
                return j;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Every query that needs a forward search is bucketed at its own hi index and
    // answered offline in one left-to-right sweep over the buildings: a min-heap
    // (DataStructures/Heap/Heap.cs with MinHeapOrder) holds every still-
    // unanswered query keyed by the height it needs beaten, and at each index j
    // every entry whose threshold that index clears is popped and answered there
    // - the smallest such j is found because the sweep itself visits indices in
    // increasing order, so nothing is ever re-checked once it has been the
    // current j. O((n + queries.Length) log queries.Length) overall.
    public static int[] FindMeetingBuildingsByOfflineHeapSweep(int[] heights, int[][] queries)
    {
        var (answers, pendingByHi) = ClassifyQueries(heights, queries);
        var heap = new Heap<(int Threshold, int QueryIndex), MinHeapOrder<(int, int)>>();

        for (var j = 0; j < heights.Length; j++)
        {
            while (heap.TryPeek(out var top) && top.Threshold < heights[j])
            {
                heap.TryPop(out top);
                answers[top.QueryIndex] = j;
            }

            foreach (var pending in pendingByHi[j])
            {
                heap.Push(pending);
            }
        }

        return answers;
    }

    // Resolves every trivial (lo == hi) or one-hop (heights[lo] < heights[hi])
    // query immediately; every other query is bucketed at index hi, keyed by the
    // height (heights[lo]) it needs a later building to beat.
    private static (int[] Answers, List<(int Threshold, int QueryIndex)>[] PendingByHi) ClassifyQueries(
        int[] heights, int[][] queries)
    {
        var answers = new int[queries.Length];
        var pendingByHi = new List<(int Threshold, int QueryIndex)>[heights.Length];

        for (var i = 0; i < pendingByHi.Length; i++)
        {
            pendingByHi[i] = [];
        }

        for (var q = 0; q < queries.Length; q++)
        {
            var lo = Math.Min(queries[q][0], queries[q][1]);
            var hi = Math.Max(queries[q][0], queries[q][1]);

            if (lo == hi)
            {
                answers[q] = lo;
            }
            else if (heights[lo] < heights[hi])
            {
                answers[q] = hi;
            }
            else
            {
                answers[q] = LeetCodeAnswer.None;
                pendingByHi[hi].Add((heights[lo], q));
            }
        }

        return (answers, pendingByHi);
    }
}
