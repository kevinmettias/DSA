using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToReorderArrayToGetSameBST;

// LeetCode 1569. Number of Ways to Reorder Array to Get Same BST: build the actual
// BST via this repo's own BinarySearchTree<int>.Insert, then fold it bottom-up (this
// repo's own TreeFold/IFoldAlgebra catamorphism, the same machinery TreeMetrics
// closes over BinaryTreeNode<int> for) computing, per node, its subtree size and the
// number of insertion orders that reproduce it - C(leftSize+rightSize, leftSize)
// ways to interleave the two already-counted child subsequences, times each child's
// own count. The binomial coefficient itself is Pascal's identity memoized by this
// repo's own Memoizer<TState,TResult>, the same shape UniqueBinarySearchTreesTests
// already uses for Catalan numbers. The answer excludes the original array itself.
public sealed partial class NumberOfWaysToReorderArrayToGetSameBSTTests
{
    [Fact]
    public void NumOfWays_LeetCodeExampleOne_ReturnsOne()
        => Assert.Equal(1, NumOfWays([2, 1, 3]));

    [Fact]
    public void NumOfWays_LeetCodeExampleTwo_ReturnsFive()
        => Assert.Equal(5, NumOfWays([3, 4, 5, 1, 2]));

    [Fact]
    public void NumOfWays_AlreadyAscendingChain_ReturnsZero()
        => Assert.Equal(0, NumOfWays([1, 2, 3]));

    [Fact]
    public void NumOfWays_LeetCodeExampleFour_ReturnsNineteen()
        => Assert.Equal(19, NumOfWays([3, 1, 2, 5, 4, 6]));

    private const long Modulo = 1_000_000_007;

    private static int NumOfWays(int[] nums)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in nums)
        {
            tree.Insert(value);
        }

        var (_, ways) = TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            WaysAlgebra, (int Size, long Ways)>(tree.Root);

        return (int)((ways - 1 + Modulo) % Modulo);
    }

    private static long Choose(int n, int k) => Memoizer.Memoize<(int N, int K), long>((n, k), ChooseRecurrence);

    private static long ChooseRecurrence((int N, int K) state, Func<(int, int), long> choose)
    {
        var (n, k) = state;
        return k == 0 || k == n ? 1 : (choose((n - 1, k - 1)) + choose((n - 1, k))) % Modulo;
    }

    // Per node: total child-subtree size, and the ways to interleave every child's
    // already-counted insertion sequence back into one sequence for this node - a
    // binary tree has at most 2 children, so choosing one child's size out of the
    // combined total is the same C(n,k) regardless of which child is picked first
    // (C(n,k) = C(n,n-k)), and reduces to the correct "1 way" for 0 or 1 children.
    private readonly struct WaysAlgebra : IFoldAlgebra<BinaryTreeNode<int>, (int Size, long Ways)>
    {
        public static (int Size, long Ways) Empty => (0, 1);

        public static (int Size, long Ways) Combine(BinaryTreeNode<int> node, IReadOnlyList<(int Size, long Ways)> children)
        {
            if (children.Count == 0)
            {
                return (1, 1);
            }

            var totalSize = 0;
            var ways = 1L;

            foreach (var child in children)
            {
                totalSize += child.Size;
                ways = ways * child.Ways % Modulo;
            }

            if (children.Count == 2)
            {
                ways = ways * Choose(totalSize, children[0].Size) % Modulo;
            }

            return (totalSize + 1, ways);
        }
    }
}
