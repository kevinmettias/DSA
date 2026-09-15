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

        return MeetingIndexFrom(heights, lo, hi);
    }

    // One already-normalized query's own answer: lo when the pair is met, hi when a
    // single hop reaches it, and otherwise the first later building that beats them both.
    private static int MeetingIndexFrom(int[] heights, int lo, int hi)
    {
        if (lo == hi)
        {
            return lo;
        }

        if (heights[lo] < heights[hi])
        {
            return hi;
        }

        return FirstBuildingTallerThan(heights, hi, lo);
    }

    // The forward scan: the first index after `afterIndex` whose height beats
    // heights[floorIndex], or the sentinel when the array runs out first.
    private static int FirstBuildingTallerThan(int[] heights, int afterIndex, int floorIndex)
    {
        for (var j = afterIndex + 1; j < heights.Length; j++)
        {
            if (heights[j] > heights[floorIndex])
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
        var pendingByHi = PendingListsByHi(heights.Length);

        for (var q = 0; q < queries.Length; q++)
        {
            var lo = Math.Min(queries[q][0], queries[q][1]);
            var hi = Math.Max(queries[q][0], queries[q][1]);
            var (answer, needsLaterBuilding) = ClassifyQuery(heights, lo, hi);

            answers[q] = answer;

            if (needsLaterBuilding)
            {
                pendingByHi[hi].Add((heights[lo], q));
            }
        }

        return (answers, pendingByHi);
    }

    // One empty pending bucket per building, each holding the queries bucketed at that
    // index - the sweep's queue of work still to do when it reaches that index.
    private static List<(int Threshold, int QueryIndex)>[] PendingListsByHi(int buildingCount)
    {
        var pendingByHi = new List<(int Threshold, int QueryIndex)>[buildingCount];

        for (var i = 0; i < buildingCount; i++)
        {
            pendingByHi[i] = [];
        }

        return pendingByHi;
    }

    // One query's own resolution: lo when the pair is already met, hi when a single hop
    // reaches it, and the sentinel plus "bucket it at hi" when only a later building
    // taller than heights[lo] can answer it.
    private static (int Answer, bool NeedsLaterBuilding) ClassifyQuery(int[] heights, int lo, int hi)
    {
        if (lo == hi)
        {
            return (lo, false);
        }

        if (heights[lo] < heights[hi])
        {
            return (hi, false);
        }

        return (LeetCodeAnswer.None, true);
    }
}
