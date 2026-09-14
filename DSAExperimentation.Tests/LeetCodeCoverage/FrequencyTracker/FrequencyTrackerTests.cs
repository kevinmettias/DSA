using DSAExperimentation.LeetCode.FrequencyTracker;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FrequencyTracker;

// Harness only. Both strategies are FrequencyTrackerSolution's - this file replays
// LeetCode's published call sequences against each IFrequencyTracker instance, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of add/deleteOne/hasFrequency calls rather than a single argument tuple,
// the same shape AllOneDataStructureTests already uses for its own instance-API
// problem. The pre-migration test only exercised the paired-HashMap strategy; the
// sorted-scan list baseline (previously untested scaffolding inlined in the
// benchmark) is held to the same script here for the first time. FrequencyTrackerOp
// .Apply is pure dispatch, no counting logic of its own.
public sealed class FrequencyTrackerTests
{
    public static TheoryData<FrequencyTrackerOp[], bool?[]> Examples =>
        new()
        {
            // LeetCode example 1.
            {
                [
                    FrequencyTrackerOp.Add(3), FrequencyTrackerOp.Add(3), FrequencyTrackerOp.HasFrequency(2),
                    FrequencyTrackerOp.Add(4), FrequencyTrackerOp.HasFrequency(1),
                ],
                [null, null, true, null, true]
            },

            // LeetCode example 2: the only occurrence is deleted again.
            {
                [FrequencyTrackerOp.Add(1), FrequencyTrackerOp.DeleteOne(1), FrequencyTrackerOp.HasFrequency(1)],
                [null, null, false]
            },

            // LeetCode example 3: a query before anything has been added.
            {
                [FrequencyTrackerOp.HasFrequency(2), FrequencyTrackerOp.Add(3), FrequencyTrackerOp.HasFrequency(1)],
                [false, null, true]
            },

            // Deleting a number that was never added is a no-op, and frequency 0 is
            // never reported as present.
            {
                [FrequencyTrackerOp.DeleteOne(0), FrequencyTrackerOp.HasFrequency(0)],
                [null, false]
            },

            // Dropping back to a previous frequency has to update both buckets: the
            // one being left as well as the one being joined.
            {
                [
                    FrequencyTrackerOp.Add(5), FrequencyTrackerOp.Add(5), FrequencyTrackerOp.HasFrequency(2),
                    FrequencyTrackerOp.DeleteOne(5),
                    FrequencyTrackerOp.HasFrequency(1), FrequencyTrackerOp.HasFrequency(2),
                ],
                [null, null, true, null, true, false]
            },

            // Two distinct numbers sharing one frequency: removing one of them must
            // not retire the bucket the other still occupies.
            {
                [
                    FrequencyTrackerOp.Add(7), FrequencyTrackerOp.Add(8), FrequencyTrackerOp.HasFrequency(1),
                    FrequencyTrackerOp.DeleteOne(7), FrequencyTrackerOp.HasFrequency(1),
                    FrequencyTrackerOp.DeleteOne(8), FrequencyTrackerOp.HasFrequency(1),
                ],
                [null, null, true, null, true, null, false]
            },

            // A number climbing several counts leaves no stale lower bucket behind.
            {
                [
                    FrequencyTrackerOp.Add(9), FrequencyTrackerOp.Add(9), FrequencyTrackerOp.Add(9),
                    FrequencyTrackerOp.HasFrequency(3), FrequencyTrackerOp.HasFrequency(2),
                    FrequencyTrackerOp.HasFrequency(1),
                ],
                [null, null, null, true, false, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySortedScanList_LeetCodeExamples_TracksNumbersAtEachFrequency(
        FrequencyTrackerOp[] operations, bool?[] expected) =>
        RunScript(FrequencyTrackerSolution.CreateBySortedScanList(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPairedHashMaps_LeetCodeExamples_TracksNumbersAtEachFrequency(
        FrequencyTrackerOp[] operations, bool?[] expected) =>
        RunScript(FrequencyTrackerSolution.CreateByPairedHashMaps(), operations, expected);

    private static void RunScript(
        FrequencyTrackerSolution.IFrequencyTracker tracker, FrequencyTrackerOp[] operations, bool?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(tracker));
        }
    }
}

// One call in a FrequencyTracker script: which method to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays. Add/DeleteOne
// return null (no value to compare); HasFrequency returns the actual answer - the
// same null-means-"no return value" convention AllOneOp.Apply uses.
public readonly record struct FrequencyTrackerOp
{
    private readonly Kind _kind;
    private readonly int _argument;

    private FrequencyTrackerOp(Kind kind, int argument)
    {
        _kind = kind;
        _argument = argument;
    }

    public static FrequencyTrackerOp Add(int number) => new(Kind.Add, number);

    public static FrequencyTrackerOp DeleteOne(int number) => new(Kind.DeleteOne, number);

    public static FrequencyTrackerOp HasFrequency(int frequency) => new(Kind.HasFrequency, frequency);

    internal bool? Apply(FrequencyTrackerSolution.IFrequencyTracker tracker)
    {
        switch (_kind)
        {
            case Kind.Add:
                tracker.Add(_argument);
                return null;
            case Kind.DeleteOne:
                tracker.DeleteOne(_argument);
                return null;
            default:
                return tracker.HasFrequency(_argument);
        }
    }

    private enum Kind
    {
        Add,
        DeleteOne,
        HasFrequency,
    }
}
