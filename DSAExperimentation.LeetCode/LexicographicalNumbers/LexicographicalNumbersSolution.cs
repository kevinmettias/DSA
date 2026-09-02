using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.LexicographicalNumbers;

// LeetCode 386. Lexicographical Numbers: list 1..n in the order their string
// representations would sort.
//
// The lexicographical order is exactly a DFS pre-order over the implicit 10-ary
// "next digit" tree - root digits 1-9, each node x has children x*10 .. x*10+9,
// pruned by <= n - so the composed strategy is this repo's own successor-function
// DepthFirstSearch.Traverse, the "implicit graph, no Representation axis" shape
// ARCHITECTURE.md's Traversal section documents. Each root's subtree is a disjoint
// range, so one Traverse call per root still produces the correct global order.
internal static class LexicographicalNumbersSolution
{
    private const int MaxDigit = 9;
    private const int DecimalBase = 10;

    // The textbook answer: materialize every value's decimal string and sort by it.
    // Deliberately written without this repo's primitives - the O(n log n) arm the
    // DFS walk below has to justify itself against.
    public static List<int> LexicalOrderByStringSort(int n) =>
        Enumerable.Range(1, n)
            .OrderBy(value => value.ToString(), StringComparer.Ordinal)
            .ToList();

    public static List<int> LexicalOrderByDepthFirstDigitTree(int n)
    {
        var order = new List<int>(n);

        for (var root = 1; root <= MaxDigit && root <= n; root++)
        {
            var traversal = DepthFirstSearch.Traverse(root, current => Successors(current, n));
            order.AddRange(traversal);
        }

        return order;
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
