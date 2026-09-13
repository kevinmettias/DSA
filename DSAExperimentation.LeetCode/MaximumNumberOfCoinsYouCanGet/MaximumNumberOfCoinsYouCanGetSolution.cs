using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumNumberOfCoinsYouCanGet;

// LeetCode 1561. Maximum Number of Coins You Can Get: piles are taken three at a
// time - Alice takes the largest of the three, you take the second largest, Bob
// takes the smallest - and you want to maximize your total.
//
// The provably optimal play always pairs the current maximum (Alice's) with the
// next highest (yours) and lets Bob absorb one of the smallest piles, so over a
// sorted-ascending array of 3n piles your share is exactly indices n, n+2, ...,
// 3n-2. The two strategies differ only in how far they lean on that: the baseline
// simulates the rounds literally with a linear scan per pick, while the composed
// arm sorts once with this repo's own MergeSort and reads the answer off the
// sorted array.
//
// Both work on a copy, so the caller's array - and a benchmark re-invoking an arm -
// always starts from the same unsorted input.
internal static class MaximumNumberOfCoinsYouCanGetSolution
{
    // Three piles are consumed per round: Alice's, yours, Bob's.
    private const int GroupSize = 3;

    // Sorted ascending, your piles are every second one from index n upward.
    private const int PickStride = 2;

    // The textbook baseline: play the game out round by round, rescanning the
    // remaining piles for the maximum and the minimum each time - O(n^2), the way
    // most people solve this before spotting the index pattern. Deliberately
    // written on BCL types alone.
    public static int MaxCoinsByRoundSimulation(int[] piles)
    {
        var remaining = new List<int>(piles);
        var total = 0;

        while (remaining.Count > 0)
        {
            RemoveMax(remaining);
            total += RemoveMax(remaining);
            RemoveMin(remaining);
        }

        return total;
    }

    private static int RemoveMax(List<int> values)
    {
        var maxIndex = 0;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] > values[maxIndex])
            {
                maxIndex = i;
            }
        }

        var value = values[maxIndex];
        values.RemoveAt(maxIndex);
        return value;
    }

    private static void RemoveMin(List<int> values)
    {
        var minIndex = 0;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] < values[minIndex])
            {
                minIndex = i;
            }
        }

        values.RemoveAt(minIndex);
    }

    // This repo's own MergeSort over the array wrapped as an IIndexedSequence<int>
    // - the same composition SortAnArray uses - with the picking arithmetic on top:
    // one O(n log n) sort, then a walk over your n indices.
    public static int MaxCoinsByMergeSort(int[] piles)
    {
        var sorted = piles.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var rounds = sorted.Length / GroupSize;
        var total = 0;

        for (var i = 0; i < rounds; i++)
        {
            total += sorted[rounds + (PickStride * i)];
        }

        return total;
    }
}
