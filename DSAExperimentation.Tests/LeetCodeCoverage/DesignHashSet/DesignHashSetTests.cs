using static DSAExperimentation.LeetCode.DesignHashSet.DesignHashSetSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashSet;

// Harness only. Both strategies are DesignHashSetSolution's - this file replays
// a script of Add/Remove/Contains calls against each IMyHashSet implementation,
// so a failure still names the strategy that broke even though the "input" here
// is a sequence of mutating/querying calls rather than a single argument tuple.
// HashSetOp.Apply is pure dispatch - no membership-tracking logic of its own.
public sealed class DesignHashSetTests
{
    public static TheoryData<HashSetOp[], bool?[]> Examples =>
        new()
        {
            {
                [
                    HashSetOp.Add(1),
                    HashSetOp.Add(2),
                    HashSetOp.Contains(1),
                    HashSetOp.Contains(3),
                    HashSetOp.Add(3),
                    HashSetOp.Contains(3),
                    HashSetOp.Remove(2),
                    HashSetOp.Contains(2),
                ],
                [null, null, true, false, null, true, null, false]
            },
            {
                // LeetCode's own published example: re-adding an already-present
                // key is a no-op, not a duplicate.
                [
                    HashSetOp.Add(1),
                    HashSetOp.Add(2),
                    HashSetOp.Contains(1),
                    HashSetOp.Contains(3),
                    HashSetOp.Add(2),
                    HashSetOp.Contains(2),
                    HashSetOp.Remove(2),
                    HashSetOp.Contains(2),
                ],
                [null, null, true, false, null, true, null, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MyHashSetByListScan_LeetCodeExamples_TracksMembershipCorrectly(
        HashSetOp[] operations, bool?[] expected) =>
        RunScript(new MyHashSetByListScan(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MyHashSetBySetBacked_LeetCodeExamples_TracksMembershipCorrectly(
        HashSetOp[] operations, bool?[] expected) =>
        RunScript(new MyHashSetBySetBacked(), operations, expected);

    private static void RunScript(IMyHashSet set, HashSetOp[] operations, bool?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(set));
        }
    }
}

// One call in a MyHashSet script: which method to invoke and with what key.
// Pure dispatch, built via the named factories below so a script (like Examples
// above) reads like the LeetCode call sequence it replays.
public readonly record struct HashSetOp
{
    private readonly Kind _kind;
    private readonly int _key;

    private HashSetOp(Kind kind, int key)
    {
        _kind = kind;
        _key = key;
    }

    public static HashSetOp Add(int key) => new(Kind.Add, key);

    public static HashSetOp Remove(int key) => new(Kind.Remove, key);

    public static HashSetOp Contains(int key) => new(Kind.Contains, key);

    // null for the two void calls, the membership result for Contains - so a
    // script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IMyHashSet is internal to
    // DesignHashSetSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal bool? Apply(IMyHashSet set)
    {
        switch (_kind)
        {
            case Kind.Add:
                set.Add(_key);
                return null;
            case Kind.Remove:
                set.Remove(_key);
                return null;
            default:
                return set.Contains(_key);
        }
    }

    private enum Kind
    {
        Add,
        Remove,
        Contains,
    }
}
