using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaxSumOfRectangleNoLargerThanK;

// LeetCode 363. Max Sum of Rectangle No Larger Than K: for every left/right column
// pair, compress the matrix to a 1D row-sum array via a running total, then find
// its best contiguous window summing to no more than sumLimit. Both strategies share
// that outer column-pair scan (ScanColumnPairs) and differ only in how a single
// row-sum window's best value is found - an O(rows^2) brute-force scan of every
// start/end pair, or this repo's own BinarySearchTree<int> holding every prefix sum
// seen so far: for each new prefix sum, the SMALLEST previously-seen prefix sum >=
// (prefix - sumLimit) gives the largest window not exceeding sumLimit, the classic TreeSet-
// ceiling approach, walked directly over BinaryTreeNode<int>'s already-public
// Value/Left/Right rather than a new production primitive (the same kind of
// problem-specific traversal KthSmallestElementInABSTSolution.RankHooks composes
// over this same tree).
internal static class MaxSumOfRectangleNoLargerThanKSolution
{
    // The textbook answer: every start/end window in the compressed row-sum array
    // checked directly, O(rows^2) per column pair - deliberately without this
    // repo's BinarySearchTree, the arm the ceiling-scan strategy below has to
    // justify itself against.
    public static int MaxSumSubmatrixByBruteForceWindowScan(int[][] matrix, int sumLimit) =>
        ScanColumnPairs(matrix, new BruteForceWindowBestSum(sumLimit));

    private static int BestWindowBruteForce(int[] rowSums, int sumLimit)
    {
        var best = int.MinValue;

        for (var start = 0; start < rowSums.Length; start++)
        {
            var sum = 0;

            for (var end = start; end < rowSums.Length; end++)
            {
                sum += rowSums[end];

                if (sum <= sumLimit)
                {
                    best = Math.Max(best, sum);
                }
            }
        }

        return best;
    }

    // This repo's own BinarySearchTree<int> holding every prefix sum seen so far,
    // answering each "smallest prefix sum >= (current - sumLimit)" query in O(log rows)
    // instead of an O(rows) scan.
    public static int MaxSumSubmatrixByBstCeilingScan(int[][] matrix, int sumLimit) =>
        ScanColumnPairs(matrix, new BstCeilingWindowBestSum(sumLimit));

    private static int BestWindowBstCeiling(int[] rowSums, int sumLimit)
    {
        var prefixes = new BinarySearchTree<int>();
        prefixes.Insert(0);

        var best = int.MinValue;
        var prefix = 0;

        foreach (var value in rowSums)
        {
            prefix += value;

            if (TryFindCeiling(prefixes, prefix - sumLimit, out var ceiling))
            {
                best = Math.Max(best, prefix - ceiling);
            }

            prefixes.Insert(prefix);
        }

        return best;
    }

    // Smallest inserted value >= target - the standard BST ceiling walk, composed
    // directly over BinarySearchTree<int>'s exposed Root and BinaryTreeNode<int>'s
    // Left/Right/Value. A problem-specific traversal, not a general-purpose
    // capability, so it stays here rather than becoming a new DataStructures/
    // witness (§17.6).
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

    // Shared column-pair/row-window scan shape both strategies ride: only how a
    // single row-sum window's best value is computed (brute force vs. BST ceiling
    // lookup) differs between them.
    private static int ScanColumnPairs(int[][] matrix, IWindowBestSum bestWindow)
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

                best = Math.Max(best, bestWindow.BestFor(rowSums));
            }
        }

        return best;
    }

    // What the shared column-pair scan asks of a strategy: given one compressed
    // row-sum array, find its best contiguous window summing to no more than
    // sumLimit. sumLimit is part of the answer's definition, so it is held by the
    // chooser built for it rather than passed alongside the array on every one of
    // the scan's calls.
    private interface IWindowBestSum
    {
        int BestFor(int[] rowSums);
    }

    private sealed class BruteForceWindowBestSum(int sumLimit) : IWindowBestSum
    {
        public int BestFor(int[] rowSums) => BestWindowBruteForce(rowSums, sumLimit);
    }

    private sealed class BstCeilingWindowBestSum(int sumLimit) : IWindowBestSum
    {
        public int BestFor(int[] rowSums) => BestWindowBstCeiling(rowSums, sumLimit);
    }
}
