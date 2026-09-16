using DSAExperimentation.LeetCode.RangeModule;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeModule;

// Harness only. Both strategies are RangeModuleSolution's - this file replays
// LeetCode's published call sequences against each IRangeModule instance, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of AddRange/QueryRange/RemoveRange calls rather than a single argument
// tuple, the same shape AllOneDataStructureTests already uses for its own instance-
// API problem. The pre-migration test only proved CreateByIntervalSetBinarySearch's
// behaviour - CreateByLinearScan's baseline (previously untested scaffolding inlined
// in the benchmark, and there only for queryRange) gets that same coverage here for
// the first time. RangeModuleOp.Apply is pure dispatch, no interval logic of its own.
public sealed class RangeModuleTests
{
    public static TheoryData<RangeModuleOp[], bool?[]> Examples =>
        new()
        {
            {
                [
                    RangeModuleOp.Add(10, 20),
                    RangeModuleOp.Query(10, 14),
                    RangeModuleOp.Query(13, 15),
                    RangeModuleOp.Query(16, 17),
                    RangeModuleOp.Remove(14, 16),
                    RangeModuleOp.Query(10, 14),
                    RangeModuleOp.Query(13, 15),
                    RangeModuleOp.Query(16, 17),
                ],
                [null, true, true, true, null, true, false, true]
            },
            {
                [RangeModuleOp.Add(1, 3), RangeModuleOp.Add(5, 7), RangeModuleOp.Query(2, 6)],
                [null, null, false]
            },
            {
                [RangeModuleOp.Add(1, 3), RangeModuleOp.Add(3, 5), RangeModuleOp.Query(1, 5)],
                [null, null, true]
            },
            {
                [
                    RangeModuleOp.Add(1, 10),
                    RangeModuleOp.Remove(1, 10),
                    RangeModuleOp.Query(1, 10),
                    RangeModuleOp.Query(2, 3),
                ],
                [null, null, false, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIntervalSetBinarySearch_LeetCodeExamples_TracksAddQueryAndRemove(
        RangeModuleOp[] operations, bool?[] expected) =>
        RunScript(RangeModuleSolution.CreateByIntervalSetBinarySearch(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearScan_LeetCodeExamples_TracksAddQueryAndRemove(
        RangeModuleOp[] operations, bool?[] expected) =>
        RunScript(RangeModuleSolution.CreateByLinearScan(), operations, expected);

    private static void RunScript(RangeModuleSolution.IRangeModule module, RangeModuleOp[] operations, bool?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(module));
        }
    }

    // One call in a RangeModule script: which method to invoke and with what bounds. Pure
    // dispatch, built via the named factories below so a script (like Examples above)
    // reads like the LeetCode call sequence it replays. Add/Remove return null (no
    // comparable value); Query returns the actual answer - the same null-means-"no return
    // value" convention AllOneOp.Apply uses for its own Inc/Dec split. Nested here rather
    // than left at file scope so the file declares exactly one type.
    public readonly record struct RangeModuleOp(RangeModuleOp.OpKind kind, int left, int right)
    {
        public static RangeModuleOp Add(int left, int right) => new(OpKind.Add, left, right);

        public static RangeModuleOp Query(int left, int right) => new(OpKind.Query, left, right);

        public static RangeModuleOp Remove(int left, int right) => new(OpKind.Remove, left, right);

        internal bool? Apply(RangeModuleSolution.IRangeModule module)
        {
            switch (kind)
            {
                case OpKind.Add:
                    module.AddRange(left, right);
                    return null;
                case OpKind.Query:
                    return module.QueryRange(left, right);
                default:
                    module.RemoveRange(left, right);
                    return null;
            }
        }

        public enum OpKind
        {
            Add,
            Query,
            Remove,
        }
    }
}
