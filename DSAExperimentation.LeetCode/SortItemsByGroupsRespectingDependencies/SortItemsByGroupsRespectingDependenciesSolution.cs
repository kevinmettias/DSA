using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

// LeetCode 1203. Sort Items by Groups Respecting Dependencies: order every item so
// that each item follows all of its beforeItems, and so that the items of any one
// group form a contiguous block. Two topological sorts answer it - once over the item
// graph, once over a derived group graph whose edges are exactly the item edges whose
// two endpoints sit in different groups. Every ungrouped item (group == -1) is first
// given its own singleton group so the same two-level algorithm handles it uniformly.
//
// Bucketing the already-valid item order by group and concatenating the buckets in
// group order is what keeps each group's internal dependencies satisfied without a
// third sort. An empty array signals "no valid ordering", LeetCode's own convention
// for a cycle at either level.
//
// The naive baseline rescans every remaining node for one with zero remaining
// prerequisites on each step, O(V^2 + V*E) per level; TopologicalSort.TrySort is
// Kahn's algorithm proper, O(V+E) via a queue of already-zero-in-degree nodes - the
// same pairing CourseScheduleII (LC 210) measures, run twice.
internal static class SortItemsByGroupsRespectingDependenciesSolution
{
    // LeetCode's own "this item has no group" marker.
    private const int Ungrouped = -1;

    // This repo's own Kahn's-algorithm TopologicalSort.TrySort, run over the item
    // graph and then over the derived group graph.
    public static int[] SortItemsByKahnsTopologicalSort(
        int itemCount, int declaredGroupCount, int[] group, int[][] beforeItems)
    {
        var (items, groups) = BuildGraph(itemCount, declaredGroupCount, group, beforeItems);

        return SortItemsByKahnsTopologicalSort(items, groups);
    }

    public static int[] SortItemsByKahnsTopologicalSort(List<ItemNode> items, List<GroupNode> groups)
    {
        var itemsSorted = TopologicalSort.TrySort<
            ItemNode, ItemTopology, ListChildren<ItemNode>,
            NaturalChildOrder<ItemNode, ListChildren<ItemNode>>, ListChildren<ItemNode>>(
            items, out var itemOrder);

        var groupsSorted = TopologicalSort.TrySort<
            GroupNode, GroupTopology, ListChildren<GroupNode>,
            NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>>(
            groups, out var groupOrder);

        if (!itemsSorted || !groupsSorted)
        {
            return [];
        }

        return Combine(itemOrder, groupOrder, groups.Count);
    }

    // The textbook answer: rescan the remaining nodes from scratch on every step
    // looking for the next one with no unresolved prerequisites, rather than tracking
    // a frontier queue. Deliberately written without this repo's TopologicalSort - it
    // is the arm the composed solution above has to justify itself against.
    public static int[] SortItemsByNaiveRescan(
        int itemCount, int declaredGroupCount, int[] group, int[][] beforeItems)
    {
        var (items, groups) = BuildGraph(itemCount, declaredGroupCount, group, beforeItems);

        return SortItemsByNaiveRescan(items, groups);
    }

    public static int[] SortItemsByNaiveRescan(List<ItemNode> items, List<GroupNode> groups)
    {
        var itemOrder = NaiveRescanOrder(items, item => item.EnabledItems);
        var groupOrder = NaiveRescanOrder(groups, group => group.EnabledGroups);

        if (itemOrder.Count != items.Count || groupOrder.Count != groups.Count)
        {
            return [];
        }

        return Combine(itemOrder, groupOrder, groups.Count);
    }

    private static List<TNode> NaiveRescanOrder<TNode>(List<TNode> nodes, Func<TNode, List<TNode>> children)
        where TNode : class
    {
        var inDegree = nodes.ToDictionary(node => node, _ => 0);

        foreach (var node in nodes)
        {
            foreach (var next in children(node))
            {
                inDegree[next]++;
            }
        }

        var remaining = new List<TNode>(nodes);
        var order = new List<TNode>(nodes.Count);

        while (remaining.Count > 0 && TryTakeNextReadyNode(remaining, order, inDegree, children))
        {
        }

        return order;
    }

    private static bool TryTakeNextReadyNode<TNode>(
        List<TNode> remaining, List<TNode> order, Dictionary<TNode, int> inDegree, Func<TNode, List<TNode>> children)
        where TNode : class
    {
        var next = remaining.FirstOrDefault(node => inDegree[node] == 0);

        if (next is null)
        {
            return false;
        }

        order.Add(next);
        remaining.Remove(next);

        foreach (var dependent in children(next))
        {
            inDegree[dependent]--;
        }

        return true;
    }

    // Buckets an already-valid item order by group, then concatenates the buckets in
    // group order - each bucket keeps its items' relative item-order, so no group's
    // internal dependencies are disturbed by the regrouping.
    private static int[] Combine(List<ItemNode> itemOrder, List<GroupNode> groupOrder, int groupCount)
    {
        var buckets = BucketItemIdsByGroup(itemOrder, groupCount);

        return ConcatenateBuckets(buckets, groupOrder, itemOrder.Count);
    }

    // One bucket of item ids per group, each bucket holding its items in the order the
    // valid item order gave them.
    private static List<int>[] BucketItemIdsByGroup(List<ItemNode> itemOrder, int groupCount)
    {
        var buckets = new List<int>[groupCount];

        for (var groupId = 0; groupId < groupCount; groupId++)
        {
            buckets[groupId] = [];
        }

        foreach (var item in itemOrder)
        {
            buckets[item.GroupId].Add(item.Id);
        }

        return buckets;
    }

    // The buckets laid end to end in group order, which is the ordering Combine returns.
    private static int[] ConcatenateBuckets(List<int>[] buckets, List<GroupNode> groupOrder, int itemCount)
    {
        var result = new int[itemCount];
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

    private static (List<ItemNode> Items, List<GroupNode> Groups) BuildGraph(
        int itemCount, int declaredGroupCount, int[] group, int[][] beforeItems)
    {
        var (groupIds, groupCount) = AssignSingletonGroups(itemCount, declaredGroupCount, group);
        var items = Enumerable.Range(0, itemCount).Select(id => new ItemNode(id, groupIds[id])).ToList();
        var groups = Enumerable.Range(0, groupCount).Select(id => new GroupNode(id)).ToList();

        AddEdges(items, groups, groupIds, beforeItems);

        return (items, groups);
    }

    // Each ungrouped item becomes its own group, so an item that belongs to nobody is
    // trivially contiguous and the group level needs no special case for it.
    private static (int[] GroupIds, int GroupCount) AssignSingletonGroups(
        int itemCount, int declaredGroupCount, int[] group)
    {
        var groupIds = (int[])group.Clone();
        var groupCount = declaredGroupCount;

        for (var item = 0; item < itemCount; item++)
        {
            if (groupIds[item] == Ungrouped)
            {
                groupIds[item] = groupCount++;
            }
        }

        return (groupIds, groupCount);
    }

    private static void AddEdges(
        List<ItemNode> items, List<GroupNode> groups, int[] groupIds, int[][] beforeItems)
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
}
