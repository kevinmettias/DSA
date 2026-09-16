using DSAExperimentation.LeetCode.DesignHashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashMap;

// Harness only. Both strategies are DesignHashMapSolution's - this file replays
// a script of Put/Get/Remove calls against each IMyHashMap implementation, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of mutating/querying calls rather than a single argument tuple.
// HashMapOp.Apply is pure dispatch - no key/value storage logic of its own.
public sealed partial class DesignHashMapTests
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
        Assert.Equal(expected, RunScript(new DesignHashMapSolution.MyHashMapByLinearScanList(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MyHashMapByHashMapBacked_LeetCodeExamples_MatchesExpectedResults(
        HashMapOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignHashMapSolution.MyHashMapByHashMapBacked(), operations));

    private static int?[] RunScript(DesignHashMapSolution.IMyHashMap map, HashMapOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(map))];
}
