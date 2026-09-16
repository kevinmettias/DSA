using DSAExperimentation.LeetCode.InsertDeleteGetRandomO1DuplicatesAllowed;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertDeleteGetRandomO1DuplicatesAllowed;

// Harness only. Both strategies are InsertDeleteGetRandomO1DuplicatesAllowedSolution's
// - this file replays the original test's three call sequences (the LeetCode-published
// script, removing one of two duplicates, and re-inserting a value after it has been
// fully removed) against each IRandomizedCollection implementation via a small
// operation script, so a failure still names the strategy that broke. GetRandom's
// result is nondeterministic and depends on each strategy's own internal array order,
// so its expected slot carries the set of values that are valid to return at that
// point in the script rather than one exact value - the same "expected slot shape
// depends on which call it answers" idea ImplementRouterTests already uses for
// ForwardPacket's int[]. RandomizedCollectionOp.Apply is pure dispatch - no
// multiplicity-tracking logic of its own.
public sealed class InsertDeleteGetRandomO1DuplicatesAllowedTests
{
    public static TheoryData<RandomizedCollectionOp[], object?[]> Examples =>
        new()
        {
            {
                [
                    RandomizedCollectionOp.Insert(1),
                    RandomizedCollectionOp.Insert(1),
                    RandomizedCollectionOp.Insert(2),
                    RandomizedCollectionOp.Count(),
                    RandomizedCollectionOp.GetRandom(),
                    RandomizedCollectionOp.Remove(1),
                    RandomizedCollectionOp.Count(),
                ],
                [true, false, true, 3, new[] { 1, 2 }, true, 2]
            },
            {
                [
                    RandomizedCollectionOp.Insert(5),
                    RandomizedCollectionOp.Insert(5),
                    RandomizedCollectionOp.Insert(7),
                    RandomizedCollectionOp.Remove(5),
                    RandomizedCollectionOp.Count(),
                    RandomizedCollectionOp.Remove(5),
                    RandomizedCollectionOp.Remove(5),
                    RandomizedCollectionOp.Count(),
                    RandomizedCollectionOp.GetRandom(),
                ],
                [true, false, true, true, 2, true, false, 1, new[] { 7 }]
            },
            {
                [
                    RandomizedCollectionOp.Insert(9),
                    RandomizedCollectionOp.Remove(9),
                    RandomizedCollectionOp.Insert(9),
                ],
                [true, true, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomizedCollectionByListScan_LeetCodeExamples_TracksMultiplicitiesCorrectly(
        RandomizedCollectionOp[] operations, object?[] expected) =>
        RunScript(
            new InsertDeleteGetRandomO1DuplicatesAllowedSolution.RandomizedCollectionByListScan(),
            operations,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomizedCollectionByLinkedOccurrences_LeetCodeExamples_TracksMultiplicitiesCorrectly(
        RandomizedCollectionOp[] operations, object?[] expected) =>
        RunScript(
            new InsertDeleteGetRandomO1DuplicatesAllowedSolution.RandomizedCollectionByLinkedOccurrences(),
            operations,
            expected);

    private static void RunScript(
        InsertDeleteGetRandomO1DuplicatesAllowedSolution.IRandomizedCollection collection,
        RandomizedCollectionOp[] operations,
        object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            AssertMatches(expected[i], operations[i].Apply(collection));
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
