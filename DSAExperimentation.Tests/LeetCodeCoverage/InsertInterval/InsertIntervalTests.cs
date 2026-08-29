using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertInterval;

// LeetCode 57. Insert Interval: IntervalSet<int> already maintains the sorted,
// merged closed-interval invariant this problem asks for after one Add.
public sealed partial class InsertIntervalTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 6, 9 }, new[] { 2, 5 }, new[] { 1, 5, 6, 9 })]
    [InlineData(new[] { 1, 2, 3, 5, 6, 7, 8, 10, 12, 16 }, new[] { 4, 8 }, new[] { 1, 2, 3, 10, 12, 16 })]
    public void Insert_LeetCodeExamples_ReturnsMergedIntervals(int[] flatIntervals, int[] newInterval, int[] expectedFlat)
        => Assert.Equal(expectedFlat, Insert(flatIntervals, newInterval).SelectMany(x => new[] { x.Start, x.End }).ToArray());

    private static List<(int Start, int End)> Insert(int[] flatIntervals, int[] newInterval)
    {
        var set = new IntervalSet<int>();
        for (var i = 0; i < flatIntervals.Length; i += 2)
        {
            set.Add(flatIntervals[i], flatIntervals[i + 1]);
        }

        set.Add(newInterval[0], newInterval[1]);
        return Enumerable.Range(0, set.Count).Select(set.Get).ToList();
    }
}
