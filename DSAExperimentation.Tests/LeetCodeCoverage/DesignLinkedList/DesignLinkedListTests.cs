using DSAExperimentation.LeetCode.DesignLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignLinkedList;

// Harness only. Both strategies are DesignLinkedListSolution's - this file replays
// LeetCode's published call sequences against each IMyLinkedList instance, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of Get/AddAtHead/AddAtTail/AddAtIndex/DeleteAtIndex calls rather than a
// single argument tuple, the same shape LRUCacheTests already uses for its own
// instance-API problem. The pre-migration test only proved
// CreateBySinglyLinkedListChain's behaviour - CreateByArrayList's baseline
// (previously scaffolding inlined in the benchmark, and only exercising AddAtHead in
// isolation there) gets the same full-API coverage here for the first time.
// DesignLinkedListOp.Apply is pure dispatch, no list logic of its own.
public sealed class DesignLinkedListTests
{
    public static TheoryData<DesignLinkedListOp[], int?[]> Examples =>
        new()
        {
            {
                [
                    DesignLinkedListOp.AddAtHead(1),
                    DesignLinkedListOp.AddAtTail(3),
                    DesignLinkedListOp.AddAtIndex(1, 2), // list: 1 -> 2 -> 3
                    DesignLinkedListOp.Get(1),
                    DesignLinkedListOp.DeleteAtIndex(1), // list: 1 -> 3
                    DesignLinkedListOp.Get(1),
                ],
                [null, null, null, 2, null, 3]
            },
            {
                [
                    DesignLinkedListOp.AddAtHead(7),
                    DesignLinkedListOp.Get(5),
                    DesignLinkedListOp.Get(-1),
                ],
                [null, -1, -1]
            },
            {
                [
                    DesignLinkedListOp.AddAtHead(1),
                    DesignLinkedListOp.AddAtIndex(1, 2), // index == length appends at tail
                    DesignLinkedListOp.Get(1),
                ],
                [null, null, 2]
            },
            {
                [
                    DesignLinkedListOp.AddAtHead(1),
                    DesignLinkedListOp.AddAtIndex(5, 99), // index > length is a no-op
                    DesignLinkedListOp.Get(1),
                ],
                [null, null, -1]
            },
            {
                [
                    DesignLinkedListOp.AddAtHead(1),
                    DesignLinkedListOp.DeleteAtIndex(5), // out-of-bounds delete is a no-op
                    DesignLinkedListOp.Get(0),
                ],
                [null, null, 1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySinglyLinkedListChain_LeetCodeExamples_MatchesExpectedResults(
        DesignLinkedListOp[] operations, int?[] expected) =>
        RunScript(DesignLinkedListSolution.CreateBySinglyLinkedListChain(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByArrayList_LeetCodeExamples_MatchesExpectedResults(
        DesignLinkedListOp[] operations, int?[] expected) =>
        RunScript(DesignLinkedListSolution.CreateByArrayList(), operations, expected);

    private static void RunScript(
        DesignLinkedListSolution.IMyLinkedList list, DesignLinkedListOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(list));
        }
    }
}

// One call in a DesignLinkedList script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays. Get returns the
// actual value (or -1 on LeetCode's own out-of-bounds convention); every mutator
// returns null, the same null-means-"no return value" convention LRUCacheOp.Apply
// uses for its own put/get split.
public readonly record struct DesignLinkedListOp
{
    private readonly Kind _kind;
    private readonly int _index;
    private readonly int _value;

    private DesignLinkedListOp(Kind kind, int index, int value)
    {
        _kind = kind;
        _index = index;
        _value = value;
    }

    public static DesignLinkedListOp Get(int index) => new(Kind.Get, index, 0);

    public static DesignLinkedListOp AddAtHead(int value) => new(Kind.AddAtHead, 0, value);

    public static DesignLinkedListOp AddAtTail(int value) => new(Kind.AddAtTail, 0, value);

    public static DesignLinkedListOp AddAtIndex(int index, int value) => new(Kind.AddAtIndex, index, value);

    public static DesignLinkedListOp DeleteAtIndex(int index) => new(Kind.DeleteAtIndex, index, 0);

    internal int? Apply(DesignLinkedListSolution.IMyLinkedList list)
    {
        switch (_kind)
        {
            case Kind.Get:
                return list.Get(_index);
            case Kind.AddAtHead:
                list.AddAtHead(_value);
                return null;
            case Kind.AddAtTail:
                list.AddAtTail(_value);
                return null;
            case Kind.AddAtIndex:
                list.AddAtIndex(_index, _value);
                return null;
            default:
                list.DeleteAtIndex(_index);
                return null;
        }
    }

    private enum Kind
    {
        Get,
        AddAtHead,
        AddAtTail,
        AddAtIndex,
        DeleteAtIndex,
    }
}
