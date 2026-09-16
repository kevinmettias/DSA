using DSAExperimentation.LeetCode.InsertInterval;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertInterval;

// Harness only. Both strategies are InsertIntervalSolution's; IntervalSet<TKey>'s
// own merge invariant is covered directly by
// DSAExperimentation.Tests/DataStructures/IntervalSet/IntervalSetTests.cs - this
// file just pins the two strategies to LeetCode's published examples.
public sealed class InsertIntervalTests
{
    public static TheoryData<(int Start, int End)[], (int Start, int End), (int Start, int End)[]> Examples =>
        new()
        {
            { [(1, 3), (6, 9)], (2, 5), [(1, 5), (6, 9)] },
            { [(1, 2), (3, 5), (6, 7), (8, 10), (12, 16)], (4, 8), [(1, 2), (3, 10), (12, 16)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void InsertByListSortAndMerge_LeetCodeExamples_ReturnsMergedIntervals(
        (int Start, int End)[] intervals, (int Start, int End) newInterval, (int Start, int End)[] expected)
    {
        var actual = InsertIntervalSolution.InsertByListSortAndMerge(intervals, newInterval);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void InsertByIntervalSet_LeetCodeExamples_ReturnsMergedIntervals(
        (int Start, int End)[] intervals, (int Start, int End) newInterval, (int Start, int End)[] expected)
    {
        var actual = InsertIntervalSolution.InsertByIntervalSet(intervals, newInterval);

        Assert.Equal(expected, actual);
    }
}
