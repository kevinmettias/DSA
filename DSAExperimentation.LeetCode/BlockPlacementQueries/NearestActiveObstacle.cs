using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.BlockPlacementQueries;

// Answers "nearest currently-active obstacle at-or-before x" and "at-or-after
// x" as obstacles get deactivated one at a time, driving two
// DisjointSetForest instances (this repo's raw Representation-tier parent/rank
// arrays) directly rather than through DisjointSet: DisjointSet.Union decides
// which root survives by rank, and a rank tie attaches the *first* argument's
// root under the *second*'s (DisjointSet.cs's own LinkByRank) - there is no
// way to ask it for "always point coordinate toward coordinate-1" the way
// this deactivation needs. So this writes its own Find, identical in shape to
// DisjointSet.Find (path compression, no union-by-rank), against the same
// forest type, and a Deactivate that always links in the one direction the
// problem needs instead of Union's undirected join. Path compression alone
// (DisjointSet.cs's own complexity-law comment) still gives amortized O(log n)
// Find, which is enough here.
//
// This answers LC 3161 alone - a coordinate can only ever be deactivated in
// the direction its own placement query undoes - which is why it lives beside
// the solution rather than in DataStructures as a second DisjointSet policy.
internal sealed class NearestActiveObstacle
{
    private readonly DisjointSetForest _atOrBefore;
    private readonly DisjointSetForest _atOrAfter;
    private readonly int _rightSentinel;

    // isFinalObstacle[c] is the obstacle layout after every type-1 query has
    // been applied - the state this instance starts from, since queries are
    // then walked in reverse and every type-1 query deactivates one coordinate
    // instead of activating it.
    public NearestActiveObstacle(bool[] isFinalObstacle, int maxCoordinate)
    {
        _rightSentinel = maxCoordinate + 1;
        _atOrBefore = new DisjointSetForest(maxCoordinate + 1);
        _atOrAfter = new DisjointSetForest(maxCoordinate + 2);

        for (var coordinate = 1; coordinate <= maxCoordinate; coordinate++)
        {
            if (!isFinalObstacle[coordinate])
            {
                _atOrBefore.SetParent(coordinate, _atOrBefore.GetParent(coordinate - 1));
            }
        }

        for (var coordinate = maxCoordinate - 1; coordinate >= 0; coordinate--)
        {
            if (!isFinalObstacle[coordinate])
            {
                _atOrAfter.SetParent(coordinate, _atOrAfter.GetParent(coordinate + 1));
            }
        }
    }

    public int NearestAtOrBefore(int coordinate) => Find(_atOrBefore, coordinate);

    public bool TryNearestAtOrAfter(int coordinate, out int nearest)
    {
        nearest = Find(_atOrAfter, coordinate);
        return nearest != _rightSentinel;
    }

    public void Deactivate(int coordinate)
    {
        _atOrBefore.SetParent(coordinate, coordinate - 1);
        _atOrAfter.SetParent(coordinate, coordinate + 1);
    }

    private static int Find(DisjointSetForest forest, int id)
    {
        var root = id;

        while (forest.GetParent(root) != root)
        {
            root = forest.GetParent(root);
        }

        CompressPath(forest, id, root);
        return root;
    }

    private static void CompressPath(DisjointSetForest forest, int id, int root)
    {
        while (forest.GetParent(id) != root)
        {
            var next = forest.GetParent(id);
            forest.SetParent(id, root);
            id = next;
        }
    }
}
