namespace DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

// What the contract suites ask of one configuration of the linked node, with nodes as objects.
internal interface ILinkedNodeView
{
    object Node(int value);

    int ValueOf(object node);

    void SetValue(object node, int value);

    object? NextOf(object node);

    void Link(object node, object? next);

    // Nodes holding 0 to count - 1, each linked to the one after it, and the last linked to the
    // node at cycleEntry, or to none when cycleEntry is null.
    IReadOnlyList<object> Chain(int count, int? cycleEntry);

    bool HasCycle(object? head);

    object? FindCycleStart(object? head);

    // What one node of this configuration takes on the heap, read off the thread's allocation
    // count.
    long BytesPerNode();
}
