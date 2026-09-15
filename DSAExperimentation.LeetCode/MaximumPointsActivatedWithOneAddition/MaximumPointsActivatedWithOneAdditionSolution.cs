using DSAExperimentation.DataStructures.KeyedDisjointSet;

namespace DSAExperimentation.LeetCode.MaximumPointsActivatedWithOneAddition;

// LeetCode 3873. Maximum Points Activated with One Addition: activating a point
// activates every point sharing its x or its y coordinate, and that cascades.
// So the original points partition into connected components of a bipartite
// multigraph whose two node sets are the distinct x-values and the distinct
// y-values, one edge per point. One new point (X, Y) at an existing x-value X
// and an existing y-value Y activates X's whole component, Y's whole component,
// and itself - merging the two if they differ (they can't already share a point,
// since that would mean they were already the same component). Picking X and Y
// from the two largest components (or the same single component when there is
// only one) is always achievable and always at least as good as any other
// choice, so the answer is simply the two largest component sizes plus one.
internal static class MaximumPointsActivatedWithOneAdditionSolution
{
    // The textbook union-find: a plain BCL Dictionary keyed by a (IsX, Value)
    // tuple so the x=5 node and the y=5 node never collide, path-compressed by
    // hand. Deliberately written without this repo's disjoint-set primitives -
    // the arm the composed strategy below has to justify itself against.
    public static int MaxActivatedByBruteForceUnionFind(int[][] points)
    {
        var parent = new Dictionary<(bool IsX, int Value), (bool IsX, int Value)>();

        foreach (var point in points)
        {
            Union(parent, (true, point[0]), (false, point[1]));
        }

        var sizeByRoot = new Dictionary<(bool IsX, int Value), int>();

        foreach (var point in points)
        {
            var root = Find(parent, (true, point[0]));
            sizeByRoot[root] = sizeByRoot.GetValueOrDefault(root) + 1;
        }

        return SumOfTwoLargest(sizeByRoot.Values) + 1;
    }

    private static void Union(
        Dictionary<(bool IsX, int Value), (bool IsX, int Value)> parent,
        (bool IsX, int Value) first, (bool IsX, int Value) second)
    {
        var firstRoot = Find(parent, first);
        var secondRoot = Find(parent, second);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }

    // Coordinate-compressed union-find via this repo's own KeyedDisjointSet<TKey>
    // (§10.4/OpenTheLock precedent: composition over redesign, an id-assignment
    // map plus DisjointSet's O(a(n)) Find/Union rather than a hand-rolled parent
    // dictionary). AxisKey plays the same "is this an x-node or a y-node" role
    // as the (bool, int) tuple above, just as a named record struct instead.
    public static int MaxActivatedByKeyedDisjointSet(int[][] points)
    {
        var disjointSet = new KeyedDisjointSet<AxisKey>(BuildAxisKeys(points));
        UnionPointAxes(disjointSet, points);

        var sizeByRoot = TallyRootSizes(disjointSet, points);

        return SumOfTwoLargest(sizeByRoot.Values) + 1;
    }

    // One node per distinct x-value and one per distinct y-value; each point is
    // then a single edge between its column node and its row node.
    private static List<AxisKey> BuildAxisKeys(int[][] points)
    {
        var keys = new List<AxisKey>(points.Length * 2);

        foreach (var point in points)
        {
            keys.Add(new AxisKey(true, point[0]));
            keys.Add(new AxisKey(false, point[1]));
        }

        return keys;
    }

    private static void UnionPointAxes(KeyedDisjointSet<AxisKey> disjointSet, int[][] points)
    {
        foreach (var point in points)
        {
            disjointSet.TryUnion(new AxisKey(true, point[0]), new AxisKey(false, point[1]));
        }
    }

    // Each point is counted once, against its own x-node's component root, so a
    // component's size is exactly the number of points it activates.
    private static Dictionary<AxisKey, int> TallyRootSizes(
        KeyedDisjointSet<AxisKey> disjointSet, int[][] points)
    {
        var sizeByRoot = new Dictionary<AxisKey, int>();

        foreach (var point in points)
        {
            disjointSet.TryFind(new AxisKey(true, point[0]), out var root);
            sizeByRoot[root] = sizeByRoot.GetValueOrDefault(root) + 1;
        }

        return sizeByRoot;
    }

    private static (bool IsX, int Value) Find(
        Dictionary<(bool IsX, int Value), (bool IsX, int Value)> parent, (bool IsX, int Value) node)
    {
        if (!parent.TryGetValue(node, out var next))
        {
            parent[node] = node;
            return node;
        }

        if (next == node)
        {
            return node;
        }

        var root = Find(parent, next);
        parent[node] = root;
        return root;
    }

    private static int SumOfTwoLargest(IEnumerable<int> componentSizes)
    {
        var largest = 0;
        var secondLargest = 0;

        foreach (var size in componentSizes)
        {
            if (size > largest)
            {
                secondLargest = largest;
                largest = size;
            }
            else if (size > secondLargest)
            {
                secondLargest = size;
            }
        }

        return largest + secondLargest;
    }

    // Distinguishes an x-node from a y-node at the same numeric coordinate -
    // point (5, 5) must not union the x=5 column with itself via a same-valued
    // row. Meaningful only to this problem's bipartite reduction.
    private readonly record struct AxisKey(bool IsX, int Value);
}
