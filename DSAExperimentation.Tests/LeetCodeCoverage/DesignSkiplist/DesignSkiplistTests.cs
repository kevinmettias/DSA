using static DSAExperimentation.LeetCode.DesignSkiplist.DesignSkiplistSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignSkiplist;

// Harness only. Both strategies are DesignSkiplistSolution's - this file replays a
// script of Add/Search/Erase calls against each ISkiplist implementation, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of mutating/querying calls rather than a single argument tuple.
// SkiplistOp.Apply is pure dispatch - no multiset logic of its own.
public sealed class DesignSkiplistTests
{
    public static TheoryData<SkiplistOp[], bool?[]> Examples =>
        new()
        {
            // LeetCode's published call sequence.
            {
                [
                    SkiplistOp.Add(1),
                    SkiplistOp.Add(2),
                    SkiplistOp.Add(3),
                    SkiplistOp.Search(0),
                    SkiplistOp.Add(4),
                    SkiplistOp.Search(1),
                    SkiplistOp.Erase(0),
                    SkiplistOp.Erase(1),
                    SkiplistOp.Search(1),
                ],
                [null, null, null, false, null, true, false, true, false]
            },

            // A multiset, not a set: each Erase removes one occurrence only, and
            // erasing past the last one reports false.
            {
                [
                    SkiplistOp.Add(5),
                    SkiplistOp.Add(5),
                    SkiplistOp.Erase(5),
                    SkiplistOp.Search(5),
                    SkiplistOp.Erase(5),
                    SkiplistOp.Search(5),
                    SkiplistOp.Erase(5),
                ],
                [null, null, true, true, true, false, false]
            },

            // Both ends of LeetCode's value domain (0 <= num <= 20000), which is
            // exactly the range the frequency-array strategy indexes.
            {
                [
                    SkiplistOp.Add(0),
                    SkiplistOp.Add(20_000),
                    SkiplistOp.Search(0),
                    SkiplistOp.Search(20_000),
                    SkiplistOp.Erase(0),
                    SkiplistOp.Search(0),
                    SkiplistOp.Search(20_000),
                ],
                [null, null, true, true, true, false, true]
            },

            // Searching and erasing on an untouched skiplist.
            {
                [
                    SkiplistOp.Search(7),
                    SkiplistOp.Erase(7),
                ],
                [false, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SkiplistByLinearScanList_LeetCodeExamples_MatchesExpectedResults(
        SkiplistOp[] operations, bool?[] expected) =>
        RunScript(new SkiplistByLinearScanList(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SkiplistByFenwickFrequencies_LeetCodeExamples_MatchesExpectedResults(
        SkiplistOp[] operations, bool?[] expected) =>
        RunScript(new SkiplistByFenwickFrequencies(), operations, expected);

    private static void RunScript(ISkiplist skiplist, SkiplistOp[] operations, bool?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(skiplist));
        }
    }
}

// One call in a Skiplist script: which method to invoke and with what value. Pure
// dispatch, built via the named factories below so a script (like Examples above)
// reads like the LeetCode call sequence it replays.
public readonly record struct SkiplistOp
{
    private readonly Kind _kind;
    private readonly int _value;

    private SkiplistOp(Kind kind, int value)
    {
        _kind = kind;
        _value = value;
    }

    public static SkiplistOp Add(int num) => new(Kind.Add, num);

    public static SkiplistOp Search(int target) => new(Kind.Search, target);

    public static SkiplistOp Erase(int num) => new(Kind.Erase, num);

    // null for the void Add, the reported bool for Search and Erase - so a script
    // runner can assert against one expected value per operation uniformly.
    // Internal, not public: ISkiplist is internal to DesignSkiplistSolution, and
    // only this same assembly's RunScript ever calls Apply.
    internal bool? Apply(ISkiplist skiplist)
    {
        switch (_kind)
        {
            case Kind.Add:
                skiplist.Add(_value);
                return null;
            case Kind.Search:
                return skiplist.Search(_value);
            default:
                return skiplist.Erase(_value);
        }
    }

    private enum Kind
    {
        Add,
        Search,
        Erase,
    }
}
