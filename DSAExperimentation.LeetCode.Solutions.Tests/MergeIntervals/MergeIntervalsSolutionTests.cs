using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.MergeIntervals;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MergeIntervals;

// Harness only. Both strategies are MergeIntervalsSolution's; IntervalSet<TKey>'s
// own merge invariant is covered directly by
// DSAExperimentation.Tests/DataStructures/IntervalSet/IntervalSetTests.cs - this
// file pins the two strategies to every case the LeetCode catalog holds for LC 56, its
// published examples and then LeetCodeDataset's. LeetCode writes an interval as a two-element
// row and the strategies take (start, end) pairs, so each case is translated once, here.
public sealed partial class MergeIntervalsSolutionTests
{
    private const string TitleSlug = "merge-intervals";

    public static TheoryData<(int Start, int End)[], (int Start, int End)[]> Cases
    {
        get
        {
            var cases = new TheoryData<(int Start, int End)[], (int Start, int End)[]>();

            foreach (var testCase in LeetCodeCatalog.Cases(TitleSlug).Where(testCase => testCase.HasExpectedOutput))
            {
                cases.Add(PairsOf(testCase.Input<int[][]>(0)), PairsOf(testCase.Expected<int[][]>()));
            }

            return cases;
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MergeByBatchSortAndMerge_LeetCodeCases_ReturnsMergedDisjointIntervals(
        (int Start, int End)[] intervals, (int Start, int End)[] expected) =>
        Assert.Equal(expected, MergeIntervalsSolution.MergeByBatchSortAndMerge(intervals));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MergeByIntervalSet_LeetCodeCases_ReturnsMergedDisjointIntervals(
        (int Start, int End)[] intervals, (int Start, int End)[] expected) =>
        Assert.Equal(expected, MergeIntervalsSolution.MergeByIntervalSet(intervals));

    private static (int Start, int End)[] PairsOf(int[][] rows) => [.. rows.Select(row => (row[0], row[1]))];
}
