using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.HandOfStraights;

// LeetCode 846. Hand of Straights: can the hand be split into groups of exactly
// groupSize consecutive cards?
//
// Both strategies run the same greedy proof: count how many of each value remain,
// walk the hand in ascending order, and at every card that still has copies left
// start a fresh run of groupSize consecutive values. That is correct because, in
// ascending order, a card with copies remaining can only ever be the bottom of a
// group - nothing smaller is left to sit beneath it.
//
// The two arms differ only in which structures do the counting and the sorting:
// the BCL's Dictionary<int,int> plus Array.Sort, or this repo's own
// HashMap<int,int> plus MergeSort over an ArrayIndexedSequence<int>.
internal static class HandOfStraightsSolution
{
    // The textbook answer: BCL Dictionary<int,int> tally and Array.Sort.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static bool IsNStraightHandByBclDictionary(int[] hand, int groupSize)
    {
        if (hand.Length % groupSize != 0)
        {
            return false;
        }

        var counts = BuildBclCounts(hand);
        var sortedHand = BclSortedCopy(hand);

        return ConsumeStraightsByBclDictionary(counts, sortedHand, groupSize);
    }

    private static Dictionary<int, int> BuildBclCounts(int[] hand)
    {
        var counts = new Dictionary<int, int>();

        foreach (var card in hand)
        {
            counts[card] = counts.GetValueOrDefault(card) + 1;
        }

        return counts;
    }

    private static int[] BclSortedCopy(int[] hand)
    {
        var sortedHand = (int[])hand.Clone();
        Array.Sort(sortedHand);

        return sortedHand;
    }

    // Every card that still has copies left must start a group of its own: in
    // ascending order nothing smaller is left to sit beneath it.
    private static bool ConsumeStraightsByBclDictionary(
        Dictionary<int, int> counts, int[] sortedHand, int groupSize)
    {
        foreach (var card in sortedHand)
        {
            if (counts[card] == 0)
            {
                continue;
            }

            if (!TryConsumeGroupByBclDictionary(counts, card, groupSize))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryConsumeGroupByBclDictionary(
        Dictionary<int, int> counts, int card, int groupSize)
    {
        for (var next = card; next < card + groupSize; next++)
        {
            if (!counts.TryGetValue(next, out var nextCount) || nextCount == 0)
            {
                return false;
            }

            counts[next] = nextCount - 1;
        }

        return true;
    }

    // This repo's own HashMap<int,int> holds the remaining-copies tally and this
    // repo's own MergeSort orders a working copy of the hand, over the same
    // ArrayIndexedSequence<int> witness Sorting/MergeSort's own tests use.
    public static bool IsNStraightHandByHashMapMergeSort(int[] hand, int groupSize)
    {
        if (hand.Length % groupSize != 0)
        {
            return false;
        }

        var counts = BuildCounts(hand);
        var sortedHand = SortedCopy(hand);

        return ConsumeStraightsByHashMap(counts, sortedHand, groupSize);
    }

    private static HashMap<int, int> BuildCounts(int[] hand)
    {
        var counts = new HashMap<int, int>();

        foreach (var card in hand)
        {
            counts.TryGetValue(card, out var count);
            counts.Set(card, count + 1);
        }

        return counts;
    }

    // Sorts a clone so the caller's hand - a benchmark's [GlobalSetup] workload,
    // reused across every invocation - is never reordered underneath it.
    private static int[] SortedCopy(int[] hand)
    {
        var sortedHand = (int[])hand.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedHand));

        return sortedHand;
    }

    private static bool ConsumeStraightsByHashMap(HashMap<int, int> counts, int[] sortedHand, int groupSize)
    {
        foreach (var card in sortedHand)
        {
            counts.TryGetValue(card, out var remaining);

            if (remaining == 0)
            {
                continue;
            }

            if (!TryConsumeGroup(counts, card, groupSize))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryConsumeGroup(HashMap<int, int> counts, int card, int groupSize)
    {
        for (var next = card; next < card + groupSize; next++)
        {
            if (!counts.TryGetValue(next, out var nextCount) || nextCount == 0)
            {
                return false;
            }

            counts.Set(next, nextCount - 1);
        }

        return true;
    }
}
