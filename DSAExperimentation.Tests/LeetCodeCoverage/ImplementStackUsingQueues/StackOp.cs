using DSAExperimentation.LeetCode.ImplementStackUsingQueues;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementStackUsingQueues;

// One call in a MyStack script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script
// (like ImplementStackUsingQueuesTests.Examples) reads like the LeetCode call
// sequence it replays. Its own file because ImplementStackUsingQueuesTests'
// public Theory signatures name it, so it cannot be a nested private helper -
// and a second file-level type beside the test class would leave the file's
// name identifying neither.
public readonly record struct StackOp(StackOp.OpKind kind, int value)
{
    public static StackOp Push(int value) => new(OpKind.Push, value);

    public static StackOp Pop() => new(OpKind.Pop, 0);

    public static StackOp Top() => new(OpKind.Top, 0);

    public static StackOp Empty() => new(OpKind.Empty, 0);

    // null for push, the int result for pop/top, the bool result for empty -
    // so a script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IStackOperations is internal to
    // ImplementStackUsingQueuesSolution, and only this same assembly's
    // ImplementStackUsingQueuesTests.RunScript ever calls Apply.
    internal object? Apply(ImplementStackUsingQueuesSolution.IStackOperations stack)
    {
        switch (kind)
        {
            case OpKind.Push:
                stack.Push(value);
                return null;
            case OpKind.Pop:
                return stack.Pop();
            case OpKind.Top:
                return stack.Top();
            default:
                return stack.Empty();
        }
    }

    public enum OpKind
    {
        Push,
        Pop,
        Top,
        Empty,
    }
}
