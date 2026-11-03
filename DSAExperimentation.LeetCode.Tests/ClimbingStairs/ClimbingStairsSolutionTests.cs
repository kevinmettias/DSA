using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.ClimbingStairs;

namespace DSAExperimentation.LeetCode.Tests.ClimbingStairs;

// Harness only. ClimbingStairsSolution owns both strategies; this file pins each to every case the
// LeetCode catalog holds for LC 70 - its published examples, then LeetCodeDataset's, which between
// them reach the constraint's 45 steps.
public sealed partial class ClimbingStairsSolutionTests
{
    private const string TitleSlug = "climbing-stairs";

    public static TheoryData<int, int> Cases
    {
        get
        {
            var cases = new TheoryData<int, int>();

            foreach (var testCase in LeetCodeCatalog.Cases(TitleSlug).Where(testCase => testCase.HasExpectedOutput))
            {
                cases.Add(testCase.Input<int>(0), testCase.Expected<int>());
            }

            return cases;
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByMemoizedRecurrence_LeetCodeCases_ReturnsDistinctClimbSequenceCount(
        int stepCount, int expected) =>
        Assert.Equal(expected, ClimbingStairsSolution.CountWaysByMemoizedRecurrence(stepCount));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByIterativeRollingTotals_LeetCodeCases_ReturnsDistinctClimbSequenceCount(
        int stepCount, int expected) =>
        Assert.Equal(expected, ClimbingStairsSolution.CountWaysByIterativeRollingTotals(stepCount));
}
