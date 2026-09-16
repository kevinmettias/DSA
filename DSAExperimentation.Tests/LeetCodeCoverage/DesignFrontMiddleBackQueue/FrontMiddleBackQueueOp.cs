using DSAExperimentation.LeetCode.DesignFrontMiddleBackQueue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignFrontMiddleBackQueue;

// One call in a FrontMiddleBackQueue script: which operation to invoke and with
// what argument. Pure dispatch, built via the named factories below so a script
// (like Examples in DesignFrontMiddleBackQueueTests) reads like the LeetCode call
// sequence it replays.
public readonly record struct FrontMiddleBackQueueOp(FrontMiddleBackQueueOp.OpKind kind, int value)
{
    public static FrontMiddleBackQueueOp PushFront(int value) => new(OpKind.PushFront, value);

    public static FrontMiddleBackQueueOp PushMiddle(int value) => new(OpKind.PushMiddle, value);

    public static FrontMiddleBackQueueOp PushBack(int value) => new(OpKind.PushBack, value);

    public static FrontMiddleBackQueueOp PopFront() => new(OpKind.PopFront, 0);

    public static FrontMiddleBackQueueOp PopMiddle() => new(OpKind.PopMiddle, 0);

    public static FrontMiddleBackQueueOp PopBack() => new(OpKind.PopBack, 0);

    // null for the three void pushes, the popped value for the three pops - so a
    // script runner can assert against LeetCode's own judge output, which reports
    // the same nulls.
    internal int? Apply(DesignFrontMiddleBackQueueSolution.IFrontMiddleBackQueue queue)
    {
        switch (kind)
        {
            case OpKind.PushFront:
                queue.PushFront(value);
                return null;
            case OpKind.PushMiddle:
                queue.PushMiddle(value);
                return null;
            case OpKind.PushBack:
                queue.PushBack(value);
                return null;
            case OpKind.PopFront:
                return queue.PopFront();
            case OpKind.PopMiddle:
                return queue.PopMiddle();
            default:
                return queue.PopBack();
        }
    }

    public enum OpKind
    {
        PushFront,
        PushMiddle,
        PushBack,
        PopFront,
        PopMiddle,
        PopBack,
    }
}
