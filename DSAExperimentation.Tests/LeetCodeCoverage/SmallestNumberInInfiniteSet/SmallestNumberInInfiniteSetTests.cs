using DSAExperimentation.LeetCode.SmallestNumberInInfiniteSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestNumberInInfiniteSet;

// Harness only. Both strategies are SmallestNumberInInfiniteSetSolution's - this file
// replays LeetCode's published call sequence against each ISmallestInfiniteSet
// instance, so a failure still names the strategy that broke even though the "input"
// here is a sequence of PopSmallest/AddBack calls rather than a single argument tuple,
// the same shape AllOneDataStructureTests already uses for its own instance-API
// problem. The pre-migration test only proved the heap+set composition;
// CreateByListScan's baseline (previously untested scaffolding inlined in the
// benchmark) gets that same coverage here for the first time. InfiniteSetOp.Apply is
// pure dispatch, no set logic of its own.
public sealed class SmallestNumberInInfiniteSetTests
{
    public static TheoryData<InfiniteSetOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's own example: AddBack(2) is a no-op because 2 has never been
                // popped, so it is already in the set.
                [
                    InfiniteSetOp.AddBack(2),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                    InfiniteSetOp.AddBack(1),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                ],
                [null, 1, 2, 3, null, 1, 4, 5]
            },
            {
                // A duplicate AddBack must not queue the same number twice.
                [
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                    InfiniteSetOp.AddBack(1), InfiniteSetOp.AddBack(1),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                ],
                [1, 2, null, null, 1, 3]
            },
            {
                // AddBack of a number never produced is ignored - it is already present.
                [
                    InfiniteSetOp.AddBack(5),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                ],
                [null, 1, 2, 3, 4, 5]
            },
            {
                // Several numbers added back out of order all come out smallest-first,
                // and only then does the counter resume.
                [
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                    InfiniteSetOp.AddBack(4), InfiniteSetOp.AddBack(2), InfiniteSetOp.AddBack(3),
                    InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop(),
                ],
                [1, 2, 3, 4, 5, null, null, null, 2, 3, 4, 6]
            },
            {
                // Popping with nothing ever added back is pure counter walking.
                [InfiniteSetOp.Pop(), InfiniteSetOp.Pop(), InfiniteSetOp.Pop()],
                [1, 2, 3]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHeapAndSet_LeetCodeExamples_MatchesTheInfiniteSetTrace(
        InfiniteSetOp[] operations, int?[] expected) =>
        RunScript(SmallestNumberInInfiniteSetSolution.CreateByHeapAndSet(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByListScan_LeetCodeExamples_MatchesTheInfiniteSetTrace(
        InfiniteSetOp[] operations, int?[] expected) =>
        RunScript(SmallestNumberInInfiniteSetSolution.CreateByListScan(), operations, expected);

    private static void RunScript(
        SmallestNumberInInfiniteSetSolution.ISmallestInfiniteSet set,
        InfiniteSetOp[] operations,
        int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(set));
        }
    }

    // One call in a SmallestInfiniteSet script: which method to invoke and with what
    // argument. Pure dispatch, built via the named factories below so a script (like
    // Examples above) reads like the LeetCode call sequence it replays. AddBack returns
    // null (no value); PopSmallest returns the actual answer - the same null-means-"no
    // return value" convention AllOneOp.Apply uses for its own void/value split. Nested
    // here rather than left at file scope so the file declares exactly one type.
    public readonly record struct InfiniteSetOp(InfiniteSetOp.OpKind kind, int num)
    {
        public static InfiniteSetOp Pop() => new(OpKind.Pop, 0);

        public static InfiniteSetOp AddBack(int num) => new(OpKind.AddBack, num);

        internal int? Apply(SmallestNumberInInfiniteSetSolution.ISmallestInfiniteSet set)
        {
            if (kind == OpKind.AddBack)
            {
                set.AddBack(num);
                return null;
            }

            return set.PopSmallest();
        }

        public enum OpKind
        {
            Pop,
            AddBack,
        }
    }
}
