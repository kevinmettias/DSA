using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.BlockPlacementQueries;

// Answers "nearest currently-active obstacle at-or-before x" and "at-or-after x" as obstacles get
// deactivated one at a time, with two DisjointSets whose representatives ARE the answers: every
// coordinate that is not an active obstacle is merged into its neighbour on one side, so Find walks to
// the nearest active obstacle on that side. DisjointSet.MergeInto is what makes that readable - it
// promises the target's root survives, where Union's root is whichever rank chose, and a deactivation
// is exactly "merge this coordinate into its neighbour". Path compression alone (DisjointSet.cs's
// complexity-law comment) gives amortized O(log n) Find, which is enough here.
//
// What is LC 3161's own is the layout: coordinate 0 is always an obstacle (ObstaclePresence marks it),
// so the at-or-before set needs no sentinel, while the at-or-after set carries one past the largest
// coordinate, which reads as "no obstacle to the right".
internal sealed class NearestActiveObstacle
{
    private readonly DisjointSet _atOrBefore;
    private readonly DisjointSet _atOrAfter;
    private readonly int _rightSentinel;

    // isFinalObstacle[c] is the obstacle layout after every type-1 query has
    // been applied - the state this instance starts from, since queries are
    // then walked in reverse and every type-1 query deactivates one coordinate
    // instead of activating it.
    public NearestActiveObstacle(bool[] isFinalObstacle, int maxCoordinate)
    {
        _rightSentinel = maxCoordinate + 1;
        _atOrBefore = new DisjointSet(maxCoordinate + 1);
        _atOrAfter = new DisjointSet(maxCoordinate + 2);

        for (var coordinate = 1; coordinate <= maxCoordinate; coordinate++)
        {
            if (!isFinalObstacle[coordinate])
            {
                _atOrBefore.MergeInto(coordinate, coordinate - 1);
            }
        }

        for (var coordinate = maxCoordinate - 1; coordinate >= 0; coordinate--)
        {
            if (!isFinalObstacle[coordinate])
            {
                _atOrAfter.MergeInto(coordinate, coordinate + 1);
            }
        }
    }

    public int NearestAtOrBefore(int coordinate) => _atOrBefore.Find(coordinate);

    public bool TryNearestAtOrAfter(int coordinate, out int nearest)
    {
        nearest = _atOrAfter.Find(coordinate);
        return nearest != _rightSentinel;
    }

    public void Deactivate(int coordinate)
    {
        _atOrBefore.MergeInto(coordinate, coordinate - 1);
        _atOrAfter.MergeInto(coordinate, coordinate + 1);
    }
}
