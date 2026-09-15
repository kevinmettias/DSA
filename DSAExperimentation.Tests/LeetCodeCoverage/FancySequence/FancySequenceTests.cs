using DSAExperimentation.LeetCode.FancySequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FancySequence;

// Harness only. Both strategies are FancySequenceSolution's - this file replays
// LeetCode's published call sequences against each IFancySequence instance, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of append/addAll/multAll/getIndex calls rather than a single argument
// tuple, the same shape AllOneDataStructureTests already uses for its own
// instance-API problem. The pre-migration test only proved the lazy-segment-tree
// strategy; CreateByArrayRescan (previously untested scaffolding inlined in the
// benchmark, where it only ever produced a checksum sum rather than answering
// getIndex at all) gets that same coverage here for the first time. FancyOp.Apply
// is pure dispatch, no sequence logic of its own.
public sealed class FancySequenceTests
{
    private const int NeverAppended = -1;

    public static TheoryData<int, FancyOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's own published example: [2] -> [5] -> [5,7] -> [10,14],
                // then -> [13,17] -> [26,34].
                2,
                [
                    FancyOp.Append(2), FancyOp.AddAll(3), FancyOp.Append(7), FancyOp.MultAll(2),
                    FancyOp.GetIndex(0),
                    FancyOp.AddAll(3), FancyOp.MultAll(2),
                    FancyOp.GetIndex(0), FancyOp.GetIndex(1),
                ],
                [null, null, null, null, 10, null, null, 26, 34]
            },
            {
                // The same opening, then a larger addAll: [10,14] -> [14,18] -> [28,36].
                4,
                [
                    FancyOp.Append(2), FancyOp.AddAll(3), FancyOp.Append(7), FancyOp.MultAll(2),
                    FancyOp.GetIndex(0),
                    FancyOp.AddAll(4), FancyOp.MultAll(2),
                    FancyOp.GetIndex(0), FancyOp.GetIndex(1),
                ],
                [null, null, null, null, 10, null, null, 28, 36]
            },
            {
                2,
                [FancyOp.Append(5), FancyOp.GetIndex(1)],
                [null, NeverAppended]
            },
            {
                // An index can be out of range and then come into range as the
                // sequence grows.
                3,
                [FancyOp.Append(1), FancyOp.Append(2), FancyOp.GetIndex(2), FancyOp.Append(3), FancyOp.GetIndex(2)],
                [null, null, NeverAppended, null, 3]
            },
            {
                // Appending 0 must not collide with the affine algebra's
                // NoUpdate = (1, 0) sentinel.
                1,
                [FancyOp.Append(0), FancyOp.GetIndex(0)],
                [null, 0]
            },
            {
                // multAll(1) and addAll(0) are both the affine identity - the two
                // operations the composed strategy skips rather than forwarding.
                1,
                [FancyOp.Append(7), FancyOp.MultAll(1), FancyOp.AddAll(0), FancyOp.GetIndex(0)],
                [null, null, null, 7]
            },
            {
                // addAll on an empty sequence applies to nothing, and must not leak
                // onto a value appended afterwards.
                1,
                [FancyOp.AddAll(5), FancyOp.Append(3), FancyOp.GetIndex(0)],
                [null, null, 3]
            },
            {
                // 1e9 * 2 = 2e9, which wraps to 999,999,993 modulo 1e9+7.
                1,
                [FancyOp.Append(1_000_000_000), FancyOp.MultAll(2), FancyOp.GetIndex(0)],
                [null, null, 999_999_993]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByArrayRescan_LeetCodeExamples_ReportsEachIndexModuloOneENine(
        int capacity, FancyOp[] operations, int?[] expected) =>
        RunScript(FancySequenceSolution.CreateByArrayRescan(capacity), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLazySegmentTreeAffine_LeetCodeExamples_ReportsEachIndexModuloOneENine(
        int capacity, FancyOp[] operations, int?[] expected) =>
        RunScript(FancySequenceSolution.CreateByLazySegmentTreeAffine(capacity), operations, expected);

    private static void RunScript(
        FancySequenceSolution.IFancySequence fancy, FancyOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(fancy));
        }
    }
}

// One call in a Fancy script: which method to invoke and with what argument. Pure
// dispatch, built via the named factories below so a script (like Examples above)
// reads like the LeetCode call sequence it replays. append/addAll/multAll return
// null (no value to compare); getIndex returns the actual answer, including -1 for
// an index that was never appended - the same null-means-"no return value"
// convention AllOneOp.Apply uses for its own inc/dec split.
public readonly record struct FancyOp(FancyOp.OpKind kind, int value)
{
    public static FancyOp Append(int val) => new(OpKind.Append, val);

    public static FancyOp AddAll(int inc) => new(OpKind.AddAll, inc);

    public static FancyOp MultAll(int m) => new(OpKind.MultAll, m);

    public static FancyOp GetIndex(int idx) => new(OpKind.GetIndex, idx);

    internal int? Apply(FancySequenceSolution.IFancySequence fancy)
    {
        switch (kind)
        {
            case OpKind.Append:
                fancy.Append(value);
                return null;
            case OpKind.AddAll:
                fancy.AddAll(value);
                return null;
            case OpKind.MultAll:
                fancy.MultAll(value);
                return null;
            default:
                return fancy.GetIndex(value);
        }
    }

    public enum OpKind
    {
        Append,
        AddAll,
        MultAll,
        GetIndex,
    }
}
