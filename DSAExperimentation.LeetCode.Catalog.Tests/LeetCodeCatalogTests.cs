using System.Text.Json;
using DSAExperimentation.LeetCode.Catalog.Bounds;
using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests;

// The checked-in snapshot as a whole: what every refresh must leave true of Snapshot/, so that a sync
// which writes something malformed fails here, in the pull request that carries it, rather than
// in whichever problem's harness reads the bad file first.
public sealed partial class LeetCodeCatalogTests
{
    private const string TwoSum = "two-sum";
    private const string MaximumPalindromeProduct = "maximum-product-of-the-length-of-two-palindromic-substrings";

    [Fact]
    public void TitleSlugs_EveryQuestionFile_LoadsUnderItsOwnSlug()
    {
        var misfiled = LeetCodeCatalog.TitleSlugs().Where(slug => LeetCodeCatalog.Question(slug).TitleSlug != slug);

        Assert.Empty(misfiled);
    }

    [Fact]
    public void Problem_EveryEnrolledQuestion_HasStatisticsWithAnAcceptanceRateBetweenNoneAndAll()
    {
        var rates = LeetCodeCatalog.Problems().Select(problem => problem.Statistics.AcceptanceRatePercent);

        Assert.All(rates, rate => Assert.InRange(rate, 0, 100));
    }

    [Fact]
    public void ContestRatings_EveryRatedQuestion_IsEnrolled()
    {
        var enrolled = LeetCodeCatalog.TitleSlugs().ToHashSet(StringComparer.Ordinal);

        Assert.All(ContestRatingsFile.Read().Questions.Keys, slug => Assert.Contains(slug, enrolled));
    }

    [Fact]
    public void Statistics_EveryEntry_IsAnEnrolledQuestion()
    {
        var enrolled = LeetCodeCatalog.TitleSlugs().ToHashSet(StringComparer.Ordinal);

        Assert.All(StatisticsFile.Read().Questions.Keys, slug => Assert.Contains(slug, enrolled));
    }

    [Fact]
    public void Cases_EveryGeneratedCase_HasOneInputPerJudgeParameter()
    {
        var misshapen = GeneratedCases()
            .Where(entry => entry.Case.Inputs.Count != entry.Question.Signature.Parameters.Count)
            .Select(entry => entry.Question.TitleSlug)
            .Distinct();

        Assert.Empty(misshapen);
    }

    [Fact]
    public void Cases_EveryGeneratedCase_IsLeetCodeNotationThroughout()
    {
        var unreadable = GeneratedCases()
            .Where(entry => !IsNotation(entry.Case))
            .Select(entry => entry.Question.TitleSlug)
            .Distinct();

        Assert.Empty(unreadable);
    }

    [Fact]
    public void Cases_EveryGeneratedCase_StaysInsideTheQuestionsStatedBounds()
    {
        var outOfBounds = GeneratedCases()
            .Where(entry => IsOutOfBounds(entry.Question, entry.Case))
            .Select(entry => entry.Question.TitleSlug)
            .Distinct();

        Assert.Empty(outOfBounds);
    }

    [Fact]
    public void Cases_TwoSum_ListsExamplesBeforeGeneratedCases()
    {
        var sources = LeetCodeCatalog.Cases(TwoSum).Select(testCase => testCase.Source).ToList();

        Assert.Equal(sources.Order(), sources);
        Assert.Equal(LeetCodeCaseSource.Example, sources[0]);
    }

    [Fact]
    public void Cases_CorrectedQuestion_RecordsEachCorrectedOutput()
    {
        var cases = LeetCodeCatalog.Cases(MaximumPalindromeProduct);

        Assert.All(
            CorrectionFiles.Read(MaximumPalindromeProduct),
            correction => Assert.Contains(cases, testCase => testCase.Inputs.SequenceEqual(correction.Inputs) && testCase.ExpectedOutput == correction.ExpectedOutput));
    }

    [Fact]
    public void Problems_EveryProblem_IsOrderedByFrontendNumber()
    {
        var numbers = LeetCodeCatalog.Problems().Select(problem => problem.Question.FrontendId).ToList();

        Assert.Equal(numbers.Order(), numbers);
    }

    [Fact]
    public void StatisticsAsOf_Snapshot_IsTheDayItsFiguresWereRead()
        => Assert.Equal(StatisticsFile.Read().AsOf, LeetCodeCatalog.StatisticsAsOf);

    [Fact]
    public void Question_SlugNotEnrolled_ThrowsNamingTheSyncCommand()
    {
        var exception = Assert.Throws<KeyNotFoundException>(() => LeetCodeCatalog.Question("no-such-question"));

        Assert.Contains("questions no-such-question", exception.Message);
    }

    private static IEnumerable<(LeetCodeQuestion Question, LeetCodeCase Case)> GeneratedCases()
        => LeetCodeCatalog.TitleSlugs()
            .Select(LeetCodeCatalog.Question)
            .SelectMany(question => LeetCodeCatalog.Cases(question.TitleSlug)
                .Where(testCase => testCase.Source != LeetCodeCaseSource.Example)
                .Select(testCase => (question, testCase)));

    private static bool IsNotation(LeetCodeCase testCase)
    {
        var isAnswerNotation = testCase.ExpectedOutput is not { } answer || IsJson(answer);
        return isAnswerNotation && testCase.Inputs.All(IsJson);
    }

    private static bool IsJson(string text)
    {
        try
        {
            using var document = JsonDocument.Parse(text);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsOutOfBounds(LeetCodeQuestion question, LeetCodeCase testCase)
        => LeetCodeBounds.Of(question.Constraints)
            .Any(bound => bound.Check(testCase, question.Signature.Parameters) == LeetCodeBoundCheck.Violated);
}
