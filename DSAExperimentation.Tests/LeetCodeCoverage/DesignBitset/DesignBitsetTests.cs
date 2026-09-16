using System.Globalization;
using IBitset = DSAExperimentation.LeetCode.DesignBitset.DesignBitsetSolution.IBitset;
using BitsetByEagerFlip = DSAExperimentation.LeetCode.DesignBitset.DesignBitsetSolution.BitsetByEagerFlip;
using BitsetByLazyFlag = DSAExperimentation.LeetCode.DesignBitset.DesignBitsetSolution.BitsetByLazyFlag;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignBitset;

// Harness only: the algorithm lives in DesignBitsetSolution. LeetCode's own shape
// here is a stateful object across a sequence of calls, so Examples encodes a call
// script instead of a single argument tuple - the same shape DesignCircularDequeTests
// already uses for its own instance-API problem. BitsetOp.Apply renders every
// operation's result as the string LeetCode's own judge output shows for it - "null"
// for the void mutators, "true"/"false" for all()/one(), the decimal count, and the
// bit string itself - so one expected value per call covers the whole mixed surface.
public sealed class DesignBitsetTests
{
    public static TheoryData<int, BitsetOp[], string[]> Examples =>
        new()
        {
            // LeetCode's published example: "00000" -> fix(3) -> fix(1) -> flip()
            // -> all() is false -> unfix(0) -> flip() -> one() is true -> unfix(0)
            // -> count() is 2 -> toString() is "01010".
            {
                5,
                [
                    BitsetOp.Fix(3),
                    BitsetOp.Fix(1),
                    BitsetOp.Flip(),
                    BitsetOp.All(),
                    BitsetOp.Unfix(0),
                    BitsetOp.Flip(),
                    BitsetOp.One(),
                    BitsetOp.Unfix(0),
                    BitsetOp.Count(),
                    BitsetOp.ToBitString(),
                ],
                ["null", "null", "null", "false", "null", "null", "true", "null", "2", "01010"]
            },

            // The step-by-step script this problem's pre-migration test asserted,
            // kept whole: fix(3), fix(4), flip(), unfix(0), flip(), ending at
            // "10011" with three ones set.
            {
                5,
                [
                    BitsetOp.Fix(3),
                    BitsetOp.Fix(4),
                    BitsetOp.ToBitString(),
                    BitsetOp.Flip(),
                    BitsetOp.ToBitString(),
                    BitsetOp.All(),
                    BitsetOp.Unfix(0),
                    BitsetOp.ToBitString(),
                    BitsetOp.Flip(),
                    BitsetOp.ToBitString(),
                    BitsetOp.Count(),
                    BitsetOp.One(),
                ],
                [
                    "null", "null", "00011", "null", "11100", "false", "null", "01100", "null", "10011",
                    "3", "true",
                ]
            },

            // Every bit fixed: all() is the only case that reports true.
            {
                3,
                [BitsetOp.Fix(0), BitsetOp.Fix(1), BitsetOp.Fix(2), BitsetOp.All(), BitsetOp.Count()],
                ["null", "null", "null", "true", "3"]
            },

            // Nothing ever fixed: one() is false and the rendering is all zeros.
            {
                4,
                [BitsetOp.One(), BitsetOp.Count(), BitsetOp.ToBitString()],
                ["false", "0", "0000"]
            },

            // Flipping twice returns to the original state, count included.
            {
                4,
                [BitsetOp.Fix(1), BitsetOp.Flip(), BitsetOp.Flip(), BitsetOp.ToBitString(), BitsetOp.Count()],
                ["null", "null", "null", "0100", "1"]
            },

            // Fixing an already-set bit and unfixing an already-clear one are both
            // no-ops, so the running count must not drift.
            {
                3,
                [
                    BitsetOp.Fix(2),
                    BitsetOp.Fix(2),
                    BitsetOp.Unfix(0),
                    BitsetOp.Count(),
                    BitsetOp.ToBitString(),
                ],
                ["null", "null", "null", "1", "001"]
            },

            // The same no-op check while the flip flag is set, where a raw write
            // and a visible write disagree.
            {
                3,
                [
                    BitsetOp.Flip(),
                    BitsetOp.Fix(1),
                    BitsetOp.Unfix(2),
                    BitsetOp.Count(),
                    BitsetOp.ToBitString(),
                ],
                ["null", "null", "null", "2", "110"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BitsetByEagerFlip_LeetCodeExamples_MatchesExpectedSequence(
        int size, BitsetOp[] operations, string[] expected) =>
        RunScript(new BitsetByEagerFlip(size), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void BitsetByLazyFlag_LeetCodeExamples_MatchesExpectedSequence(
        int size, BitsetOp[] operations, string[] expected) =>
        RunScript(new BitsetByLazyFlag(size), operations, expected);

    private static void RunScript(IBitset bitset, BitsetOp[] operations, string[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(bitset));
        }
    }

    // One call in a Bitset script: which operation to invoke and with what index. Pure
    // dispatch, built via the named factories below so a script (like Examples above)
    // reads like the LeetCode call sequence it replays. Nested, because it is this
    // harness's own way of stating a script and has no identity outside it.
    public readonly record struct BitsetOp(BitsetOp.OpKind kind, int index)
    {
        private const string VoidResult = "null";
        private const string TrueResult = "true";
        private const string FalseResult = "false";

        public static BitsetOp Fix(int idx) => new(OpKind.Fix, idx);

        public static BitsetOp Unfix(int idx) => new(OpKind.Unfix, idx);

        public static BitsetOp Flip() => new(OpKind.Flip, 0);

        public static BitsetOp All() => new(OpKind.All, 0);

        public static BitsetOp One() => new(OpKind.One, 0);

        public static BitsetOp Count() => new(OpKind.Count, 0);

        public static BitsetOp ToBitString() => new(OpKind.ToBitString, 0);

        // The string LeetCode's own judge output shows for this call, so one expected
        // value per operation covers the mutators, the two predicates, the count and
        // the rendering uniformly.
        internal string Apply(IBitset bitset) => kind switch
        {
            OpKind.Fix => ApplyFix(bitset),
            OpKind.Unfix => ApplyUnfix(bitset),
            OpKind.Flip => ApplyFlip(bitset),
            OpKind.All => Rendered(bitset.All()),
            OpKind.One => Rendered(bitset.One()),
            OpKind.Count => bitset.Count().ToString(CultureInfo.InvariantCulture),
            _ => bitset.ToBitString(),
        };

        // The two index-taking writes, each named the way ApplyFlip names the flip: the
        // bare Action<int> this used to hand around said only that an int went somewhere,
        // never which write of the bitset was being made.
        private string ApplyFix(IBitset bitset)
        {
            bitset.Fix(index);

            return VoidResult;
        }

        private string ApplyUnfix(IBitset bitset)
        {
            bitset.Unfix(index);

            return VoidResult;
        }

        private static string ApplyFlip(IBitset bitset)
        {
            bitset.Flip();

            return VoidResult;
        }

        private static string Rendered(bool value) => value ? TrueResult : FalseResult;

        public enum OpKind
        {
            Fix,
            Unfix,
            Flip,
            All,
            One,
            Count,
            ToBitString,
        }
    }
}
