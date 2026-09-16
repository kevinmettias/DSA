using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

using RepoSegmentTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<int, DSAExperimentation.DataStructures.SegmentTree.MaxOperation<int>>;

namespace DSAExperimentation.LeetCode.LengthOfTheLongestIncreasingPath;

// LeetCode 3288. Length of the Longest Increasing Path: coordinates[requiredIndex]
// must appear somewhere in a sequence of points whose x and y both strictly increase
// step to step; return the longest such sequence's length. This is 2D LIS split at a
// fixed point: the answer is (longest chain ENDING at requiredIndex) + (longest chain
// STARTING at requiredIndex) - 1, the shared point counted once.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class LengthOfTheLongestIncreasingPathSolution
{
    // Textbook O(n^2): memoized DP over every ordered pair, directly on
    // coordinates[][]. LongestChainEndingAt recurses only into points with strictly
    // smaller x and y, so it terminates without needing a topological sort of its
    // own; negating every point turns "longest chain starting at i, extending toward
    // larger x and y" into "longest chain ending at i in the negated plane", which is
    // how the one helper answers both halves here - the arm the segment-tree strategy
    // below has to beat.
    public static int MaxPathLengthByBruteForce(int[][] coordinates, int requiredIndex)
    {
        var n = coordinates.Length;
        var endingAt = new int[n];
        var startingAt = new int[n];
        Array.Fill(endingAt, -1);
        Array.Fill(startingAt, -1);

        return LongestChainEndingAt(coordinates, endingAt, requiredIndex)
            + LongestChainEndingAt(Negate(coordinates), startingAt, requiredIndex) - 1;
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
    public static int MaxPathLengthBySegmentTree(int[][] coordinates, int requiredIndex)
    {
        var endingAt = ChainLengths(coordinates);
        var startingAt = ChainLengths(Negate(coordinates));

        return endingAt[requiredIndex] + startingAt[requiredIndex] - 1;
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
        var chain = new int[points.Length];

        if (chain.Length == 0)
        {
            return chain;
        }

        var ySequence = new ArraySequence<long>(points.Select(p => (long)p[1]).Distinct().OrderBy(y => y).ToArray());
        var tree = new RepoSegmentTree(new int[ySequence.Length]);
        var sweep = Enumerable.Range(0, points.Length)
            .Select(i => (Index: i, X: (long)points[i][0], Rank: BinarySearch.LowerBound(ySequence, (long)points[i][1])))
            .OrderBy(point => point.X)
            .ToArray();

        var groupStart = 0;

        while (groupStart < sweep.Length)
        {
            groupStart = SweepEqualXGroup(sweep, tree, chain, groupStart);
        }

        return chain;
    }

    // One x-ascending group at a time, returning the start of the next group. The read
    // pass runs to completion before the write pass begins, which is what keeps two
    // equal-x points from seeing each other as a valid predecessor.
    private static int SweepEqualXGroup(
        (int Index, long X, int Rank)[] sweep, RepoSegmentTree tree, int[] chain, int groupStart)
    {
        var groupEnd = groupStart;

        while (groupEnd < sweep.Length && sweep[groupEnd].X == sweep[groupStart].X)
        {
            groupEnd++;
        }

        for (var idx = groupStart; idx < groupEnd; idx++)
        {
            var point = sweep[idx];
            var best = point.Rank > 0 ? tree.Query(0, point.Rank - 1) : 0;
            chain[point.Index] = best + 1;
        }

        for (var idx = groupStart; idx < groupEnd; idx++)
        {
            WritePointToTree(sweep[idx], tree, chain);
        }

        return groupEnd;
    }

    // Write pass: fold one point's chain length into the tree at its own compressed rank.
    private static void WritePointToTree((int Index, long X, int Rank) point, RepoSegmentTree tree, int[] chain)
    {
        var current = tree.Query(point.Rank, point.Rank);
        var updated = Math.Max(current, chain[point.Index]);
        tree.Update(point.Rank, updated);
    }

    // The longest strictly increasing chain that ends at point i, memoized: point j is
    // a valid predecessor exactly when both of its coordinates sit strictly below point
    // i's. A caller wanting the chain STARTING at i negates every point first, which
    // reverses every chain's direction - see Negate.
    private static int LongestChainEndingAt(int[][] points, int[] memo, int index)
    {
        if (memo[index] >= 0)
        {
            return memo[index];
        }

        var best = 0;

        for (var j = 0; j < points.Length; j++)
        {
            if (points[j][0] < points[index][0] && points[j][1] < points[index][1])
            {
                var candidate = LongestChainEndingAt(points, memo, j);
                best = Math.Max(best, candidate);
            }
        }

        return memo[index] = best + 1;
    }

    // Negating both coordinates of every point reverses every chain's direction:
    // "longest chain starting at i" over the original points is "longest chain ending
    // at i" over the negated ones, so both halves of the answer come from the one
    // chain helper above rather than from a second, mirrored implementation.
    private static int[][] Negate(int[][] coordinates)
    {
        var negated = new int[coordinates.Length][];

        for (var i = 0; i < coordinates.Length; i++)
        {
            negated[i] = [-coordinates[i][0], -coordinates[i][1]];
        }

        return negated;
    }
}
