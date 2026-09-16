using DSAExperimentation.LeetCode.DesignCircularDeque;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularDeque;

// One call in a CircularDeque script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples in DesignCircularDequeTests) reads like the LeetCode call sequence it
// replays. Internal, not public: only this same assembly's test method ever calls
// Apply.
public readonly record struct CircularDequeOp(CircularDequeOp.OpKind kind, int value)
{
    public static CircularDequeOp InsertFront(int value) => new(OpKind.InsertFront, value);

    public static CircularDequeOp InsertLast(int value) => new(OpKind.InsertLast, value);

    public static CircularDequeOp DeleteFront() => new(OpKind.DeleteFront, 0);

    public static CircularDequeOp DeleteLast() => new(OpKind.DeleteLast, 0);

    public static CircularDequeOp GetFront() => new(OpKind.GetFront, 0);

    public static CircularDequeOp GetRear() => new(OpKind.GetRear, 0);

    public static CircularDequeOp IsEmpty() => new(OpKind.IsEmpty, 0);

    public static CircularDequeOp IsFull() => new(OpKind.IsFull, 0);

    // 1/0 for the four bool-returning operations, the actual value for
    // GetFront/GetRear - so a script runner can assert against one expected value
    // per operation uniformly.
    internal int Apply(DesignCircularDequeSolution.ICircularDeque deque) => kind switch
    {
        OpKind.InsertFront => deque.InsertFront(value) ? 1 : 0,
        OpKind.InsertLast => deque.InsertLast(value) ? 1 : 0,
        OpKind.DeleteFront => deque.DeleteFront() ? 1 : 0,
        OpKind.DeleteLast => deque.DeleteLast() ? 1 : 0,
        OpKind.GetFront => deque.GetFront(),
        OpKind.GetRear => deque.GetRear(),
        OpKind.IsEmpty => deque.IsEmpty() ? 1 : 0,
        _ => deque.IsFull() ? 1 : 0,
    };

    public enum OpKind
    {
        InsertFront,
        InsertLast,
        DeleteFront,
        DeleteLast,
        GetFront,
        GetRear,
        IsEmpty,
        IsFull,
    }
}
