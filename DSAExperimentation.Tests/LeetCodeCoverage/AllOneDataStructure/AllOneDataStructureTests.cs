using DSAExperimentation.LeetCode.AllOneDataStructure;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllOneDataStructure;

// Harness only. Both strategies are AllOneDataStructureSolution's - this file replays
// LeetCode's published call sequences against each IAllOne instance, so a failure still
// names the strategy that broke even though the "input" here is a sequence of
// Inc/Dec/GetMaxKey/GetMinKey calls rather than a single argument tuple, the same shape
// LRUCacheTests already uses for its own instance-API problem. The pre-migration test
// only proved CreateByBucketedLinkedList's Dec behaviour - CreateByDictionaryScan's
// baseline (previously untested scaffolding inlined in the benchmark) gets that same
// coverage here for the first time. AllOneOp.Apply is pure dispatch, no counting logic
// of its own.
public sealed class AllOneDataStructureTests
{
    public static TheoryData<AllOneOp[], string?[]> Examples =>
        new()
        {
            {
                [AllOneOp.Inc("hello"), AllOneOp.Inc("hello"), AllOneOp.Inc("leet"), AllOneOp.GetMax(), AllOneOp.GetMin()],
                [null, null, null, "hello", "leet"]
            },
            {
                [AllOneOp.GetMin(), AllOneOp.GetMax()],
                ["", ""]
            },
            {
                [AllOneOp.Inc("a"), AllOneOp.Dec("a"), AllOneOp.GetMax()],
                [null, null, ""]
            },
            {
                // "a" is pushed up to count 4 first, so no bucket ever exists at counts 2 or
                // 3; "c" is then inserted (count 1) and fully removed again, leaving only "a"
                // at count 4. A min-tracking scheme that just assumes "the new minimum is
                // oldMinimum + 1" would wrongly look for a bucket at count 2, which was never
                // created.
                [
                    AllOneOp.Inc("a"), AllOneOp.Inc("a"), AllOneOp.Inc("a"), AllOneOp.Inc("a"),
                    AllOneOp.Inc("c"), AllOneOp.Dec("c"),
                    AllOneOp.GetMin(), AllOneOp.GetMax(),
                ],
                [null, null, null, null, null, null, "a", "a"]
            },
            {
                [
                    AllOneOp.Inc("only"), AllOneOp.Inc("only"), AllOneOp.Inc("only"),
                    AllOneOp.Inc("only"), AllOneOp.Inc("only"),
                    AllOneOp.GetMax(), AllOneOp.GetMin(),
                ],
                [null, null, null, null, null, "only", "only"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBucketedLinkedList_LeetCodeExamples_TracksMaxAndMinCountKeys(
        AllOneOp[] operations, string?[] expected) =>
        RunScript(AllOneDataStructureSolution.CreateByBucketedLinkedList(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDictionaryScan_LeetCodeExamples_TracksMaxAndMinCountKeys(
        AllOneOp[] operations, string?[] expected) =>
        RunScript(AllOneDataStructureSolution.CreateByDictionaryScan(), operations, expected);

    private static void RunScript(
        AllOneDataStructureSolution.IAllOne allOne, AllOneOp[] operations, string?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(allOne));
        }
    }
}

// One call in an AllOne script: which method to invoke and with what key. Pure dispatch,
// built via the named factories below so a script (like Examples above) reads like the
// LeetCode call sequence it replays. Inc/Dec return null (no comparable key); GetMax/GetMin
// return the actual answer, including "" for an empty structure - the same null-means-
// "no return value" convention LRUCacheOp.Apply uses for its own put/get split.
public readonly record struct AllOneOp
{
    private readonly Kind _kind;
    private readonly string _key;

    private AllOneOp(Kind kind, string key)
    {
        _kind = kind;
        _key = key;
    }

    public static AllOneOp Inc(string key) => new(Kind.Inc, key);

    public static AllOneOp Dec(string key) => new(Kind.Dec, key);

    public static AllOneOp GetMax() => new(Kind.GetMax, "");

    public static AllOneOp GetMin() => new(Kind.GetMin, "");

    internal string? Apply(AllOneDataStructureSolution.IAllOne allOne)
    {
        switch (_kind)
        {
            case Kind.Inc:
                allOne.Inc(_key);
                return null;
            case Kind.Dec:
                allOne.Dec(_key);
                return null;
            case Kind.GetMax:
                return allOne.GetMaxKey();
            default:
                return allOne.GetMinKey();
        }
    }

    private enum Kind
    {
        Inc,
        Dec,
        GetMax,
        GetMin,
    }
}
