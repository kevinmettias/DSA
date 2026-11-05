namespace DSAExperimentation.LeetCode.Solutions.SortItemsByGroupsRespectingDependencies;

// LC 1203 accepts any order of the items 0 to n - 1, each once, that keeps the items of each group next to
// each other and puts every item after the items its beforeItems lists ("Return any solution if there is
// more than one solution"), and the empty list only when no such order exists. An item of group -1 belongs
// to no group, so it need stand beside nothing. The catalog records one order, so an answer stands when it is
// empty exactly where the recorded one is, or else such an order. Kahn's sort and the naive rescan break ties
// between ready items, and between ready groups, differently, and neither always as the catalog did.
internal static class SortItemsByGroupsRespectingDependenciesAnswerCheck
{
    public static bool IsAccepted(int itemCount, int declaredGroupCount, int[] group, int[][] beforeItems, int[] answer, int[] recorded)
    {
        if (answer.Length == 0 || recorded.Length == 0)
        {
            return answer.Length == recorded.Length;
        }

        if (PositionsOf(itemCount, answer) is not { } positions)
        {
            return false;
        }

        return IsEveryDependencyBefore(beforeItems, positions) && IsEachGroupOneBlock(group, positions);
    }

    // Where each item stands in the order, indexed by the item; null unless the order takes each item from 0
    // to n - 1 exactly once.
    private static int[]? PositionsOf(int itemCount, int[] order)
    {
        var everyItem = Enumerable.Range(0, itemCount);

        if (!order.Order().SequenceEqual(everyItem))
        {
            return null;
        }

        var positions = new int[itemCount];

        for (var position = 0; position < order.Length; position++)
        {
            positions[order[position]] = position;
        }

        return positions;
    }

    // Whether every item stands after each item its beforeItems lists.
    private static bool IsEveryDependencyBefore(int[][] beforeItems, int[] positions)
        => Enumerable.Range(0, beforeItems.Length).All(item => beforeItems[item].All(before => positions[before] < positions[item]));

    // Whether the items of each group fill one unbroken stretch of the order. Groups are numbered from 0, so an
    // item numbered below that belongs to none and is in no stretch.
    private static bool IsEachGroupOneBlock(int[] group, int[] positions)
    {
        var groupedItems = Enumerable.Range(0, group.Length).Where(item => group[item] >= 0);

        return groupedItems.GroupBy(item => group[item]).All(members => IsOneBlock(members, positions));
    }

    // Whether the items stand side by side: as many places from the first of them to the last as there are
    // items, each item having a place of its own.
    private static bool IsOneBlock(IEnumerable<int> items, int[] positions)
    {
        var places = items.Select(item => positions[item]).ToList();

        return places.Max() - places.Min() + 1 == places.Count;
    }
}
