using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.CycleLengthQueriesInATree;

// LeetCode 2509. Cycle Length Queries in a Tree: node ids 1..2^n-1 pack a complete
// binary tree exactly the way Heap's own backing array does (parent(id) = id/2,
// children 2*id and 2*id+1), just 1-indexed here instead of Heap's 0-indexed
// convention. Adding edge (a, b) to a tree creates exactly one cycle, whose length
// is 1 + the tree distance between a and b, so every query is a lowest-common-
// ancestor walk and no tree is ever materialized.
//
// n only bounds the id range the queries may use - the walk reads each id's depth
// out of the id itself - so neither strategy consults it; it stays on the signature
// because that is LeetCode's own shape.
internal static class CycleLengthQueriesInATreeSolution
{
    // Ids are 1-based, so id 1 is the root and id - 1 is the same node's 0-indexed
    // heap array position.
    private const int RootId = 1;

    // The edge the query itself adds, which closes the path into a cycle.
    private const int ClosingEdge = 1;

    // A complete binary tree halves an id to reach its parent.
    private const int ChildrenPerNode = 2;

    // Lifting both endpoints one level covers one edge on each side of the path.
    private const int EdgesPerSharedLift = 2;

    // The textbook answer: for each query record every ancestor of a in a freshly
    // allocated Dictionary keyed by node id, then walk b up until it lands on one of
    // them. Deliberately BCL-only - a Dictionary per query is what you write without
    // this repo, and it is the arm the walk below has to justify itself against.
    public static int[] CycleLengthQueriesByAncestorDictionary(int n, int[][] queries)
    {
        var lengths = new int[queries.Length];

        for (var index = 0; index < queries.Length; index++)
        {
            lengths[index] = CycleLengthByAncestorDictionary(queries[index][0], queries[index][1]);
        }

        return lengths;
    }

    private static int CycleLengthByAncestorDictionary(int a, int b)
    {
        var distanceFromA = new Dictionary<int, int>();
        var depth = 0;
        var current = a;

        while (current >= RootId)
        {
            distanceFromA[current] = depth;
            current /= ChildrenPerNode;
            depth++;
        }

        var distanceFromB = 0;
        current = b;

        while (!distanceFromA.ContainsKey(current))
        {
            current /= ChildrenPerNode;
            distanceFromB++;
        }

        return distanceFromA[current] + distanceFromB + ClosingEdge;
    }

    // This repo's own complete-binary-tree arithmetic: shifting each id down by one
    // turns it into the 0-indexed position HeapArrayIndex.Parent understands
    // ((index - 1) / 2 on a 0-indexed id is the same walk as id / 2 on the 1-indexed
    // original), so the whole query is the two-pointer LCA walk - lift the deeper
    // endpoint to the shallower one's depth, then lift both together - touching two
    // ints and allocating nothing per query.
    public static int[] CycleLengthQueriesByParentIndexWalk(int n, int[][] queries)
    {
        var lengths = new int[queries.Length];

        for (var index = 0; index < queries.Length; index++)
        {
            lengths[index] = CycleLengthByParentIndexWalk(queries[index][0], queries[index][1]);
        }

        return lengths;
    }

    private static int CycleLengthByParentIndexWalk(int a, int b)
    {
        var indexA = a - RootId;
        var indexB = b - RootId;
        var depthA = Depth(indexA);
        var depthB = Depth(indexB);

        var (leveledA, leveledB, distance) = LiftDeeperSide(indexA, depthA, indexB, depthB);

        while (leveledA != leveledB)
        {
            leveledA = HeapArrayIndex.Parent(leveledA);
            leveledB = HeapArrayIndex.Parent(leveledB);
            distance += EdgesPerSharedLift;
        }

        return distance + ClosingEdge;
    }

    private static int Depth(int index)
    {
        var depth = 0;

        while (index > 0)
        {
            index = HeapArrayIndex.Parent(index);
            depth++;
        }

        return depth;
    }

    // Lifts whichever of the two endpoints is deeper until both sit at the same
    // level, charging one edge per lift, and reports the leveled ids alongside what
    // that half of the walk cost.
    private static (int IndexA, int IndexB, int Distance) LiftDeeperSide(
        int indexA, int depthA, int indexB, int depthB)
    {
        var distance = 0;

        while (depthA > depthB)
        {
            indexA = HeapArrayIndex.Parent(indexA);
            depthA--;
            distance++;
        }

        while (depthB > depthA)
        {
            indexB = HeapArrayIndex.Parent(indexB);
            depthB--;
            distance++;
        }

        return (indexA, indexB, distance);
    }
}
