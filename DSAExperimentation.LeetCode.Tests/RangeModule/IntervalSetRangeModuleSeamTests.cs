using DSAExperimentation.LeetCode.RangeModule;

namespace DSAExperimentation.LeetCode.Tests.RangeModule;

// The seam between RangeModuleSolution's two range modules.
// CreateByLinearScan keeps a List<(int, int)> of merged half-open ranges and scans
// it per call. CreateByIntervalSetBinarySearch composes
// DataStructures.IntervalSet<int> instead: AddRange is one IntervalSet.Add,
// QueryRange locates its candidate with BinarySearch.UpperBound over an
// IRandomAccessSequence witness of the set's starts, and RemoveRange - which
// IntervalSet has no operation for - rebuilds the set from the surviving fragments.
//
// The rebuild is the risk the two libraries create between them. IntervalSet
// merges on a closed-overlap rule while this problem is half-open throughout, and
// the fragments RemoveRange re-adds are re-merged by that other rule; a fragment
// boundary that IntervalSet decides to join is a range the module will later
// report as covered when it is not. Nothing inside IntervalSet can see that, so
// the linear arm is the reference.
public sealed partial class IntervalSetRangeModuleSeamTests
{
    [Fact]
    public void RangeModule_LeetCodeExample_MatchesLinearScan()
    {
        RangeOperation[] script =
        [
            RangeOperation.Add(10, 20),
            RangeOperation.Remove(14, 16),
            RangeOperation.Query(10, 14),
            RangeOperation.Query(13, 15),
            RangeOperation.Query(16, 17),
        ];

        Assert.Equal(
            ["true", "false", "true"],
            Replay(script, useIntervalSet: true));
        AssertSameAnswers(script);
    }

    // Two ranges that share only a boundary value: half-open says they touch and
    // must become one range, which is exactly the case IntervalSet's closed-interval
    // merge rule has to get right for this problem.
    [Fact]
    public void AddRange_TouchingRanges_MergeIntoOneOnBothArms()
    {
        RangeOperation[] script =
        [
            RangeOperation.Add(10, 20),
            RangeOperation.Add(20, 30),
            RangeOperation.Query(19, 21),
            RangeOperation.Remove(20, 21),
            RangeOperation.Query(19, 21),
        ];

        AssertSameAnswers(script);
    }

    // Overlapping adds, where the merge has to widen one stored range rather than
    // add a second one the queries would then have to walk past.
    [Fact]
    public void AddRange_OverlappingRanges_MatchesLinearScan()
    {
        RangeOperation[] script =
        [
            RangeOperation.Add(10, 20),
            RangeOperation.Add(15, 25),
            RangeOperation.Query(10, 25),
            RangeOperation.Query(24, 26),
            RangeOperation.Add(1, 9),
            RangeOperation.Query(1, 10),
        ];

        AssertSameAnswers(script);
    }

    // A removal that splits a range and a later add that re-bridges the gap: the
    // IntervalSet arm has rebuilt its whole set in between, so the re-bridge is a
    // merge into a set whose shape no longer resembles the one it started from.
    [Fact]
    public void Remove_ThenReAddAcrossTheGap_MatchesLinearScan()
    {
        RangeOperation[] script =
        [
            RangeOperation.Add(10, 30),
            RangeOperation.Remove(14, 16),
            RangeOperation.Query(13, 17),
            RangeOperation.Add(14, 16),
            RangeOperation.Query(13, 17),
            RangeOperation.Remove(10, 30),
            RangeOperation.Query(10, 30),
        ];

        AssertSameAnswers(script);
    }

    [Fact]
    public void Query_OnAnEmptyModule_AnswersFalseOnBothArms()
    {
        RangeOperation[] script = [RangeOperation.Query(0, 1), RangeOperation.Query(100, 200)];

        AssertSameAnswers(script);
    }

    // The shortest range the module can be asked about, where a one-wide half-open
    // window is covered only if the set's start is strictly below it.
    [Fact]
    public void Query_SingleWideRanges_MatchesLinearScan()
    {
        RangeOperation[] script =
        [
            RangeOperation.Add(5, 6),
            RangeOperation.Query(5, 6),
            RangeOperation.Query(6, 7),
            RangeOperation.Query(4, 5),
        ];

        AssertSameAnswers(script);
    }

    [Fact]
    public void AddRange_InterleavedWithRemovals_MatchesLinearScan()
    {
        RangeOperation[] script =
        [
            RangeOperation.Add(1, 10),
            RangeOperation.Add(20, 30),
            RangeOperation.Add(40, 50),
            RangeOperation.Remove(5, 45),
            RangeOperation.Query(1, 5),
            RangeOperation.Query(5, 20),
            RangeOperation.Query(45, 50),
            RangeOperation.Add(5, 45),
            RangeOperation.Query(0, 60),
        ];

        AssertSameAnswers(script);
    }

    private static void AssertSameAnswers(RangeOperation[] script)
        => Assert.Equal(Replay(script, useIntervalSet: false), Replay(script, useIntervalSet: true));

    private static List<string> Replay(RangeOperation[] script, bool useIntervalSet)
    {
        var module = useIntervalSet
            ? RangeModuleSolution.CreateByIntervalSetBinarySearch()
            : RangeModuleSolution.CreateByLinearScan();
        var transcript = new List<string>();

        foreach (var operation in script)
        {
            switch (operation.Kind)
            {
                case RangeOperationKind.Add:
                    module.AddRange(operation.Left, operation.Right);
                    break;
                case RangeOperationKind.Remove:
                    module.RemoveRange(operation.Left, operation.Right);
                    break;
                default:
                    transcript.Add(module.QueryRange(operation.Left, operation.Right) ? "true" : "false");
                    break;
            }
        }

        return transcript;
    }

    private enum RangeOperationKind
    {
        Add,
        Remove,
        Query,
    }

    private readonly record struct RangeOperation(RangeOperationKind Kind, int Left, int Right)
    {
        public static RangeOperation Add(int left, int right) => new(RangeOperationKind.Add, left, right);

        public static RangeOperation Remove(int left, int right) => new(RangeOperationKind.Remove, left, right);

        public static RangeOperation Query(int left, int right) => new(RangeOperationKind.Query, left, right);
    }
}
