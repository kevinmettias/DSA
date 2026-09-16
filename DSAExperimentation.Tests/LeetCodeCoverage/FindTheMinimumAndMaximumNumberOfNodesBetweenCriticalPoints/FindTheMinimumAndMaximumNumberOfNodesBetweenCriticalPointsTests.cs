using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints;

// Harness only. Both strategies are
// FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution's - including
// the materialized-indices baseline, which the benchmark used to own privately and
// nothing asserted. SinglyLinkedListNode<int> is internal, so it cannot appear in a
// public TheoryData<...> member (CS0053); the examples state the node values and
// each theory builds the chain, the same shape
// ConvertBinaryNumberInALinkedListToIntegerTests uses.
public sealed partial class FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // Fewer than three nodes: no interior node, so no critical point.
            { [3, 1], [-1, -1] },

            // Three critical points at indices 2, 4 and 5.
            { [5, 3, 1, 2, 5, 1, 2], [1, 3] },

            // Exactly two critical points, at indices 1 and 4: min equals max.
            { [1, 3, 2, 2, 3, 2, 2, 2, 7], [3, 3] },

            // Monotonic: every interior node sits strictly between its neighbors.
            { [1, 2, 3, 4, 5], [-1, -1] },

            // Exactly one critical point is still no gap.
            { [1, 3, 2], [-1, -1] },

            // Five consecutive critical points at indices 1..5.
            { [1, 3, 2, 4, 1, 5, 0], [1, 4] },

            // Equal neighbors never make a node critical, so a plateau list has none.
            { [2, 3, 3, 2], [-1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NodesBetweenCriticalPointsByMaterializedIndices_LeetCodeExamples_ReturnsClosestAndFarthestGap(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution
                .NodesBetweenCriticalPointsByMaterializedIndices(BuildList(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NodesBetweenCriticalPointsBySinglePassScan_LeetCodeExamples_ReturnsClosestAndFarthestGap(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution
                .NodesBetweenCriticalPointsBySinglePassScan(BuildList(values)));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        foreach (var value in values[1..])
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return head;
    }
}
