using DSAExperimentation.LeetCode.Catalog.Sync;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests;

public sealed partial class QuestionsTallyTests
{
    private const string TwoSum = "two-sum";
    private const string ClimbingStairs = "climbing-stairs";

    private static readonly LeetCodeStatistics Before = new(50, 1, 2, 3, 4);
    private static readonly LeetCodeStatistics After = new(51, 2, 3, 4, 5);

    private readonly StringWriter _output = new();
    private readonly StringWriter _error = new();

    [Fact]
    public void Statistics_NothingRecorded_AreThePreviousFigures()
    {
        var tally = TallyOf(2);

        Assert.Equal((Before, Before), (tally.Statistics[TwoSum], tally.Statistics[ClimbingStairs]));
    }

    [Fact]
    public void Unchanged_QuestionWithPreviousFigures_ReplacesThemAndKeepsTheOthers()
    {
        var tally = TallyOf(2);

        tally.Unchanged(TwoSum, After);

        Assert.Equal((After, Before), (tally.Statistics[TwoSum], tally.Statistics[ClimbingStairs]));
    }

    [Fact]
    public void Unchanged_HundredthQuestion_WritesAProgressLine()
    {
        var tally = TallyOf(250);

        for (var question = 0; question < 100; question++)
        {
            tally.Unchanged(TwoSum, After);
        }

        Assert.Contains("100/250 questions fetched, 0 changed, 0 failed", _output.ToString());
    }

    [Fact]
    public void Rewritten_OneQuestion_CountsItAsChangedAndTakesItsFigures()
    {
        var tally = TallyOf(1);

        tally.Rewritten(TwoSum, After);
        tally.Finish();

        Assert.Equal(After, tally.Statistics[TwoSum]);
        Assert.Contains("1 questions fetched: 1 changed, 0 failed.", _output.ToString());
    }

    [Fact]
    public void Failed_OneQuestion_ReportsItsSlugAndReasonAndKeepsItsOldFigures()
    {
        var tally = TallyOf(1);

        tally.Failed(TwoSum, "connection reset");

        Assert.Contains("two-sum: connection reset", _error.ToString());
        Assert.Equal(Before, tally.Statistics[TwoSum]);
    }

    [Fact]
    public void Renamed_NewSlug_ReportsBothSlugs()
    {
        var tally = TallyOf(1);

        tally.Renamed(TwoSum, "two-sum-renamed");

        Assert.Contains("two-sum: leetcode.com now calls it 'two-sum-renamed'", _error.ToString());
    }

    [Fact]
    public void Finish_EveryQuestionRefreshed_ReturnsZero()
    {
        var tally = TallyOf(1);
        tally.Rewritten(TwoSum, After);

        var exitCode = tally.Finish();

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public void Finish_AQuestionFailed_NamesItAndReturnsOne()
    {
        var tally = TallyOf(2);
        tally.Unchanged(ClimbingStairs, After);
        tally.Failed(TwoSum, "connection reset");

        var exitCode = tally.Finish();

        Assert.Equal(1, exitCode);
        Assert.Contains("Failed: two-sum", _error.ToString());
    }

    private QuestionsTally TallyOf(int total)
    {
        var previous = new Dictionary<string, LeetCodeStatistics> { [TwoSum] = Before, [ClimbingStairs] = Before };
        return new QuestionsTally(total, previous, _output, _error);
    }
}
