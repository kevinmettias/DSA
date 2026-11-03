using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.Solutions.Tests;

// Every strategy of every solution that names its LeetCode question, run over every case the LeetCode
// catalog holds for that question and judged against the recorded output by the question's order rule
// (ARCHITECTURE 17.13). It is the one test those strategies need for LeetCode's cases. A solution's own
// <Problem>SolutionTests keeps only what the catalog cannot state, such as an input outside the question's
// promise, and LeetCodeStrategyCoverageTests counts each strategy run here as named. A failure lists the
// strategy, the input, the recorded output and the answer.
public sealed partial class CatalogCaseTests
{
    // How many wrong answers a failure lists before it stops.
    private const int MostListed = 5;

    public static TheoryData<string, string> EnrolledStrategies
    {
        get
        {
            var rows = new TheoryData<string, string>();

            foreach (var solution in CatalogStrategy.EnrolledSolutions())
            {
                foreach (var strategy in CatalogStrategy.Of(solution))
                {
                    rows.Add(solution.Name, strategy.Name);
                }
            }

            return rows;
        }
    }

    // The theory below has one row per enrolled strategy, so a discovery that found none would report it
    // green while checking nothing.
    [Fact]
    public void EnrolledStrategies_AfterDiscovery_AreFound() => Assert.NotEmpty(EnrolledStrategies);

    [Theory]
    [MemberData(nameof(EnrolledStrategies))]
    public void Strategy_EveryCatalogCase_AnswersWhatTheCatalogRecords(string solutionName, string strategyName)
    {
        var solution = CatalogStrategy.EnrolledSolutions().Single(type => type.Name == solutionName);
        var strategy = CatalogStrategy.Of(solution).Single(candidate => candidate.Name == strategyName);
        var columns = strategy.Columns();
        var order = CatalogColumns.QuestionOf(solution).AnswerOrder;
        var wrong = CatalogJudge.WrongAnswers(strategy.Name, order, columns.Cases, index => strategy.AnswerTo(columns, index));

        Assert.True(wrong.Count == 0, string.Join(Environment.NewLine, wrong.Take(MostListed)));
    }
}
