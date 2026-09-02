using DSAExperimentation.LeetCode.PeekingIterator;
using static DSAExperimentation.LeetCode.PeekingIterator.PeekingIteratorSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PeekingIterator;

// Harness only. Both strategies are PeekingIteratorSolution's - this file
// replays LeetCode's published call sequences against each IPeekingIterator
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a sequence of calls
// rather than a single argument tuple. PeekOp.Apply is pure dispatch (which
// method to call) - no buffering logic of its own.
public sealed class PeekingIteratorTests
{
    public static TheoryData<int[], PeekOp[], object?[]> Examples =>
        new()
        {
            // LeetCode's own example: peek(), next(), next(), hasNext(), peek(),
            // next(), hasNext().
            {
                [1, 2, 3],
                [PeekOp.Peek(), PeekOp.Next(), PeekOp.Next(), PeekOp.HasNext(), PeekOp.Peek(), PeekOp.Next(), PeekOp.HasNext()],
                [1, 1, 2, true, 3, 3, false]
            },
            // Repeated peeks must not advance the underlying sequence.
            {
                [5],
                [PeekOp.Peek(), PeekOp.Peek(), PeekOp.HasNext(), PeekOp.Next(), PeekOp.HasNext()],
                [5, 5, true, 5, false]
            },
            // Pure next()-driven exhaustion, with no peek() calls at all.
            {
                [10, 20],
                [PeekOp.HasNext(), PeekOp.Next(), PeekOp.HasNext(), PeekOp.Next(), PeekOp.HasNext()],
                [true, 10, true, 20, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIndexTracked_LeetCodeExamples_InterleavesCorrectly(
        int[] source, PeekOp[] operations, object?[] expected) =>
        RunScript(CreateByIndexTracked(source), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByQueuePrimitive_LeetCodeExamples_InterleavesCorrectly(
        int[] source, PeekOp[] operations, object?[] expected) =>
        RunScript(CreateByQueuePrimitive(source), operations, expected);

    private static void RunScript(IPeekingIterator iterator, PeekOp[] operations, object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(iterator));
        }
    }
}

// One call in a peeking-iterator script: which operation to invoke. Pure
// dispatch, built via the named factories below so a script (like Examples
// above) reads like the LeetCode call sequence it replays.
public readonly record struct PeekOp
{
    private readonly Kind _kind;

    private PeekOp(Kind kind) => _kind = kind;

    public static PeekOp HasNext() => new(Kind.HasNext);

    public static PeekOp Peek() => new(Kind.Peek);

    public static PeekOp Next() => new(Kind.Next);

    // The bool result for hasNext, the int result for peek/next - so a script
    // runner can assert against one expected value per operation uniformly.
    // Internal, not public: IPeekingIterator is internal, and only this same
    // assembly's RunScript ever calls Apply.
    internal object? Apply(IPeekingIterator iterator)
    {
        switch (_kind)
        {
            case Kind.HasNext:
                return iterator.HasNext();
            case Kind.Peek:
                return iterator.Peek();
            default:
                return iterator.Next();
        }
    }

    private enum Kind
    {
        HasNext,
        Peek,
        Next,
    }
}
