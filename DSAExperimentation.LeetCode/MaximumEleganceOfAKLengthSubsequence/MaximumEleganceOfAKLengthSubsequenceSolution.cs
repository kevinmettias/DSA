using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.Set;

using RepoProfitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximumEleganceOfAKLengthSubsequence;

// LeetCode 2813. Maximum Elegance of a K-Length Subsequence: choose exactly k
// items (profit, category) to maximize sum(profit) + distinctCategoryCount^2.
// Sort by profit descending and greedily take the first k (the best possible
// profit sum); any item beyond the first k whose category is NEW can only help
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
    public static long MaximumEleganceByBcl(int[][] items, int k)
    {
        var sorted = items
            .Select(item => (Profit: item[0], Category: item[1]))
            .OrderByDescending(item => item.Profit)
            .ToArray();

        long total = 0;
        var seen = new HashSet<int>();
        var duplicates = new System.Collections.Generic.Stack<int>();

        for (var i = 0; i < k; i++)
        {
            total += sorted[i].Profit;

            if (!seen.Add(sorted[i].Category))
            {
                duplicates.Push(sorted[i].Profit);
            }
        }

        var best = total + ((long)seen.Count * seen.Count);

        for (var i = k; i < sorted.Length; i++)
        {
            if (duplicates.Count == 0)
            {
                break;
            }

            if (!seen.Add(sorted[i].Category))
            {
                continue;
            }

            total += sorted[i].Profit - duplicates.Pop();
            best = Math.Max(best, total + ((long)seen.Count * seen.Count));
        }

        return best;
    }

    // Composed: the same greedy-swap algorithm, sorted via this repo's MergeSort
    // over ArrayIndexedSequence<Element> (the same composition
    // TheNumberOfWeakCharactersInTheGameTests uses), seen categories tracked
    // with this repo's Set<int>, replaceable duplicate profits tracked with this
    // repo's own Stack<int>.
    public static long MaximumEleganceByRepoPrimitives(int[][] items, int k)
    {
        var sorted = items.Select(item => (Profit: item[0], Category: item[1])).ToArray();
        var byProfitDescending = Comparer<(int Profit, int Category)>.Create((a, b) => b.Profit.CompareTo(a.Profit));

        MergeSort.Sort<(int Profit, int Category), ArrayIndexedSequence<(int Profit, int Category)>>(
            new ArrayIndexedSequence<(int Profit, int Category)>(sorted), byProfitDescending);

        long total = 0;
        var seen = new Set<int>();
        var duplicates = new RepoProfitStack();

        for (var i = 0; i < k; i++)
        {
            total += sorted[i].Profit;

            if (!seen.TryAdd(sorted[i].Category))
            {
                duplicates.Push(sorted[i].Profit);
            }
        }

        var best = total + ((long)seen.Count * seen.Count);

        for (var i = k; i < sorted.Length; i++)
        {
            if (duplicates.Count == 0)
            {
                break;
            }

            if (!seen.TryAdd(sorted[i].Category))
            {
                continue;
            }

            duplicates.TryPop(out var removedProfit);
            total += sorted[i].Profit - removedProfit;
            best = Math.Max(best, total + ((long)seen.Count * seen.Count));
        }

        return best;
    }
}
