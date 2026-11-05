using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.Benchmarks.Catalog;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.Catalog;

// A generated catalog arm's buffer (ARCHITECTURE 17.13) read the way LeetCode reads answers: each case's
// answer in LeetCode's notation, by its question's order rule. CatalogBenchmarksTests judges a baseline
// this way against the catalog, and BenchmarkArmsTests holds the other arms to the baseline this way. So
// arms agree when LeetCode would take either answer for the other: an any-order answer's elements may
// come back in another order, and a tree is its wire-format array, which writes out again any subtree
// one arm shares between answers.
internal static class CatalogArmAnswers
{
    private const string CaseSeparator = "\n";

    public static bool IsCatalogClass(Type benchmark) => typeof(ICatalogBenchmarks).IsAssignableFrom(benchmark);

    // The solution a generated class times. Its constructor runs no setup, so asking costs nothing.
    public static Type SolutionOf(Type benchmark) =>
        (Activator.CreateInstance(benchmark) as ICatalogBenchmarks)?.Solution
            ?? throw new InvalidOperationException($"{benchmark.Name} is not a generated catalog class.");

    // The answer to the case at an index of an arm's answers, in LeetCode's notation: an element of its
    // buffer, or for a design question's arm the script's answer its call buffers hold.
    public static Func<int, JsonNode> NotationOf(Type solution, MethodInfo arm, object? answers)
    {
        if (answers is CatalogCallAnswers scripts)
        {
            return scripts.NotationOf;
        }

        var buffer = BufferOf(arm, answers);
        Func<object?, JsonNode> notationOf = IsTryShaped(solution, arm) ? AnswerNotation.OfFoundOutputs : AnswerNotation.Of;

        return index => notationOf(buffer.GetValue(index));
    }

    // How many cases an arm's answers are for.
    public static int CountOf(MethodInfo arm, object? answers)
    {
        if (answers is CatalogCallAnswers scripts)
        {
            return scripts.Count;
        }

        return BufferOf(arm, answers).Length;
    }

    // Every case's answer in an arm's buffer, one line each, written as CatalogJudge.AgreementTextOf writes
    // it for the question's order rule.
    public static string AgreementTextOf(Type benchmark, MethodInfo arm, object? answers)
    {
        var solution = SolutionOf(benchmark);
        var question = CatalogColumns.QuestionOf(solution);
        var notationOf = NotationOf(solution, arm, answers);
        var indices = Enumerable.Range(0, CountOf(arm, answers));

        if (question.AnswerCheck is { } check)
        {
            var cases = CasesOf(solution, arm);
            return string.Join(CaseSeparator, indices.Select(index => CatalogAnswerCheck.VerdictTextOf(check, cases[index], notationOf(index))));
        }

        var lines = indices.Select(index => CatalogJudge.AgreementTextOf(notationOf(index), question.AnswerOrder, question.Tolerance));
        return string.Join(CaseSeparator, lines);
    }

    // The cases an arm's buffer lines up with, which are its strategy's. Where the statement lets several
    // answers stand, arms may give different ones, so they are held to one another by the answer check's
    // verdict on each of these.
    private static IReadOnlyList<LeetCodeCase> CasesOf(Type solution, MethodInfo arm) =>
        CatalogStrategy.Of(solution).Single(strategy => strategy.Name == arm.Name).Columns().Cases;

    private static Array BufferOf(MethodInfo arm, object? answers) =>
        answers as Array ?? throw new InvalidOperationException($"{arm.Name} returned no buffer of answers.");

    // A strategy with out parameters answers (found, outputs...), which an arm stores as one tuple.
    private static bool IsTryShaped(Type solution, MethodInfo arm) =>
        solution.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Any(method => method.Name == arm.Name && method.GetParameters().Any(parameter => parameter.IsOut));
}
