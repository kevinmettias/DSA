using DSAExperimentation.LeetCode.TimeBasedKeyValueStore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TimeBasedKeyValueStore;

// Harness only: both strategies live in TimeBasedKeyValueStoreSolution and are
// replayed against the same call scripts - LeetCode's published Set/Get sequence
// (including the two Gets that fall between stored timestamps), a Get before the
// key's first Set, a Get for a key that was never set at all, and interleaved keys
// whose histories must stay independent.
//
// A script step is one published call: IsSet steps call Set(Key, Value, Timestamp)
// and assert nothing; the rest call Get(Key, Timestamp) and assert Value, which for
// those steps is the answer LeetCode publishes.
public sealed class TimeBasedKeyValueStoreTests
{
    public static TheoryData<(bool IsSet, string Key, string Value, int Timestamp)[]> Examples =>
        new()
        {
            {
                [
                    (true, "foo", "bar", 1),
                    (false, "foo", "bar", 1),
                    (false, "foo", "bar", 3),
                    (true, "foo", "bar2", 4),
                    (false, "foo", "bar2", 4),
                    (false, "foo", "bar2", 5),
                ]
            },
            {
                [
                    (true, "foo", "bar", 5),
                    (false, "foo", "", 1),
                ]
            },
            {
                [
                    (false, "missing", "", 10),
                ]
            },
            {
                [
                    (true, "a", "a1", 1),
                    (true, "b", "b1", 2),
                    (true, "a", "a2", 3),
                    (false, "a", "a1", 2),
                    (false, "b", "b1", 10),
                    (false, "a", "a2", 3),
                    (false, "b", "", 1),
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBinarySearchFloor_LeetCodeExamples_ReturnsFloorTimestampValue(
        (bool IsSet, string Key, string Value, int Timestamp)[] calls) =>
        AssertScript(TimeBasedKeyValueStoreSolution.CreateByBinarySearchFloor(), calls);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearFloorScan_LeetCodeExamples_ReturnsFloorTimestampValue(
        (bool IsSet, string Key, string Value, int Timestamp)[] calls) =>
        AssertScript(TimeBasedKeyValueStoreSolution.CreateByLinearFloorScan(), calls);

    private static void AssertScript(
        TimeBasedKeyValueStoreSolution.ITimeMap store,
        (bool IsSet, string Key, string Value, int Timestamp)[] calls)
    {
        foreach (var (isSet, key, value, timestamp) in calls)
        {
            if (isSet)
            {
                store.Set(key, value, timestamp);
                continue;
            }

            Assert.Equal(value, store.Get(key, timestamp));
        }
    }
}
