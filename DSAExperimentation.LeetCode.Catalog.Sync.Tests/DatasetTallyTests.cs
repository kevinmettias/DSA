using DSAExperimentation.LeetCode.Catalog.Sync;
using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests;

public sealed partial class DatasetTallyTests
{
    private static readonly LeetCodeCase KeptCase = new() { Inputs = ["1"], ExpectedOutput = "1" };

    [Fact]
    public void Add_ConversionsWithAndWithoutCases_CountsOnlyCoveredQuestions()
    {
        var report = ReportOf(
            new DatasetConversion([KeptCase, KeptCase], new Dictionary<DatasetDropReason, int>()),
            DatasetConversion.None);

        Assert.StartsWith("2 cases kept for 1 questions.", report);
    }

    [Fact]
    public void Report_DropsFromSeveralConversions_SumsEachReasonLargestFirst()
    {
        var report = ReportOf(
            new DatasetConversion([], new Dictionary<DatasetDropReason, int> { [DatasetDropReason.Duplicate] = 1 }),
            new DatasetConversion([], new Dictionary<DatasetDropReason, int>
            {
                [DatasetDropReason.Duplicate] = 1,
                [DatasetDropReason.OutOfBounds] = 5,
            }));

        var lines = report.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(["  dropped 5: OutOfBounds", "  dropped 2: Duplicate"], lines[1..]);
    }

    private static string ReportOf(params DatasetConversion[] conversions)
    {
        var tally = new DatasetTally();
        var output = new StringWriter();

        foreach (var conversion in conversions)
        {
            tally.Add(conversion);
        }

        tally.Report(output);
        return output.ToString();
    }
}
