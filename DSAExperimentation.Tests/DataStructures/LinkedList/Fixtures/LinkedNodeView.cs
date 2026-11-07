using DSAExperimentation.DataStructures.LinkedList;

namespace DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

// One configuration behind the view: every member casts its objects back to the node type and
// calls the node template or CycleDetection with the configuration's own type arguments.
internal sealed class LinkedNodeView<TNode>(Func<int, TNode> make) : ILinkedNodeView
    where TNode : LinkedNode<TNode, int>
{
    public object Node(int value) => make(value);

    public int ValueOf(object node) => ((TNode)node).Value;

    public void SetValue(object node, int value) => ((TNode)node).Value = value;

    public object? NextOf(object node) => ((TNode)node).Next;

    public void Link(object node, object? next) => ((TNode)node).Next = (TNode?)next;

    public IReadOnlyList<object> Chain(int count, int? cycleEntry)
    {
        var nodes = Enumerable.Range(0, count).Select(make).ToArray();

        for (var index = 0; index + 1 < count; index++)
        {
            nodes[index].Next = nodes[index + 1];
        }

        if (cycleEntry is { } entry)
        {
            nodes[^1].Next = nodes[entry];
        }

        return nodes;
    }

    public bool HasCycle(object? head) => CycleDetection.HasCycle<TNode, int>((TNode?)head);

    public object? FindCycleStart(object? head) =>
        CycleDetection.FindCycleStart<TNode, int>((TNode?)head);

    public long BytesPerNode() => Allocations.BytesOf(() => make(0));
}
