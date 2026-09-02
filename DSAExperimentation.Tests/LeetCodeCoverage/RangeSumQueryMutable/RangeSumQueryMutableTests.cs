using DSAExperimentation.LeetCode.RangeSumQueryMutable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQueryMutable;

// Harness only. Both strategies are RangeSumQueryMutableSolution's - this file replays
// LeetCode's published call sequences against each INumArray implementation via a small operation
// script, so a failure still names the strategy that broke even though the "input" here is a
// sequence of mutating calls rather than a single argument tuple - the same shape
// DesignTaskManagerTests already uses for its own instance-API problem. NumArrayOp.Apply is pure
// dispatch (which method to call with which arguments) - no summing logic of its own.
public sealed class RangeSumQueryMutableTests
{
    public static TheoryData<int[], NumArrayOp[], int?[]> Examples =>
        new()
        {
            {
                [1, 3, 5],
                [
                    NumArrayOp.SumRange(0, 2),
                    NumArrayOp.Update(1, 2),
                    NumArrayOp.SumRange(0, 2),
                ],
                [9, null, 8]
            },
            {
                [0, 0, 0, 0],
                [
                    NumArrayOp.Update(2, 10),
                    NumArrayOp.Update(2, 4),
                    NumArrayOp.SumRange(0, 3),
                ],
                [null, null, 4]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByArrayRescan_LeetCodeExamples_ReflectsMutation(int[] initial, NumArrayOp[] operations, int?[] expected) =>
        RunScript(RangeSumQueryMutableSolution.CreateByArrayRescan(initial), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySegmentTreeQuery_LeetCodeExamples_ReflectsMutation(int[] initial, NumArrayOp[] operations, int?[] expected) =>
        RunScript(RangeSumQueryMutableSolution.CreateBySegmentTreeQuery(initial), operations, expected);

    private static void RunScript(INumArray numArray, NumArrayOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(numArray));
        }
    }
}

// One call in a NumArray script: which operation to invoke and with what arguments. Pure
// dispatch, built via the named factories below so a script (like Examples above) reads like the
// LeetCode call sequence it replays.
public readonly record struct NumArrayOp
{
    private readonly Kind _kind;
    private readonly int _a;
    private readonly int _b;

    private NumArrayOp(Kind kind, int a, int b)
    {
        _kind = kind;
        _a = a;
        _b = b;
    }

    public static NumArrayOp Update(int index, int val) => new(Kind.Update, index, val);

    public static NumArrayOp SumRange(int left, int right) => new(Kind.SumRange, left, right);

    // null for Update, the returned sum for SumRange - so a script runner can assert against one
    // expected value per operation uniformly. Internal, not public: only this same assembly's
    // RunScript ever calls Apply.
    internal int? Apply(INumArray numArray)
    {
        switch (_kind)
        {
            case Kind.Update:
                numArray.Update(_a, _b);
                return null;
            default:
                return numArray.SumRange(_a, _b);
        }
    }

    private enum Kind
    {
        Update,
        SumRange,
    }
}
