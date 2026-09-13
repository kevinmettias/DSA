using DSAExperimentation.LeetCode.SnapshotArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SnapshotArray;

// Harness only: both strategies live in SnapshotArraySolution and are replayed
// against the same call scripts - LeetCode's published Set/Snap/Set/Get sequence,
// a Get for an index never written before the queried snapshot, three snapshots of
// one index each reporting their own value, two indices whose histories must stay
// independent, and two Sets inside the same snapshot where the later write wins.
//
// A script step is one published call: Set steps call Set(Index, Argument) and
// assert nothing; Snap steps call Snap() and assert the snap id it hands back; Get
// steps call Get(Index, Argument) and assert the value, which for those steps is
// the answer LeetCode publishes.
public sealed class SnapshotArrayTests
{
    public enum SnapshotCall
    {
        Set,
        Snap,
        Get,
    }

    public static TheoryData<int, (SnapshotCall Call, int Index, int Argument, int Expected)[]> Examples =>
        new()
        {
            {
                3,
                [
                    (SnapshotCall.Set, 0, 5, 0),
                    (SnapshotCall.Snap, 0, 0, 0),
                    (SnapshotCall.Set, 0, 6, 0),
                    (SnapshotCall.Get, 0, 0, 5),
                ]
            },
            {
                3,
                [
                    (SnapshotCall.Snap, 0, 0, 0),
                    (SnapshotCall.Set, 1, 9, 0),
                    (SnapshotCall.Get, 1, 0, 0),
                    (SnapshotCall.Snap, 0, 0, 1),
                    (SnapshotCall.Get, 1, 1, 9),
                ]
            },
            {
                1,
                [
                    (SnapshotCall.Set, 0, 1, 0),
                    (SnapshotCall.Snap, 0, 0, 0),
                    (SnapshotCall.Set, 0, 2, 0),
                    (SnapshotCall.Snap, 0, 0, 1),
                    (SnapshotCall.Set, 0, 3, 0),
                    (SnapshotCall.Snap, 0, 0, 2),
                    (SnapshotCall.Get, 0, 0, 1),
                    (SnapshotCall.Get, 0, 1, 2),
                    (SnapshotCall.Get, 0, 2, 3),
                ]
            },
            {
                2,
                [
                    (SnapshotCall.Set, 0, 7, 0),
                    (SnapshotCall.Snap, 0, 0, 0),
                    (SnapshotCall.Set, 1, 8, 0),
                    (SnapshotCall.Snap, 0, 0, 1),
                    (SnapshotCall.Get, 0, 0, 7),
                    (SnapshotCall.Get, 0, 1, 7),
                    (SnapshotCall.Get, 1, 0, 0),
                    (SnapshotCall.Get, 1, 1, 8),
                ]
            },
            {
                1,
                [
                    (SnapshotCall.Set, 0, 4, 0),
                    (SnapshotCall.Set, 0, 6, 0),
                    (SnapshotCall.Snap, 0, 0, 0),
                    (SnapshotCall.Get, 0, 0, 6),
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBinarySearchFloor_LeetCodeExamples_ReturnsValueAtSnapshotTime(
        int length,
        (SnapshotCall Call, int Index, int Argument, int Expected)[] calls) =>
        AssertScript(SnapshotArraySolution.CreateByBinarySearchFloor(length), calls);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearFloorScan_LeetCodeExamples_ReturnsValueAtSnapshotTime(
        int length,
        (SnapshotCall Call, int Index, int Argument, int Expected)[] calls) =>
        AssertScript(SnapshotArraySolution.CreateByLinearFloorScan(length), calls);

    private static void AssertScript(
        SnapshotArraySolution.ISnapshotArray snapshotArray,
        (SnapshotCall Call, int Index, int Argument, int Expected)[] calls)
    {
        foreach (var (call, index, argument, expected) in calls)
        {
            switch (call)
            {
                case SnapshotCall.Set:
                    snapshotArray.Set(index, argument);
                    break;
                case SnapshotCall.Snap:
                    Assert.Equal(expected, snapshotArray.Snap());
                    break;
                default:
                    Assert.Equal(expected, snapshotArray.Get(index, argument));
                    break;
            }
        }
    }
}
