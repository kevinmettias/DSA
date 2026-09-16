using DSAExperimentation.LeetCode.RangeSumQueryMutable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQueryMutable;

// Harness only. Both strategies are RangeSumQueryMutableSolution's - this file replays
// LeetCode's published call sequences against each INumArray implementation via a small operation
// script, so a failure still names the strategy that broke even though the "input" here is a
// sequence of mutating calls rather than a single argument tuple - the same shape
// DesignTaskManagerTests already uses for its own instance-API problem. RangeSumOperation.Apply is pure
// dispatch (which method to call with which arguments) - no summing logic of its own.
public sealed partial class RangeSumQueryMutableTests
{
    public static TheoryData<int[], RangeSumOperation[], int?[]> Examples =>
        new()
        {
            {
                [1, 3, 5],
                [
                    RangeSumOperation.SumRange(0, 2),
                    RangeSumOperation.Update(1, 2),
                    RangeSumOperation.SumRange(0, 2),
                ],
                [9, null, 8]
            },
            {
                [0, 0, 0, 0],
                [
                    RangeSumOperation.Update(2, 10),
                    RangeSumOperation.Update(2, 4),
                    RangeSumOperation.SumRange(0, 3),
                ],
                [null, null, 4]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByArrayRescan_LeetCodeExamples_ReflectsMutation(int[] initial, RangeSumOperation[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(RangeSumQueryMutableSolution.CreateByArrayRescan(initial), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySegmentTreeQuery_LeetCodeExamples_ReflectsMutation(int[] initial, RangeSumOperation[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(RangeSumQueryMutableSolution.CreateBySegmentTreeQuery(initial), operations));

    private static int?[] RunScript(INumArray numArray, RangeSumOperation[] operations) =>
        [.. operations.Select(operation => operation.Apply(numArray))];

    // One call in a NumArray script: which operation to invoke and with what arguments. Pure
    // dispatch, built via the named factories below so a script (like Examples above) reads like the
    // LeetCode call sequence it replays. Nested here rather than left at file scope so the file
    // declares exactly one type.
    public readonly record struct RangeSumOperation(RangeSumOperation.OpKind kind, int a, int b)
    {
        public static RangeSumOperation Update(int index, int val) => new(OpKind.Update, index, val);

        public static RangeSumOperation SumRange(int left, int right) => new(OpKind.SumRange, left, right);

        // null for Update, the returned sum for SumRange - so a script runner can assert against one
        // expected value per operation uniformly. Internal, not public: only this same assembly's
        // RunScript ever calls Apply.
        internal int? Apply(INumArray numArray)
        {
            switch (kind)
            {
                case OpKind.Update:
                    numArray.Update(a, b);
                    return null;
                default:
                    return numArray.SumRange(a, b);
            }
        }

        public enum OpKind
        {
            Update,
            SumRange,
        }
    }
}
