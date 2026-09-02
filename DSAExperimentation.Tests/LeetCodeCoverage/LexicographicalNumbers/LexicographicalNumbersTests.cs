using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicalNumbers;

// LeetCode 386. Lexicographical Numbers: DFS over the implicit 10-ary "next digit"
// tree (root digits 1-9, each node x has children x*10 .. x*10+9, pruned by <= n)
// via this repo's own successor-function DepthFirstSearch.Traverse - the exact
// "implicit graph, no Representation axis" example ARCHITECTURE.md's Traversal
// section documents. Each root's subtree is a disjoint range, so one Traverse call
// per root (each with its own visited set) still produces the correct global order.
public sealed partial class LexicographicalNumbersTests
{
    [Fact]
    public void LexicalOrder_LeetCodeExample_MatchesExpectedOrder()
    {
        var result = LexicalOrder(13);

        Assert.Equal([1, 10, 11, 12, 13, 2, 3, 4, 5, 6, 7, 8, 9], result);
    }

    [Fact]
    public void LexicalOrder_SingleDigitLimit_ReturnsAscendingDigits()
    {
        var result = LexicalOrder(5);

        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    private static List<int> LexicalOrder(int n)
    {
        var order = new List<int>();

        for (var root = 1; root <= 9 && root <= n; root++)
        {
            var traversal = DepthFirstSearch.Traverse(root, current => Successors(current, n));
            order.AddRange(traversal);
        }

        return order;
    }

    private static IEnumerable<int> Successors(int current, int n)
    {
        for (var digit = 0; digit <= 9; digit++)
        {
            var next = (current * 10) + digit;
            if (next > n)
            {
                yield break;
            }

            yield return next;
        }
    }
}
