namespace DSAExperimentation.LeetCode.Solutions.FlattenAMultilevelDoublyLinkedList;

// LC 430's list as its judge writes it, level by level: a level's values, then a null to end it, then one more
// null for each node of that level the next level's head does not hang from, so the next level is the child
// of the node at that position. The judge reads the flattened list back as its values in order, and only a
// list whose every node points back at the one before it and has no child left is flat.
internal static class MultilevelListNotation
{
    public static Node? From(int?[] levels)
    {
        var position = 0;
        var level = ReadLevel(levels, ref position);
        var head = level.FirstOrDefault();

        while (position < levels.Length)
        {
            var offset = CountNulls(levels, ref position);
            var next = ReadLevel(levels, ref position);
            level[offset].Child = next.FirstOrDefault();
            level = next;
        }

        return head;
    }

    // The values of a flat list in order, or none when a node still has a child or does not point back at
    // the node before it.
    public static int[] ValuesOf(Node? head)
    {
        var values = new List<int>();

        for (Node? previous = null, node = head; node is not null; previous = node, node = node.Next)
        {
            if (node.Child is not null || node.Previous != previous)
            {
                return [];
            }

            values.Add(node.Value);
        }

        return [.. values];
    }

    // One level's nodes, linked both ways, read up to the null that ends it, which is passed over.
    private static List<Node> ReadLevel(int?[] levels, ref int position)
    {
        var nodes = new List<Node>();

        for (; position < levels.Length && levels[position] is { } value; position++)
        {
            var node = new Node(value) { Previous = nodes.LastOrDefault() };
            nodes.LastOrDefault()?.Next = node;
            nodes.Add(node);
        }

        position++;
        return nodes;
    }

    private static int CountNulls(int?[] levels, ref int position)
    {
        var count = 0;

        for (; position < levels.Length && levels[position] is null; position++)
        {
            count++;
        }

        return count;
    }
}
