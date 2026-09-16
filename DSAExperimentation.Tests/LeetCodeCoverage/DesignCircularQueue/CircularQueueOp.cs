using DSAExperimentation.LeetCode.DesignCircularQueue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularQueue;

// One call in a CircularQueue script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples in DesignCircularQueueTests) reads like the LeetCode call sequence it
// replays. Internal, not public: only this same assembly's test method ever calls
// Apply.
public readonly record struct CircularQueueOp(CircularQueueOp.OpKind kind, int value)
{
    public static CircularQueueOp EnQueue(int value) => new(OpKind.EnQueue, value);

    public static CircularQueueOp DeQueue() => new(OpKind.DeQueue, 0);

    public static CircularQueueOp Front() => new(OpKind.Front, 0);

    public static CircularQueueOp Rear() => new(OpKind.Rear, 0);

    public static CircularQueueOp IsEmpty() => new(OpKind.IsEmpty, 0);

    public static CircularQueueOp IsFull() => new(OpKind.IsFull, 0);

    // 1/0 for the four bool-returning operations, the actual value for
    // Front/Rear - so a script runner can assert against one expected value per
    // operation uniformly.
    internal int Apply(DesignCircularQueueSolution.ICircularQueue queue) => kind switch
    {
        OpKind.EnQueue => queue.EnQueue(value) ? 1 : 0,
        OpKind.DeQueue => queue.DeQueue() ? 1 : 0,
        OpKind.Front => queue.Front(),
        OpKind.Rear => queue.Rear(),
        OpKind.IsEmpty => queue.IsEmpty() ? 1 : 0,
        _ => queue.IsFull() ? 1 : 0,
    };

    public enum OpKind
    {
        EnQueue,
        DeQueue,
        Front,
        Rear,
        IsEmpty,
        IsFull,
    }
}
