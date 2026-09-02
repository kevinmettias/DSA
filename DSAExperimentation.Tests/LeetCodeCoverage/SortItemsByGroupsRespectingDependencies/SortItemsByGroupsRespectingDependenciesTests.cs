using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.SortItemsByGroupsRespectingDependencies.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortItemsByGroupsRespectingDependencies;

// LeetCode 1203. Sort Items by Groups Respecting Dependencies: two applications of
// this repo's own Kahn's-algorithm TopologicalSort.TrySort (the same CourseScheduleII
// composition, run twice) - once over the item graph, once over a derived group
// graph (an edge only crosses into the group graph when an item edge's two endpoints
// sit in different groups). Every ungrouped item (group == -1) is first assigned its
// own singleton group so the same two-level algorithm handles it uniformly. Once both
// orders exist, items are bucketed by group in item-order and the buckets are
// concatenated in group-order - bucketing after an already-valid item order is what
// keeps each group's own internal dependencies satisfied without a third sort.
public sealed partial class SortItemsByGroupsRespectingDependenciesTests
{
    [Fact]
    public void SortItems_LeetCodeExampleWithoutCycle_RespectsPrerequisitesAndGroupContiguity()
    {
        var order = SortItems(
            n: 8, m: 2,
            group: [-1, -1, 1, 0, 0, 1, 0, -1],
            beforeItems: [[], [6], [5], [6], [3, 6], [], [], []]);

        Assert.Equal(8, order.Length);
        var expectedItems = Enumerable.Range(0, 8);
        Assert.Equal(expectedItems, order.OrderBy(item => item));

        var position = order.Select((item, index) => (item, index)).ToDictionary(x => x.item, x => x.index);
        Assert.True(position[6] < position[1]);
        Assert.True(position[5] < position[2]);
        Assert.True(position[6] < position[3]);
        Assert.True(position[3] < position[4]);
        Assert.True(position[6] < position[4]);

        // Group 0 = {3, 4, 6}, group 1 = {2, 5}: each group's items form a
        // contiguous block once a valid group order is chosen.
        AssertContiguous(position, 3, 4, 6);
        AssertContiguous(position, 2, 5);
    }

    [Fact]
    public void SortItems_CyclicPrerequisites_ReturnsEmptyOrder()
    {
        var order = SortItems(
            n: 8, m: 2,
            group: [-1, -1, 1, 0, 0, 1, 0, -1],
            beforeItems: [[], [6], [5], [6], [3], [], [4], []]);

        Assert.Empty(order);
    }

    private static void AssertContiguous(Dictionary<int, int> position, params int[] members)
    {
        var positions = members.Select(member => position[member]).ToArray();
        Assert.Equal(members.Length - 1, positions.Max() - positions.Min());
    }

    private static int[] SortItems(int n, int m, int[] group, List<int>[] beforeItems)
    {
        var (groupIds, groupCount) = AssignSingletonGroups(n, m, group);
        var (items, groups) = BuildNodes(n, groupCount, groupIds);
        AddEdges(items, groups, groupIds, beforeItems);
        return ComputeOrder(items, groups, groupCount);
    }

    private static (int[] GroupIds, int GroupCount) AssignSingletonGroups(int n, int m, int[] group)
    {
        var groupIds = (int[])group.Clone();
        var groupCount = m;
        for (var i = 0; i < n; i++)
        {
            if (groupIds[i] == -1)
            {
                groupIds[i] = groupCount++;
            }
        }

        return (groupIds, groupCount);
    }

    private static (List<ItemNode> Items, List<GroupNode> Groups) BuildNodes(int n, int groupCount, int[] groupIds)
    {
        var items = Enumerable.Range(0, n).Select(id => new ItemNode(id, groupIds[id])).ToList();
        var groups = Enumerable.Range(0, groupCount).Select(id => new GroupNode(id)).ToList();
        return (items, groups);
    }

    private static void AddEdges(List<ItemNode> items, List<GroupNode> groups, int[] groupIds, List<int>[] beforeItems)
    {
        for (var item = 0; item < items.Count; item++)
        {
            foreach (var prerequisite in beforeItems[item])
            {
                items[prerequisite].EnabledItems.Add(items[item]);

                if (groupIds[prerequisite] != groupIds[item])
                {
                    groups[groupIds[prerequisite]].EnabledGroups.Add(groups[groupIds[item]]);
                }
            }
        }
    }

    private static int[] ComputeOrder(List<ItemNode> items, List<GroupNode> groups, int groupCount)
    {
        if (!TopologicalSort.TrySort<
                ItemNode, ItemTopology, ListChildren<ItemNode>,
                NaturalChildOrder<ItemNode, ListChildren<ItemNode>>, ListChildren<ItemNode>>(
                items, out var itemOrder))
        {
            return [];
        }

        if (!TopologicalSort.TrySort<
                GroupNode, GroupTopology, ListChildren<GroupNode>,
                NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>>(
                groups, out var groupOrder))
        {
            return [];
        }

        return Combine(itemOrder, groupOrder, groupCount);
    }

    private static int[] Combine(List<ItemNode> itemOrder, List<GroupNode> groupOrder, int groupCount)
    {
        var buckets = new List<int>[groupCount];
        for (var g = 0; g < groupCount; g++)
        {
            buckets[g] = [];
        }

        foreach (var item in itemOrder)
        {
            buckets[item.GroupId].Add(item.Id);
        }

        var result = new int[itemOrder.Count];
        var index = 0;
        foreach (var groupNode in groupOrder)
        {
            foreach (var itemId in buckets[groupNode.Id])
            {
                result[index++] = itemId;
            }
        }

        return result;
    }
}
