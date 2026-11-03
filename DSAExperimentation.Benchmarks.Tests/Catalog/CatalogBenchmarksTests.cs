using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.Benchmarks.Catalog;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.Solutions.Conventions;

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
        Func<object?, JsonNode> notationOf = IsTryShaped(catalog.Solution, benchmark.Baseline) ? AnswerNotation.OfFoundOutputs : AnswerNotation.Of;
        var order = CatalogColumns.QuestionOf(catalog.Solution).AnswerOrder;
        var wrong = CatalogJudge.WrongAnswers(benchmark.Baseline.Name, order, cases, index => notationOf(answers.GetValue(index)));

        Assert.Equal(cases.Count, answers.Length);
        Assert.True(wrong.Count == 0, string.Join(Environment.NewLine, wrong.Take(MostListed)));
    }

    private static IEnumerable<BenchmarkClass> CatalogBenchmarks() =>
        BenchmarkClass.All.Where(benchmark => typeof(ICatalogBenchmarks).IsAssignableFrom(benchmark.Type));

    private static Type SolutionOf(BenchmarkClass benchmark) => ((ICatalogBenchmarks)benchmark.Prepare(benchmark.Baseline)).Solution;

    // A strategy with out parameters answers (found, outputs...), which an arm stores as one tuple.
    private static bool IsTryShaped(Type solution, MethodInfo arm) =>
        solution.GetMethods().Any(method => method.Name == arm.Name && method.GetParameters().Any(parameter => parameter.IsOut));
}
