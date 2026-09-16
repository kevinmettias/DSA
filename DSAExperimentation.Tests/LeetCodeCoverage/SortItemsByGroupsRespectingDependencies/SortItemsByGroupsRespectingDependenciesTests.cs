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
public sealed partial class SortItemsByGroupsRespectingDependenciesTests
{
    public static TheoryData<OrderExample> Examples =>
        new()
        {
            {
                new OrderExample(
                    N: 8, M: 2, Group: [-1, -1, 1, 0, 0, 1, 0, -1],
                    BeforeItems: [[], [6], [5], [6], [3, 6], [], [], []], OrderExists: true)
            },
            {
                new OrderExample(
                    N: 8, M: 2, Group: [-1, -1, 1, 0, 0, 1, 0, -1],
                    BeforeItems: [[], [6], [5], [6], [3], [], [4], []], OrderExists: false)
            },
            { new OrderExample(N: 3, M: 1, Group: [0, 0, 0], BeforeItems: [[], [0], [1]], OrderExists: true) },
            { new OrderExample(N: 4, M: 2, Group: [0, 0, 1, 1], BeforeItems: [[], [0], [1], [2]], OrderExists: true) },
            { new OrderExample(N: 4, M: 2, Group: [0, 1, 1, 0], BeforeItems: [[], [0], [], [2]], OrderExists: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortItemsByKahnsTopologicalSort_LeetCodeExamples_OrdersItemsInContiguousGroupBlocks(
        OrderExample example)
    {
        var order = SortItemsByGroupsRespectingDependenciesSolution.SortItemsByKahnsTopologicalSort(
            example.N, example.M, example.Group, example.BeforeItems);

        AssertOrder(order, example);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortItemsByNaiveRescan_LeetCodeExamples_OrdersItemsInContiguousGroupBlocks(
        OrderExample example)
    {
        var order = SortItemsByGroupsRespectingDependenciesSolution.SortItemsByNaiveRescan(
            example.N, example.M, example.Group, example.BeforeItems);

        AssertOrder(order, example);
    }

    private static void AssertOrder(int[] order, OrderExample example)
    {
        if (!example.OrderExists)
        {
            Assert.Empty(order);
            return;
        }

        var expectedItems = Enumerable.Range(0, example.N);

        Assert.Equal(expectedItems, order.OrderBy(item => item));

        var position = order
            .Select((item, index) => (Item: item, Index: index))
            .ToDictionary(placed => placed.Item, placed => placed.Index);

        AssertPrerequisitesPrecedeDependents(position, example.BeforeItems);
        AssertGroupsAreContiguous(position, example.M, example.Group);
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

    private static void AssertGroupsAreContiguous(Dictionary<int, int> position, int groupCount, int[] group)
    {
        for (var groupId = 0; groupId < groupCount; groupId++)
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

    // One LeetCode example: the item and group counts, each item's group assignment (-1 for
    // none), the beforeItems constraints, and whether any valid ordering exists at all. The
    // five travel together into every assertion, so each is named rather than left as a
    // position in a row of five literals.
    public readonly record struct OrderExample(
        int N,
        int M,
        int[] Group,
        int[][] BeforeItems,
        bool OrderExists);
}
