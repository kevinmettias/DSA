using static DSAExperimentation.LeetCode.DesignHashMap.DesignHashMapSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashMap;

// Harness only. Both strategies are DesignHashMapSolution's - this file replays
// a script of Put/Get/Remove calls against each IMyHashMap implementation, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of mutating/querying calls rather than a single argument tuple.
// HashMapOp.Apply is pure dispatch - no key/value storage logic of its own.
public sealed class DesignHashMapTests
{
    public static TheoryData<HashMapOp[], int?[]> Examples =>
        new()
        {
            {
                [
                    HashMapOp.Put(1, 1),
                    HashMapOp.Put(2, 2),
                    HashMapOp.Get(1),
                    HashMapOp.Get(3),
                    HashMapOp.Put(2, 1), // update an existing key's value
                    HashMapOp.Get(2),
                    HashMapOp.Remove(2),
                    HashMapOp.Get(2),
                ],
                [null, null, 1, -1, null, 1, null, -1]
            },
            {
                // Removing a key that was never inserted is a no-op.
                [
                    HashMapOp.Remove(42),
                    HashMapOp.Get(42),
                ],
                [null, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MyHashMapByLinearScanList_LeetCodeExamples_MatchesExpectedResults(
        HashMapOp[] operations, int?[] expected) =>
        RunScript(new MyHashMapByLinearScanList(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MyHashMapByHashMapBacked_LeetCodeExamples_MatchesExpectedResults(
        HashMapOp[] operations, int?[] expected) =>
        RunScript(new MyHashMapByHashMapBacked(), operations, expected);

    private static void RunScript(IMyHashMap map, HashMapOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(map));
        }
    }
}

// One call in a MyHashMap script: which method to invoke and with what
// key/value. Pure dispatch, built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct HashMapOp(HashMapOp.OpKind kind, int key, int value)
{
    public static HashMapOp Put(int key, int value) => new(OpKind.Put, key, value);

    public static HashMapOp Get(int key) => new(OpKind.Get, key, 0);

    public static HashMapOp Remove(int key) => new(OpKind.Remove, key, 0);

    // null for the two void calls, the looked-up value (or -1) for Get - so a
    // script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IMyHashMap is internal to
    // DesignHashMapSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal int? Apply(IMyHashMap map)
    {
        switch (kind)
        {
            case OpKind.Put:
                map.Put(key, value);
                return null;
            case OpKind.Remove:
                map.Remove(key);
                return null;
            default:
                return map.Get(key);
        }
    }

    public enum OpKind
    {
        Put,
        Get,
        Remove,
    }
}
