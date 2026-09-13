using DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortItemsByGroupsRespectingDependencies;

// Harness only. Both two-level topological strategies are
// SortItemsByGroupsRespectingDependenciesSolution's - this file pins them to
// LeetCode's published examples plus a group-level-only cycle (an acyclic item graph
// whose derived group graph is not), which is the case a single item-level sort would
// silently answer wrong.
//
// A valid ordering is not unique, so the assertion checks the properties LeetCode
// actually asks for rather than one arbitrary permutation: every item appears once,
// every beforeItems constraint holds, and each group's items sit in one contiguous
// block. An empty result means "no valid ordering exists".
public sealed class SortItemsByGroupsRespectingDependenciesTests
{
    public static TheoryData<int, int, int[], int[][], bool> Examples =>
        new()
        {
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3, 6], [], [], []], true },
            { 8, 2, [-1, -1, 1, 0, 0, 1, 0, -1], [[], [6], [5], [6], [3], [], [4], []], false },
            { 3, 1, [0, 0, 0], [[], [0], [1]], true },
            { 4, 2, [0, 0, 1, 1], [[], [0], [1], [2]], true },
            { 4, 2, [0, 1, 1, 0], [[], [0], [], [2]], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortItemsByKahnsTopologicalSort_LeetCodeExamples_OrdersItemsInContiguousGroupBlocks(
        int n, int m, int[] group, int[][] beforeItems, bool orderExists) =>
        AssertOrder(
            SortItemsByGroupsRespectingDependenciesSolution.SortItemsByKahnsTopologicalSort(n, m, group, beforeItems),
            n, m, group, beforeItems, orderExists);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortItemsByNaiveRescan_LeetCodeExamples_OrdersItemsInContiguousGroupBlocks(
        int n, int m, int[] group, int[][] beforeItems, bool orderExists) =>
        AssertOrder(
            SortItemsByGroupsRespectingDependenciesSolution.SortItemsByNaiveRescan(n, m, group, beforeItems),
            n, m, group, beforeItems, orderExists);

    private static void AssertOrder(int[] order, int n, int m, int[] group, int[][] beforeItems, bool orderExists)
    {
        if (!orderExists)
        {
            Assert.Empty(order);
            return;
        }

        Assert.Equal(Enumerable.Range(0, n), order.OrderBy(item => item));

        var position = order
            .Select((item, index) => (Item: item, Index: index))
            .ToDictionary(placed => placed.Item, placed => placed.Index);

        AssertPrerequisitesPrecedeDependents(position, beforeItems);
        AssertGroupsAreContiguous(position, m, group);
    }

    private static void AssertPrerequisitesPrecedeDependents(
        Dictionary<int, int> position, int[][] beforeItems)
    {
        for (var item = 0; item < beforeItems.Length; item++)
        {
            foreach (var prerequisite in beforeItems[item])
            {
                Assert.True(position[prerequisite] < position[item]);
            }
        }
    }

    private static void AssertGroupsAreContiguous(Dictionary<int, int> position, int m, int[] group)
    {
        for (var groupId = 0; groupId < m; groupId++)
        {
            var positions = group
                .Select((assigned, item) => (Assigned: assigned, Item: item))
                .Where(membership => membership.Assigned == groupId)
                .Select(membership => position[membership.Item])
                .ToArray();

            if (positions.Length > 0)
            {
                Assert.Equal(positions.Length - 1, positions.Max() - positions.Min());
            }
        }
    }
}
