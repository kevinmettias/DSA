using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.Set;

using BclProfitStack = System.Collections.Generic.Stack<int>;
using RepoProfitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximumEleganceOfAKLengthSubsequence;

// LeetCode 2813. Maximum Elegance of a K-Length Subsequence: choose exactly
// subsequenceLength items (profit, category) to maximize
// sum(profit) + distinctCategoryCount^2.
// Sort by profit descending and greedily take the first subsequenceLength (the
// best possible profit sum); any item beyond the first subsequenceLength whose
// category is NEW can only help
// by swapping in for the SMALLEST-profit item already taken from a category
// that's already represented twice - exactly a LIFO stack of "replaceable"
// duplicate profits, since items are scanned in descending-profit order, so the
// most recently pushed duplicate is always the smallest one available to swap
// out.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm. Repo Stack<T> is aliased
// (RepoProfitStack) rather than used via a bare `using` + `new Stack<T>()`: this
// project's global `using System.Collections.Generic;` makes a bare `Stack<T>`
// ambiguous (CS0104) against the BCL's own Stack<T>.
internal static class MaximumEleganceOfAKLengthSubsequenceSolution
{
    // Baseline: BCL OrderByDescending, System.Collections.Generic.Stack<int> for
    // duplicate profits, HashSet<int> for seen categories - "what you'd write
    // without this repo".
    public static long MaximumEleganceByBcl(int[][] items, int subsequenceLength)
    {
        var sorted = items
            .Select(item => (Profit: item[0], Category: item[1]))
            .OrderByDescending(item => item.Profit)
            .ToArray();

        var (total, seen, duplicates) = TakeFirstKWithBclSets(sorted, subsequenceLength);
        var best = total + ((long)seen.Count * seen.Count);

        (total, best) = SwapInNewCategoriesWithBclSets(
            sorted, subsequenceLength, (total, best), (seen, duplicates));

        return best;
    }

    // The opening phase of the greedy: take the subsequenceLength highest-profit
    // items outright - the best possible profit sum - remembering every category
    // already represented and stacking the profit of each duplicate category as a
    // candidate to swap back out.
    private static (long Total, HashSet<int> Seen, BclProfitStack Duplicates) TakeFirstKWithBclSets(
        (int Profit, int Category)[] sorted,
        int subsequenceLength)
    {
        long total = 0;
        var seen = new HashSet<int>();
        var duplicates = new BclProfitStack();

        for (var i = 0; i < subsequenceLength; i++)
        {
            total += sorted[i].Profit;

            if (!seen.Add(sorted[i].Category))
            {
                duplicates.Push(sorted[i].Profit);
            }
        }

        return (total, seen, duplicates);
    }

    // The closing phase: every remaining item from a brand-new category is worth
    // swapping in for the smallest duplicate profit taken so far, since the
    // profit sum falls only by that duplicate while distinct categories go up.
    private static (long Total, long Best) SwapInNewCategoriesWithBclSets(
        (int Profit, int Category)[] sorted,
        int subsequenceLength,
        (long Total, long Best) running,
        (HashSet<int> Seen, BclProfitStack Duplicates) taken)
    {
        for (var i = subsequenceLength; i < sorted.Length; i++)
        {
            if (taken.Duplicates.Count == 0)
            {
                break;
            }

            if (!taken.Seen.Add(sorted[i].Category))
            {
                continue;
            }

            running.Total += sorted[i].Profit - taken.Duplicates.Pop();
            running.Best = Math.Max(
                running.Best, running.Total + ((long)taken.Seen.Count * taken.Seen.Count));
        }

        return running;
    }

    // Composed: the same greedy-swap algorithm, sorted via this repo's MergeSort
    // over ArrayIndexedSequence<Element> (the same composition
    // TheNumberOfWeakCharactersInTheGameTests uses), seen categories tracked
    // with this repo's Set<int>, replaceable duplicate profits tracked with this
    // repo's own Stack<int>.
    public static long MaximumEleganceByRepoPrimitives(int[][] items, int subsequenceLength)
    {
        var sorted = SortByProfitDescending(items);

        var (total, seen, duplicates) = TakeFirstKWithRepoPrimitives(sorted, subsequenceLength);
        var best = total + ((long)seen.Count * seen.Count);

        (total, best) = SwapInNewCategoriesWithRepoPrimitives(
            sorted, subsequenceLength, (total, best), (seen, duplicates));

        return best;
    }

    private static (int Profit, int Category)[] SortByProfitDescending(int[][] items)
    {
        var sorted = items.Select(item => (Profit: item[0], Category: item[1])).ToArray();
        var byProfitDescending = Comparer<(int Profit, int Category)>.Create((a, b) => b.Profit.CompareTo(a.Profit));

        MergeSort.Sort<(int Profit, int Category), ArrayIndexedSequence<(int Profit, int Category)>>(
            new ArrayIndexedSequence<(int Profit, int Category)>(sorted), byProfitDescending);

        return sorted;
    }

    private static (long Total, Set<int> Seen, RepoProfitStack Duplicates) TakeFirstKWithRepoPrimitives(
        (int Profit, int Category)[] sorted,
        int subsequenceLength)
    {
        long total = 0;
        var seen = new Set<int>();
        var duplicates = new RepoProfitStack();

        for (var i = 0; i < subsequenceLength; i++)
        {
            total += sorted[i].Profit;

            if (!seen.TryAdd(sorted[i].Category))
            {
                duplicates.Push(sorted[i].Profit);
            }
        }

        return (total, seen, duplicates);
    }

    private static (long Total, long Best) SwapInNewCategoriesWithRepoPrimitives(
        (int Profit, int Category)[] sorted,
        int subsequenceLength,
        (long Total, long Best) running,
        (Set<int> Seen, RepoProfitStack Duplicates) taken)
    {
        for (var i = subsequenceLength; i < sorted.Length; i++)
        {
            if (taken.Duplicates.Count == 0)
            {
                break;
            }

            if (!taken.Seen.TryAdd(sorted[i].Category))
            {
                continue;
            }

            taken.Duplicates.TryPop(out var removedProfit);
            running.Total += sorted[i].Profit - removedProfit;
            running.Best = Math.Max(
                running.Best, running.Total + ((long)taken.Seen.Count * taken.Seen.Count));
        }

        return running;
    }
}
