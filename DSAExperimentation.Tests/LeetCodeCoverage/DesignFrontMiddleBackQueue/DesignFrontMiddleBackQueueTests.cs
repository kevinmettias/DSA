using static DSAExperimentation.LeetCode.DesignFrontMiddleBackQueue.DesignFrontMiddleBackQueueSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignFrontMiddleBackQueue;

// Harness only: the algorithm lives in DesignFrontMiddleBackQueueSolution.
// LeetCode's own shape here is a stateful object across a sequence of calls, so
// Examples encodes a call script instead of a single argument tuple - the same
// shape DesignCircularDequeTests already uses for its own instance-API problem.
// Apply returns int? so a script can state LeetCode's own judge output verbatim,
// nulls for the three void pushes included.
public sealed class DesignFrontMiddleBackQueueTests
{
    public static TheoryData<FrontMiddleBackQueueOp[], int?[]> Examples =>
        new()
        {
            // LeetCode's published example, judge output and all.
            {
                [
                    FrontMiddleBackQueueOp.PushFront(1),   // [1]
                    FrontMiddleBackQueueOp.PushBack(2),    // [1, 2]
                    FrontMiddleBackQueueOp.PushMiddle(3),  // [1, 3, 2]
                    FrontMiddleBackQueueOp.PushMiddle(4),  // [1, 4, 3, 2]
                    FrontMiddleBackQueueOp.PopFront(),     // [4, 3, 2]
                    FrontMiddleBackQueueOp.PopMiddle(),    // [4, 2]
                    FrontMiddleBackQueueOp.PopMiddle(),    // [2]
                    FrontMiddleBackQueueOp.PopBack(),      // []
                    FrontMiddleBackQueueOp.PopFront(),
                ],
                [null, null, null, null, 1, 3, 4, 2, -1]
            },

            // An even-length queue pops the LEFT of its two middle elements.
            {
                [
                    FrontMiddleBackQueueOp.PushBack(1),
                    FrontMiddleBackQueueOp.PushBack(2),
                    FrontMiddleBackQueueOp.PushBack(3),
                    FrontMiddleBackQueueOp.PushBack(4),
                    FrontMiddleBackQueueOp.PopMiddle(),
                    FrontMiddleBackQueueOp.PopMiddle(),
                ],
                [null, null, null, null, 2, 3]
            },

            // A middle push into a one-element queue lands BEFORE it, because
            // index n / 2 is 0 there. The shortest script that pins the rounding.
            {
                [
                    FrontMiddleBackQueueOp.PushFront(1),   // [1]
                    FrontMiddleBackQueueOp.PushMiddle(2),  // [2, 1]
                    FrontMiddleBackQueueOp.PopFront(),
                    FrontMiddleBackQueueOp.PopFront(),
                    FrontMiddleBackQueueOp.PopFront(),
                ],
                [null, null, 2, 1, -1]
            },

            // A front-loaded script: three front pushes before the middle ones, so
            // every middle operation is answered on a queue that grew from the
            // front rather than the back.
            {
                [
                    FrontMiddleBackQueueOp.PushFront(1),   // [1]
                    FrontMiddleBackQueueOp.PushFront(2),   // [2, 1]
                    FrontMiddleBackQueueOp.PushFront(3),   // [3, 2, 1]
                    FrontMiddleBackQueueOp.PushMiddle(4),  // [3, 4, 2, 1]
                    FrontMiddleBackQueueOp.PopMiddle(),    // [3, 2, 1]
                    FrontMiddleBackQueueOp.PopMiddle(),    // [3, 1]
                    FrontMiddleBackQueueOp.PopBack(),      // [3]
                    FrontMiddleBackQueueOp.PopFront(),     // []
                ],
                [null, null, null, null, 4, 2, 1, 3]
            },

            // Every pop on an empty queue reports LeetCode's -1.
            {
                [
                    FrontMiddleBackQueueOp.PopFront(),
                    FrontMiddleBackQueueOp.PopMiddle(),
                    FrontMiddleBackQueueOp.PopBack(),
                ],
                [-1, -1, -1]
            },

            // A middle push into an EMPTY queue, and the middle pop that takes it
            // back out - the degenerate end of both index rules.
            {
                [
                    FrontMiddleBackQueueOp.PushMiddle(5),
                    FrontMiddleBackQueueOp.PopMiddle(),
                    FrontMiddleBackQueueOp.PopMiddle(),
                ],
                [null, 5, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FrontMiddleBackQueueByListInsert_LeetCodeExamples_MatchesExpectedSequence(
        FrontMiddleBackQueueOp[] operations, int?[] expected) =>
        RunScript(new FrontMiddleBackQueueByListInsert(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void FrontMiddleBackQueueByTwoDeques_LeetCodeExamples_MatchesExpectedSequence(
        FrontMiddleBackQueueOp[] operations, int?[] expected) =>
        RunScript(new FrontMiddleBackQueueByTwoDeques(), operations, expected);

    private static void RunScript(
        IFrontMiddleBackQueue queue, FrontMiddleBackQueueOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(queue));
        }
    }
}

// One call in a FrontMiddleBackQueue script: which operation to invoke and with
// what argument. Pure dispatch, built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct FrontMiddleBackQueueOp
{
    private readonly Kind _kind;
    private readonly int _value;

    private FrontMiddleBackQueueOp(Kind kind, int value)
    {
        _kind = kind;
        _value = value;
    }

    public static FrontMiddleBackQueueOp PushFront(int value) => new(Kind.PushFront, value);

    public static FrontMiddleBackQueueOp PushMiddle(int value) => new(Kind.PushMiddle, value);

    public static FrontMiddleBackQueueOp PushBack(int value) => new(Kind.PushBack, value);

    public static FrontMiddleBackQueueOp PopFront() => new(Kind.PopFront, 0);

    public static FrontMiddleBackQueueOp PopMiddle() => new(Kind.PopMiddle, 0);

    public static FrontMiddleBackQueueOp PopBack() => new(Kind.PopBack, 0);

    // null for the three void pushes, the popped value for the three pops - so a
    // script runner can assert against LeetCode's own judge output, which reports
    // the same nulls.
    internal int? Apply(IFrontMiddleBackQueue queue)
    {
        switch (_kind)
        {
            case Kind.PushFront:
                queue.PushFront(_value);
                return null;
            case Kind.PushMiddle:
                queue.PushMiddle(_value);
                return null;
            case Kind.PushBack:
                queue.PushBack(_value);
                return null;
            case Kind.PopFront:
                return queue.PopFront();
            case Kind.PopMiddle:
                return queue.PopMiddle();
            default:
                return queue.PopBack();
        }
    }

    private enum Kind
    {
        PushFront,
        PushMiddle,
        PushBack,
        PopFront,
        PopMiddle,
        PopBack,
    }
}
