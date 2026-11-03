using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.Benchmarks.Catalog;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Catalog;

// The generated catalog benchmark classes (ARCHITECTURE 17.13), checked once for all of them. Each
// class's every arm answers every catalog case with the output the catalog records, judged by the order
// rule its solution states - which pins the arms' answers to values no arm derived (17.9), and is what a
// hand-written <Problem>BenchmarksTests did for one class at a time. And the classes are exactly the
// enrolled solutions': the generator neither skipped one nor wrote one for a solution that names no
// question. BenchmarkArmsTests holds them, as every class, to reproducing their workload and their
// answers.
public sealed partial class CatalogBenchmarksTests
{
    // How many wrong answers a failure lists before it stops.
    private const int MostListed = 5;

    private const string InputSeparator = " | ";
    private const string BareNullOutput = "A recorded output is never a bare null.";

    public static TheoryData<string> CatalogClasses => new(CatalogBenchmarks().Select(benchmark => benchmark.Name));

    [Fact]
    public void CatalogClasses_AfterGeneration_AreOnePerEnrolledSolution()
    {
        var enrolled = typeof(LeetCodeQuestionAttribute).Assembly.GetTypes()
            .Where(type => type.IsDefined(typeof(LeetCodeQuestionAttribute)))
            .Select(type => type.FullName)
            .Order(StringComparer.Ordinal);
        var generated = CatalogBenchmarks()
            .Select(benchmark => ((ICatalogBenchmarks)benchmark.Prepare(benchmark.Baseline)).Solution.FullName)
            .Order(StringComparer.Ordinal);

        Assert.NotEmpty(enrolled);
        Assert.Equal(enrolled, generated);
    }

    [Theory]
    [MemberData(nameof(CatalogClasses))]
    public void EveryArm_EveryCatalogCase_AnswersWhatTheCatalogRecords(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);
        var wrong = benchmark.Arms.SelectMany(arm => WrongAnswersOf(benchmark, arm)).Take(MostListed).ToList();

        Assert.True(wrong.Count == 0, string.Join(Environment.NewLine, wrong));
    }

    private static IEnumerable<BenchmarkClass> CatalogBenchmarks() =>
        BenchmarkClass.All.Where(benchmark => typeof(ICatalogBenchmarks).IsAssignableFrom(benchmark.Type));

    private static IEnumerable<string> WrongAnswersOf(BenchmarkClass benchmark, MethodInfo arm)
    {
        var harness = benchmark.Prepare(arm);
        var answers = benchmark.Run(harness, arm) as Array
            ?? throw new InvalidOperationException($"{arm.Name} returned no buffer of answers.");
        var catalog = (ICatalogBenchmarks)harness;
        var cases = catalog.CasesOf(arm.Name);
        var order = CatalogColumns.QuestionOf(catalog.Solution).AnswerOrder;
        Func<object?, JsonNode> notationOf = IsTryShaped(catalog.Solution, arm) ? AnswerNotation.OfFoundOutputs : AnswerNotation.Of;
        var wrong = new List<string>();

        Assert.Equal(cases.Count, answers.Length);

        for (var index = 0; index < cases.Count; index++)
        {
            if (cases[index].ExpectedOutput is not { } recorded)
            {
                continue;
            }

            var expected = JsonNode.Parse(recorded) ?? throw new InvalidOperationException(BareNullOutput);
            var actual = notationOf(answers.GetValue(index));

            if (!AnswerNotation.IsAccepted(actual, expected, order))
            {
                wrong.Add($"{arm.Name} on {string.Join(InputSeparator, cases[index].Inputs)}: the catalog records {expected.ToJsonString()}, and it answered {actual.ToJsonString()}.");
            }
        }

        return wrong;
    }

    // A strategy with out parameters answers (found, outputs...), which an arm stores as one tuple.
    private static bool IsTryShaped(Type solution, MethodInfo arm) =>
        solution.GetMethods().Any(method => method.Name == arm.Name && method.GetParameters().Any(parameter => parameter.IsOut));
}
