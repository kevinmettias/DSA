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
public sealed partial class FrequencyTrackerTests
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
        Assert.Equal(expected, RunScript(FrequencyTrackerSolution.CreateBySortedScanList(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPairedHashMaps_LeetCodeExamples_TracksNumbersAtEachFrequency(
        FrequencyTrackerOp[] operations, bool?[] expected) =>
        Assert.Equal(expected, RunScript(FrequencyTrackerSolution.CreateByPairedHashMaps(), operations));

    private static bool?[] RunScript(
        FrequencyTrackerSolution.IFrequencyTracker tracker, FrequencyTrackerOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(tracker))];

    // One call in a FrequencyTracker script: which method to invoke and with what
    // argument. Pure dispatch, built via the named factories below so a script (like
    // Examples above) reads like the LeetCode call sequence it replays. Add/DeleteOne
    // return null (no value to compare); HasFrequency returns the actual answer - the
    // same null-means-"no return value" convention AllOneOp.Apply uses. Nested here
    // rather than left at file scope so the file declares exactly one type.
    public readonly record struct FrequencyTrackerOp(FrequencyTrackerOp.OpKind kind, int argument)
    {
        public static FrequencyTrackerOp Add(int number) => new(OpKind.Add, number);

        public static FrequencyTrackerOp DeleteOne(int number) => new(OpKind.DeleteOne, number);

        public static FrequencyTrackerOp HasFrequency(int frequency) => new(OpKind.HasFrequency, frequency);

        internal bool? Apply(FrequencyTrackerSolution.IFrequencyTracker tracker)
        {
            switch (kind)
            {
                case OpKind.Add:
                    tracker.Add(argument);
                    return null;
                case OpKind.DeleteOne:
                    tracker.DeleteOne(argument);
                    return null;
                default:
                    return tracker.HasFrequency(argument);
            }
        }

        public enum OpKind
        {
            Add,
            DeleteOne,
            HasFrequency,
        }
    }
}
