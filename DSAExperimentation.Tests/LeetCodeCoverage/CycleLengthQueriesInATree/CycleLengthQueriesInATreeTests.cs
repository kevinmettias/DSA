using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CycleLengthQueriesInATree;

// LeetCode 2509. Cycle Length Queries in a Tree: node ids 1..2^n-1 pack a
// complete binary tree exactly the way Heap's own backing array does
// (parent(id) = id/2, children 2*id and 2*id+1) - just 1-indexed here instead of
// Heap's 0-indexed convention, so shifting every id down by one first makes
// HeapArrayIndex.Parent directly reusable ((index-1)/2 on a 0-indexed id is the
// same walk as id/2 on the 1-indexed original - see this suite's own derivation
// in code review). Adding edge (a,b) to a tree creates exactly one cycle, whose
// length is 1 + the tree distance between a and b; distance is the textbook
// walk-the-deeper-node-up-then-both-up-together-to-the-LCA, using nothing but
// that same Parent arithmetic at every step - no tree is ever materialized, the
// same "index arithmetic over a complete binary tree" representation
// ARCHITECTURE.md's Collections/Heap section already documents.
public sealed partial class CycleLengthQueriesInATreeTests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            // LeetCode's own published example.
            { 3, new[] { new[] { 5, 3 }, new[] { 4, 7 }, new[] { 2, 3 } }, [4, 5, 3] },
            // n=2 (nodes 1,2,3): adding edge 2-3 closes the triangle 2-1-3-2.
            { 2, new[] { new[] { 2, 3 } }, [3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CycleLengthQueries_LeetCodeExamples_ReturnsPerQueryCycleLengths(
        int n, int[][] queries, int[] expected)
    {
        Assert.Equal(expected, CycleLengthQueries(n, queries));
    }

    private static int[] CycleLengthQueries(int n, int[][] queries)
        => queries.Select(query => CycleLength(query[0], query[1])).ToArray();

    private static int CycleLength(int a, int b)
    {
        var indexA = a - 1;
        var indexB = b - 1;
        var depthA = Depth(indexA);
        var depthB = Depth(indexB);
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

        while (indexA != indexB)
        {
            indexA = HeapArrayIndex.Parent(indexA);
            indexB = HeapArrayIndex.Parent(indexB);
            distance += 2;
        }

        return distance + 1;
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
}
