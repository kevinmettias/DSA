using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode;

// LeetCode's list with random pointers, as Copy List with Random Pointer's judge writes it: each node as
// [value, index of the node its random pointer names], the index null where it names none. The judge reads a
// copy back the same way, and refuses one that shares a node with the list it copies.
internal static class LeetCodeRandomLists
{
    private const int ValueSlot = 0;
    private const int RandomSlot = 1;

    public static RandomLinkedListNode<int>? From(int?[][] nodes)
    {
        var built = nodes.Select(node => new RandomLinkedListNode<int>(node[ValueSlot] ?? 0)).ToArray();
        var positions = new NodePositions(built);

        for (var index = 0; index < built.Length; index++)
        {
            built[index].Next = built.ElementAtOrDefault(index + 1);
            built[index].Random = positions.NodeAt(nodes[index][RandomSlot]);
        }

        return built.FirstOrDefault();
    }

    public static int?[][] PairsOf(RandomLinkedListNode<int>? head)
    {
        var nodes = NodesFrom(head);
        var positions = new NodePositions(nodes);

        return [.. nodes.Select(node => new int?[] { node.Value, positions.IndexOf(node.Random) })];
    }

    // Whether a copy shares a node with the list it copies, through its next or its random pointers.
    public static bool IsSharingANode(RandomLinkedListNode<int>? copy, RandomLinkedListNode<int>? original)
    {
        var given = new HashSet<RandomLinkedListNode<int>>(NodesFrom(original), ReferenceEqualityComparer.Instance);

        foreach (var node in NodesFrom(copy))
        {
            var isRandomGiven = node.Random is not null && given.Contains(node.Random);

            if (given.Contains(node) || isRandomGiven)
            {
                return true;
            }
        }

        return false;
    }

    private static RandomLinkedListNode<int>[] NodesFrom(RandomLinkedListNode<int>? head)
    {
        var nodes = new List<RandomLinkedListNode<int>>();

        for (var node = head; node is not null; node = node.Next)
        {
            nodes.Add(node);
        }

        return [.. nodes];
    }

    // A list's nodes by their index and back, which is all a random pointer's notation names.
    private sealed class NodePositions
    {
        private readonly RandomLinkedListNode<int>[] _nodes;
        private readonly Dictionary<RandomLinkedListNode<int>, int> _indices = new(ReferenceEqualityComparer.Instance);

        public NodePositions(RandomLinkedListNode<int>[] nodes)
        {
            _nodes = nodes;

            foreach (var node in nodes)
            {
                _indices[node] = _indices.Count;
            }
        }

        public RandomLinkedListNode<int>? NodeAt(int? index)
        {
            if (index is not { } position)
            {
                return null;
            }

            return _nodes[position];
        }

        public int? IndexOf(RandomLinkedListNode<int>? node)
        {
            if (node is null)
            {
                return null;
            }

            return _indices[node];
        }
    }
}
