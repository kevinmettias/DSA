using DSAExperimentation.Benchmarks.Catalog;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.Catalog;

// The generated catalog benchmark classes (ARCHITECTURE 17.13), checked once for all of them, for what is
// theirs rather than their strategies': CatalogCaseTests already holds every strategy to every catalog
// case. Here:
// - each class's baseline buffer lines up with its cases and answers each as the catalog records, which
//   pins the arm's answers to values no arm derived (17.9), while BenchmarkArmsTests holds the other arms
//   to the baseline's answers
// - the classes are exactly the enrolled solutions', so the generator neither skipped one nor wrote one
//   for a solution that names no question
// - a class's arms are exactly the strategies the harness runs for its solution.
public sealed partial class CatalogBenchmarksTests
{
    // How many wrong answers a failure lists before it stops.
    private const int MostListed = 5;

    public static TheoryData<string> CatalogClasses => new(CatalogBenchmarks().Select(benchmark => benchmark.Name));

    [Fact]
    public void CatalogClasses_AfterGeneration_AreOnePerEnrolledSolution()
    {
        var enrolled = CatalogStrategy.EnrolledSolutions().Select(solution => solution.FullName);
        var generated = CatalogBenchmarks().Select(benchmark => SolutionOf(benchmark).FullName).Order(StringComparer.Ordinal);

        Assert.NotEmpty(enrolled);
        Assert.Equal(enrolled, generated);
    }

    [Theory]
    [MemberData(nameof(CatalogClasses))]
    public void Arms_GeneratedClass_AreItsSolutionsStrategiesInOrder(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);

        Assert.Equal(CatalogStrategy.Of(SolutionOf(benchmark)).Select(strategy => strategy.Name), benchmark.Arms.Select(arm => arm.Name));
    }

    [Theory]
    [MemberData(nameof(CatalogClasses))]
    public void Baseline_EveryCatalogCase_AnswersWhatTheCatalogRecords(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);
        var harness = benchmark.Prepare(benchmark.Baseline);
        var answers = benchmark.Run(harness, benchmark.Baseline) as Array
            ?? throw new InvalidOperationException($"{benchmark.Baseline.Name} returned no buffer of answers.");
        var catalog = (ICatalogBenchmarks)harness;
        var cases = catalog.CasesOf(benchmark.Baseline.Name);
        var order = CatalogColumns.QuestionOf(catalog.Solution).AnswerOrder;
        var notationOf = CatalogArmAnswers.NotationOf(catalog.Solution, benchmark.Baseline, answers);
        var wrong = CatalogJudge.WrongAnswers(benchmark.Baseline.Name, order, cases, notationOf);

        Assert.Equal(cases.Count, answers.Length);
        Assert.True(wrong.Count == 0, string.Join(Environment.NewLine, wrong.Take(MostListed)));
    }

    private static IEnumerable<BenchmarkClass> CatalogBenchmarks() =>
        BenchmarkClass.All.Where(benchmark => CatalogArmAnswers.IsCatalogClass(benchmark.Type));

    private static Type SolutionOf(BenchmarkClass benchmark) => CatalogArmAnswers.SolutionOf(benchmark.Type);
}
