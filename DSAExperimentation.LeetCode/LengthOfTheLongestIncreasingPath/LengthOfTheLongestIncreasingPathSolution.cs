using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

using RepoSegmentTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<int, DSAExperimentation.DataStructures.SegmentTree.MaxOperation<int>>;

namespace DSAExperimentation.LeetCode.LengthOfTheLongestIncreasingPath;

// LeetCode 3288. Length of the Longest Increasing Path: coordinates[k] must appear
// somewhere in a sequence of points whose x and y both strictly increase step to
// step; return the longest such sequence's length. This is 2D LIS split at a fixed
// point: the answer is (longest chain ENDING at k) + (longest chain STARTING at k) - 1,
// the shared point counted once.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class LengthOfTheLongestIncreasingPathSolution
{
    // Textbook O(n^2): memoized DP over every ordered pair, directly on
    // coordinates[][]. EndingAt(i) recurses only into points with strictly
    // smaller x, StartingAt(i) only into points with strictly larger x, so
    // both terminate without needing a topological sort of their own - the
    // arm the segment-tree strategy below has to beat.
    public static int MaxPathLengthByBruteForce(int[][] coordinates, int k)
    {
        var n = coordinates.Length;
        var endingAt = new int[n];
        var startingAt = new int[n];
        Array.Fill(endingAt, -1);
        Array.Fill(startingAt, -1);

        return EndingAt(coordinates, endingAt, k) + StartingAt(coordinates, startingAt, k) - 1;
    }

    private static int EndingAt(int[][] coordinates, int[] memo, int i)
    {
        if (memo[i] >= 0)
        {
            return memo[i];
        }

        var best = 0;

        for (var j = 0; j < coordinates.Length; j++)
        {
            if (coordinates[j][0] < coordinates[i][0] && coordinates[j][1] < coordinates[i][1])
            {
                best = Math.Max(best, EndingAt(coordinates, memo, j));
            }
        }

        return memo[i] = best + 1;
    }

    private static int StartingAt(int[][] coordinates, int[] memo, int i)
    {
        if (memo[i] >= 0)
        {
            return memo[i];
        }

        var best = 0;

        for (var j = 0; j < coordinates.Length; j++)
        {
            if (coordinates[j][0] > coordinates[i][0] && coordinates[j][1] > coordinates[i][1])
            {
                best = Math.Max(best, StartingAt(coordinates, memo, j));
            }
        }

        return memo[i] = best + 1;
    }

    // Composed: ChainLengths below is a coordinate-compressed, x-ascending sweep
    // through a SegmentTree<int, MaxOperation<int>> - the same "compress + segment-tree
    // sweep" idiom MaximumBalancedSubsequenceSumSolution uses for its own O(n log n)
    // arm - giving the longest chain ENDING at every point in one pass. Negating both
    // coordinates of every point turns "longest chain STARTING at i, extending toward
    // larger x and y" into "longest chain ENDING at i in the negated plane, extending
    // from smaller negated x and y" (reversing a chain never changes its length), so
    // the same helper run twice - once as given, once on the negated points - produces
    // both halves without a second, mirrored implementation to keep in sync.
    public static int MaxPathLengthBySegmentTree(int[][] coordinates, int k)
    {
        var endingAt = ChainLengths(coordinates);
        var negated = new int[coordinates.Length][];

        for (var i = 0; i < coordinates.Length; i++)
        {
            negated[i] = [-coordinates[i][0], -coordinates[i][1]];
        }

        var startingAt = ChainLengths(negated);

        return endingAt[k] + startingAt[k] - 1;
    }

    // chain[i] = length of the longest increasing path ending at point i. Points are
    // swept in x-ascending order, grouped by equal x: every point in a group is READ
    // (queried) against the tree before any of them is WRITTEN back, so two points
    // sharing an x - which can never both sit on a strictly increasing path - never
    // see each other as a valid predecessor. y is coordinate-compressed via this
    // repo's own BinarySearch.LowerBound so the tree only needs as many leaves as
    // there are distinct y-values.
    private static int[] ChainLengths(int[][] points)
    {
        var n = points.Length;
        var chain = new int[n];

        if (n == 0)
        {
            return chain;
        }

        var sortedY = points.Select(p => (long)p[1]).Distinct().OrderBy(y => y).ToArray();
        var ySequence = new ArraySequence<long>(sortedY);
        var tree = new RepoSegmentTree(new int[sortedY.Length]);
        var order = Enumerable.Range(0, n).OrderBy(i => points[i][0]).ToArray();

        var groupStart = 0;

        while (groupStart < n)
        {
            var groupEnd = groupStart;

            while (groupEnd < n && points[order[groupEnd]][0] == points[order[groupStart]][0])
            {
                groupEnd++;
            }

            for (var idx = groupStart; idx < groupEnd; idx++)
            {
                var i = order[idx];
                var rank = BinarySearch.LowerBound(ySequence, (long)points[i][1]);
                var best = rank > 0 ? tree.Query(0, rank - 1) : 0;
                chain[i] = best + 1;
            }

            for (var idx = groupStart; idx < groupEnd; idx++)
            {
                var i = order[idx];
                var rank = BinarySearch.LowerBound(ySequence, (long)points[i][1]);
                tree.Update(rank, Math.Max(tree.Query(rank, rank), chain[i]));
            }

            groupStart = groupEnd;
        }

        return chain;
    }
}
