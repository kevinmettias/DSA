using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumIceCreamBars;

// LeetCode 1833. Maximum Ice Cream Bars: with `coins` to spend and a bar priced
// costs[i], buy as many bars as possible. The greedy exchange argument settles it -
// any optimal selection can be reordered to take strictly cheaper-or-equal bars
// first without lowering its count - so both strategies buy in ascending price
// order and differ only in how they get that order.
internal static class MaximumIceCreamBarsSolution
{
    // The textbook answer without this repo: selection sort's inner loop, rescanning
    // for the cheapest bar not yet bought on every round, O(n^2) over nothing but
    // BCL arrays. It never reorders `costs`, so no defensive copy is needed.
    public static int MaxIceCreamBySelectionScan(int[] costs, int coins)
    {
        var bought = new bool[costs.Length];
        var count = 0;

        for (var round = 0; round < costs.Length; round++)
        {
            var cheapest = CheapestUnbought(costs, bought);

            if (costs[cheapest] > coins)
            {
                break;
            }

            bought[cheapest] = true;
            coins -= costs[cheapest];
            count++;
        }

        return count;
    }

    // Precondition: at least one bar is still unbought, which the caller's round
    // count guarantees - round r runs with exactly r bars bought out of costs.Length.
    private static int CheapestUnbought(int[] costs, bool[] bought)
    {
        var cheapest = 0;

        while (bought[cheapest])
        {
            cheapest++;
        }

        for (var i = cheapest + 1; i < costs.Length; i++)
        {
            if (!bought[i] && costs[i] < costs[cheapest])
            {
                cheapest = i;
            }
        }

        return cheapest;
    }

    // This repo's MergeSort over an ArrayIndexedSequence view of a private copy,
    // once up front, then a single O(n) walk that buys until the coins run out.
    // The copy is what keeps the caller's array unreordered.
    public static int MaxIceCreamByMergeSortGreedy(int[] costs, int coins)
    {
        var sorted = costs.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var count = 0;

        foreach (var cost in sorted)
        {
            if (cost > coins)
            {
                break;
            }

            coins -= cost;
            count++;
        }

        return count;
    }
}
