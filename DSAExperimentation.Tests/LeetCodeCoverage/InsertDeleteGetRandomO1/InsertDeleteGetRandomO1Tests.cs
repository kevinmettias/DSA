using DSAExperimentation.LeetCode.InsertDeleteGetRandomO1;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertDeleteGetRandomO1;

// Harness only. Both strategies are InsertDeleteGetRandomO1Solution's - this file
// replays LeetCode's published call sequence, plus the middle-element swap-back case
// the original test covered, against each IRandomizedSet implementation via a small
// operation script, so a failure still names the strategy that broke even though the
// "input" here is a sequence of mutating/querying calls rather than a single argument
// tuple. GetRandom's result is nondeterministic and depends on each strategy's own
// internal array order, so its expected slot carries the set of values that are valid
// to return at that point in the script rather than one exact value - the same
// "expected slot shape depends on which call it answers" idea ImplementRouterTests
// already uses for ForwardPacket's int[]. RandomizedSetOp.Apply is pure dispatch - no
// membership-tracking logic of its own.
public sealed class InsertDeleteGetRandomO1Tests
{
    public static TheoryData<RandomizedSetOp[], object?[]> Examples =>
        new()
        {
            {
                [
                    RandomizedSetOp.Insert(1),
                    RandomizedSetOp.Remove(2),
                    RandomizedSetOp.Insert(2),
                    RandomizedSetOp.Count(),
                    RandomizedSetOp.GetRandom(),
                    RandomizedSetOp.Remove(1),
                    RandomizedSetOp.Insert(2),
                    RandomizedSetOp.GetRandom(),
                ],
                [true, false, true, 2, new[] { 1, 2 }, true, false, new[] { 2 }]
            },
            {
                [
                    RandomizedSetOp.Insert(10),
                    RandomizedSetOp.Insert(20),
                    RandomizedSetOp.Insert(30),
                    RandomizedSetOp.Remove(10),
                    RandomizedSetOp.Count(),
                    RandomizedSetOp.Remove(10),
                    RandomizedSetOp.Remove(30),
                    RandomizedSetOp.Remove(20),
                    RandomizedSetOp.Count(),
                ],
                [true, true, true, true, 2, false, true, true, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomizedSetByListScan_LeetCodeExamples_TracksMembershipCorrectly(
        RandomizedSetOp[] operations, object?[] expected) =>
        RunScript(new InsertDeleteGetRandomO1Solution.RandomizedSetByListScan(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomizedSetByHashMapSwapRemove_LeetCodeExamples_TracksMembershipCorrectly(
        RandomizedSetOp[] operations, object?[] expected) =>
        RunScript(new InsertDeleteGetRandomO1Solution.RandomizedSetByHashMapSwapRemove(), operations, expected);

    private static void RunScript(
        InsertDeleteGetRandomO1Solution.IRandomizedSet set, RandomizedSetOp[] operations, object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            AssertMatches(expected[i], operations[i].Apply(set));
        }
    }

    // GetRandom's expected slot carries the candidate set of values valid at that
    // point in the script rather than one exact value; int/bool results
    // (Insert/Remove/Count) compare fine as plain boxed objects.
    private static void AssertMatches(object? expected, object? actual)
    {
        if (expected is int[] candidates)
        {
            Assert.Contains(Assert.IsType<int>(actual), candidates);
        }
        else
        {
            Assert.Equal(expected, actual);
        }
    }
}
