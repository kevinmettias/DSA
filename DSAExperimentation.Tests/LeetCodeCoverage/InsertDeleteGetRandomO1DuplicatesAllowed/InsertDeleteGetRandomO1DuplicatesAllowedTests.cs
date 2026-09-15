using static DSAExperimentation.LeetCode.InsertDeleteGetRandomO1DuplicatesAllowed.InsertDeleteGetRandomO1DuplicatesAllowedSolution;

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
        RunScript(new RandomizedCollectionByListScan(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomizedCollectionByLinkedOccurrences_LeetCodeExamples_TracksMultiplicitiesCorrectly(
        RandomizedCollectionOp[] operations, object?[] expected) =>
        RunScript(new RandomizedCollectionByLinkedOccurrences(), operations, expected);

    private static void RunScript(
        IRandomizedCollection collection, RandomizedCollectionOp[] operations, object?[] expected)
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

// One call in a RandomizedCollection script: which method to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the call sequence it replays.
public readonly record struct RandomizedCollectionOp(RandomizedCollectionOp.OpKind kind, int value)
{
    public static RandomizedCollectionOp Insert(int value) => new(OpKind.Insert, value);

    public static RandomizedCollectionOp Remove(int value) => new(OpKind.Remove, value);

    public static RandomizedCollectionOp GetRandom() => new(OpKind.GetRandom, 0);

    public static RandomizedCollectionOp Count() => new(OpKind.Count, 0);

    // Boxed uniformly so a script runner can assert against one expected value per
    // operation regardless of which method it dispatches to. Internal, not public:
    // IRandomizedCollection is internal to
    // InsertDeleteGetRandomO1DuplicatesAllowedSolution, and only this same assembly's
    // RunScript ever calls Apply.
    internal object? Apply(IRandomizedCollection collection) => kind switch
    {
        OpKind.Insert => collection.Insert(value),
        OpKind.Remove => collection.Remove(value),
        OpKind.GetRandom => collection.GetRandom(),
        _ => collection.Count,
    };

    public enum OpKind
    {
        Insert,
        Remove,
        GetRandom,
        Count,
    }
}
