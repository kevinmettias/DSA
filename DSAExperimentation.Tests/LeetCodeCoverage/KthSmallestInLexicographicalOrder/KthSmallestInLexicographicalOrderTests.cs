using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestInLexicographicalOrder;

// LeetCode 440. K-th Smallest in Lexicographical Order: the lexicographical order of
// 1..n is exactly the pre-order DFS of the implicit 10-ary "next digit" tree
// LexicographicalNumbersTests (LC 386) already walks via this repo's own
// successor-function DepthFirstSearch.Traverse - the k-th smallest is simply the
// (k-1)-th element of that same order, one root-seeded Traverse call per digit 1-9
// since each root's subtree is a disjoint range.
public sealed partial class KthSmallestInLexicographicalOrderTests
{
    [Fact]
    public void FindKthNumber_LeetCodeExample_ReturnsExpectedValue()
    {
        var result = FindKthNumber(n: 13, k: 2);

        Assert.Equal(10, result);
    }

    [Fact]
    public void FindKthNumber_FirstElement_ReturnsSmallestRoot()
    {
        var result = FindKthNumber(n: 13, k: 1);

        Assert.Equal(1, result);
    }

    [Fact]
    public void FindKthNumber_LastElement_ReturnsLargestByLexOrder()
    {
        var result = FindKthNumber(n: 13, k: 13);

        Assert.Equal(9, result);
    }

    private static int FindKthNumber(int n, int k)
    {
        var order = new List<int>();
        for (var root = 1; root <= 9 && root <= n; root++)
        {
            var traversal = DepthFirstSearch.Traverse(root, current => Successors(current, n));
            order.AddRange(traversal);
        }

        return order[k - 1];
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
