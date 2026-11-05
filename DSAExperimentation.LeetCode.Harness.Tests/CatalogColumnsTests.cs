using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.ClimbingStairs;
using DSAExperimentation.LeetCode.Solutions.MergeIntervals;
using DSAExperimentation.LeetCode.Solutions.TwoSum;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogColumnsTests
{
    private const string ClimbingStairsSlug = "climbing-stairs";

    [Fact]
    public void For_SolutionWithNoPromise_KeepsEveryCaseInCatalogOrder() =>
        Assert.Equal(
            LeetCodeCatalog.Cases(ClimbingStairsSlug).Select(InputTextOf),
            CatalogColumns.For(typeof(ClimbingStairsSolution), typeof(int)).Cases.Select(InputTextOf));

    // LC 1 promises exactly one pair, and LeetCodeDataset's generated inputs do not always keep it.
    [Fact]
    public void For_SolutionWithAPromise_KeepsOnlyTheCasesThatKeepIt()
    {
        var columns = CatalogColumns.For(typeof(TwoSumSolution), typeof(int[]), typeof(int));
        var nums = columns.Column<int[]>(0);
        var targets = columns.Column<int>(1);

        Assert.All(Enumerable.Range(0, columns.Count), index => Assert.True(TwoSumPromise.IsKeptBy(nums[index], targets[index])));
    }

    [Fact]
    public void For_FewerTypesThanACaseHasInputs_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => CatalogColumns.For(typeof(TwoSumSolution), typeof(int[])));

    [Fact]
    public void For_RecordsStandingForSeveralInputs_ReadsConsecutiveInputsIntoEach()
    {
        var columns = CatalogColumns.For(typeof(SpreadProbeSolution), typeof(ProbeCorners), typeof(ProbeCorners));
        var inputs = columns.Cases[0].Inputs.Select(int.Parse).ToArray();

        Assert.Equal(new ProbeCorners(inputs[4], inputs[5], inputs[6], inputs[7]), columns.Column<ProbeCorners>(1)[0]);
    }

    [Fact]
    public void Column_IntervalsAsPairs_ReadsEachIntervalAsAStartAndAnEnd()
    {
        var columns = CatalogColumns.For(typeof(MergeIntervalsSolution), typeof(IEnumerable<(int Start, int End)>));
        var first = LeetCodeCatalog.Cases("merge-intervals")[0].Input<int[][]>(0);

        Assert.Equal(first.Select(row => (row[0], row[1])), columns.Column<IEnumerable<(int Start, int End)>>(0)[0]);
    }

    [Fact]
    public void ValueAt_AnyCase_IsTheValueItsColumnHolds()
    {
        var columns = CatalogColumns.For(typeof(ClimbingStairsSolution), typeof(int));

        Assert.Equal(columns.Column<int>(0)[0], columns.ValueAt(0, 0));
    }

    [Fact]
    public void Count_AnyEnrolledSolution_CountsItsCases()
    {
        var columns = CatalogColumns.For(typeof(ClimbingStairsSolution), typeof(int));

        Assert.Equal(columns.Cases.Count, columns.Count);
    }

    [Fact]
    public void QuestionOf_EnrolledSolution_NamesItsQuestion() =>
        Assert.Equal(ClimbingStairsSlug, CatalogColumns.QuestionOf(typeof(ClimbingStairsSolution)).TitleSlug);

    [Fact]
    public void QuestionOf_TypeNamingNoQuestion_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => CatalogColumns.QuestionOf(typeof(CatalogColumnsTests)));

    // A case's inputs as one text: LeetCodeCase holds them as a list, which compares by reference.
    private static string InputTextOf(LeetCodeCase testCase) => string.Join(LeetCodeNotation.InputSeparator, testCase.Inputs);
}
