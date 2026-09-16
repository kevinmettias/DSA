namespace DSAExperimentation.LeetCode.MaximizeActiveSectionWithTradeII;

// LeetCode 3501. Maximize Active Section with Trade II: for each query
// [l, r], report the most '1's text could have after at most one trade - zero
// out one block of '1's surrounded by '0's inside "1" + text[l..r] + "1", then
// set one block of '0's surrounded by '1's (in the string that leaves
// behind) to '1' - with everything outside [l, r] left untouched, and both
// steps mandatory (no trade at all, i.e. 0 gain, is always the fallback).
internal static class MaximizeActiveSectionWithTradeIISolution
{
    // The textbook answer: for every query, literally run-length-encode the
    // augmented window from scratch and scan its runs for the best trade -
    // deliberately without any precomputed structure, the arm the
    // range-max-index strategy below has to beat.
    public static int[] MaxActiveAfterTradeByRunScan(string text, int[][] queries)
    {
        var activeOnes = text.Count(c => c == '1');
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = activeOnes + BestGain(text, queries[i][0], queries[i][1]);
        }

        return answers;
    }

    // The best gain a trade confined to text[left..right] can achieve: the
    // largest (leftNeighbor + rightNeighbor) zero-run pair around any
    // interior one-run of "1" + text[left..right] + "1", or 0 if none exists.
    private static int BestGain(string text, int left, int right)
    {
        var runs = RunLengthEncodeAugmented(text, left, right);
        var bestGain = 0;

        for (var i = 1; i < runs.Count - 1; i++)
        {
            if (runs[i].Kind == '1')
            {
                bestGain = Math.Max(bestGain, runs[i - 1].Length + runs[i + 1].Length);
            }
        }

        return bestGain;
    }

    // Maximal runs of "1" + text[left..right] + "1", built as one left-to-right
    // pass so the artificial boundary '1's merge into a real leading/trailing
    // one-run exactly like any other adjacent equal characters would.
    private static List<(char Kind, int Length)> RunLengthEncodeAugmented(string text, int left, int right)
    {
        var runs = new List<(char Kind, int Length)> { ('1', 1) };

        for (var i = left; i <= right; i++)
        {
            Extend(runs, text[i]);
        }

        Extend(runs, '1');
        return runs;
    }

    private static void Extend(List<(char Kind, int Length)> runs, char next)
    {
        if (runs[^1].Kind == next)
        {
            runs[^1] = (next, runs[^1].Length + 1);
        }
        else
        {
            runs.Add((next, 1));
        }
    }

    // This repo's own SegmentTree<int, MaxOperation<int>>, composed via
    // ActiveSectionTradeIndex's per-string preprocessing (§17.3 witness) -
    // hoisted out of the per-query loop exactly like OpenTheLock hoists a
    // built LockGraph, so the O(n) build is [GlobalSetup]'s cost and each
    // query pays only its own O(log n) range-max lookup.
    public static int[] MaxActiveAfterTradeByRangeMaxIndex(string text, int[][] queries)
    {
        var index = new ActiveSectionTradeIndex(text);

        return MaxActiveAfterTradeByRangeMaxIndex(index, queries);
    }

    public static int[] MaxActiveAfterTradeByRangeMaxIndex(ActiveSectionTradeIndex index, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = index.BestActiveAfterTrade(queries[i][0], queries[i][1]);
        }

        return answers;
    }
}
