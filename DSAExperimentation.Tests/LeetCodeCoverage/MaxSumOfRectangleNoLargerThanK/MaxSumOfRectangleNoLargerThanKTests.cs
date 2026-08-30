using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxSumOfRectangleNoLargerThanK;

// LeetCode 363. Max Sum of Rectangle No Larger Than K: for every left/right column
// pair, compress the matrix to a 1D row-sum array via a running total, then find
// its best window summing to no more than k using this repo's own
// BinarySearchTree<int> holding every prefix sum seen so far - for each new prefix
// sum, the SMALLEST previously-seen prefix sum >= (prefix - k) gives the largest
// window not exceeding k. That's the classic TreeSet-ceiling approach, walked
// directly over BinaryTreeNode<int>'s already-public Value/Left/Right rather than a
// new production primitive (the same kind of problem-specific traversal
// KthSmallestElementInABSTTests' RankHooks already composes over this tree).
public sealed partial class MaxSumOfRectangleNoLargerThanKTests
{
    [Fact]
    public void MaxSumSubmatrix_ClassicExample_ReturnsLargestSumWithinLimit()
    {
        int[][] matrix = [[1, 0, 1], [0, -2, 3]];

        Assert.Equal(2, MaxSumSubmatrix(matrix, k: 2));
    }

    [Fact]
    public void MaxSumSubmatrix_SingleRowAtExactLimit_ReturnsFullRowSum()
    {
        int[][] matrix = [[2, 2, -1]];

        Assert.Equal(3, MaxSumSubmatrix(matrix, k: 3));
    }

    private static int MaxSumSubmatrix(int[][] matrix, int k)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var best = int.MinValue;

        for (var left = 0; left < cols; left++)
        {
            var rowSums = new int[rows];

            for (var right = left; right < cols; right++)
            {
                for (var r = 0; r < rows; r++)
                {
                    rowSums[r] += matrix[r][right];
                }

                best = Math.Max(best, BestWindowNoLargerThanK(rowSums, k));
            }
        }

        return best;
    }

    private static int BestWindowNoLargerThanK(int[] rowSums, int k)
    {
        var prefixes = new BinarySearchTree<int>();
        prefixes.Insert(0);

        var best = int.MinValue;
        var prefix = 0;

        foreach (var value in rowSums)
        {
            prefix += value;

            if (TryFindCeiling(prefixes, prefix - k, out var ceiling))
            {
                best = Math.Max(best, prefix - ceiling);
            }

            prefixes.Insert(prefix);
        }

        return best;
    }

    // Smallest inserted value >= target - the standard BST ceiling walk, composed
    // directly over BinarySearchTree<int>'s exposed Root and BinaryTreeNode<int>'s
    // Left/Right/Value.
    private static bool TryFindCeiling(BinarySearchTree<int> tree, int target, out int ceiling)
    {
        var node = tree.Root;
        var found = false;
        ceiling = default;

        while (node is not null)
        {
            if (node.Value >= target)
            {
                ceiling = node.Value;
                found = true;
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }

        return found;
    }
}
