using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.KthSmallestInLexicographicalOrder;

// LeetCode 440. K-th Smallest in Lexicographical Order: the lexicographical order of
// 1..n is exactly the pre-order DFS of the implicit 10-ary "next digit" tree
// LexicographicalNumbersTests (LC 386) already walks via this repo's own
// successor-function DepthFirstSearch.Traverse - the k-th smallest is simply the
// (k-1)-th element of that same order, one root-seeded Traverse call per digit 1-9
// since each root's subtree is a disjoint range.
internal static class KthSmallestInLexicographicalOrderSolution
{
    private const int MaxDigit = 9;
    private const int DecimalBase = 10;

    // The textbook answer: generate every number 1..n as a string and sort ordinally -
    // ordinal string comparison matches the same digit-by-digit order the implicit
    // 10-ary tree walks. Deliberately written without this repo's own primitives - it
    // is the arm the composed solution below has to justify itself against.
    public static int FindKthNumberByGenerateAndSort(int n, int k)
    {
        var values = new string[n];
        for (var i = 1; i <= n; i++)
        {
            values[i - 1] = i.ToString();
        }

        Array.Sort(values, StringComparer.Ordinal);

        return int.Parse(values[k - 1]);
    }

    // This repo's own DFS: DepthFirstSearch.Traverse walks the implicit 10-ary
    // "next digit" tree directly in lexicographical order, one root-seeded call per
    // digit 1-9 since each root's subtree is a disjoint range - no sort needed.
    public static int FindKthNumberByDepthFirstTraversal(int n, int k)
    {
        var order = new List<int>();
        for (var root = 1; root <= MaxDigit && root <= n; root++)
        {
            var traversal = DepthFirstSearch.Traverse(root, current => Successors(current, n));
            order.AddRange(traversal);
        }

        return order[k - 1];
    }

    private static IEnumerable<int> Successors(int current, int n)
    {
        for (var digit = 0; digit <= MaxDigit; digit++)
        {
            var next = (current * DecimalBase) + digit;
            if (next > n)
            {
                yield break;
            }

            yield return next;
        }
    }
}
