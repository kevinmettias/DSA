using DSAExperimentation.LeetCode.DesignHashSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashSet;

// Harness only. Both strategies are DesignHashSetSolution's - this file replays
// a script of Add/Remove/Contains calls against each IMyHashSet implementation,
// so a failure still names the strategy that broke even though the "input" here
// is a sequence of mutating/querying calls rather than a single argument tuple.
// HashSetOp.Apply is pure dispatch - no membership-tracking logic of its own.
public sealed partial class DesignHashSetTests
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
        Assert.Equal(expected, RunScript(new DesignHashSetSolution.MyHashSetByListScan(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MyHashSetBySetBacked_LeetCodeExamples_TracksMembershipCorrectly(
        HashSetOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignHashSetSolution.MyHashSetBySetBacked(), operations));

    private static bool?[] RunScript(DesignHashSetSolution.IMyHashSet set, HashSetOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(set))];
}
