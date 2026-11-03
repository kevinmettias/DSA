using System.Reflection;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Harness;

// The catalog's cases for one enrolled solution, read into the parameter types of one of its strategies
// and laid out by column: Column<TValue>(k) holds every case's k-th input as a TValue[]. A generated arm
// then indexes plain arrays and calls its strategy directly, and every conversion out of LeetCode's
// notation is paid here, in [GlobalSetup], never in the timed loop. A case the question's promise rules
// out is dropped (LeetCodeQuestionAttribute.Promise), so each case left has the one answer the catalog
// records for it, or none recorded at all. The columns are indexed by parameter, then by case.
internal sealed class CatalogColumns(IReadOnlyList<LeetCodeCase> cases, object?[][] columns)
{
    // The method a promise type states its promise with, as TwoSumPromise.IsKeptBy does.
    private const string PromiseMethod = "IsKeptBy";

    // The cases in column order, so a harness's k-th answer is the answer to Cases[k].
    public IReadOnlyList<LeetCodeCase> Cases { get; } = cases;

    public int Count => Cases.Count;

    public static CatalogColumns For(Type solution, params Type[] parameterTypes)
    {
        var question = QuestionOf(solution);
        var kept = LeetCodeCatalog.Cases(question.TitleSlug)
            .Select(testCase => (Case: testCase, Arguments: ArgumentsOf(testCase, parameterTypes)))
            .Where(row => IsKept(question, parameterTypes, row.Arguments))
            .ToList();
        var byParameter = parameterTypes
            .Select((_, parameter) => kept.Select(row => row.Arguments[parameter]).ToArray())
            .ToArray();

        return new CatalogColumns([.. kept.Select(row => row.Case)], byParameter);
    }

    private static object?[] ArgumentsOf(LeetCodeCase testCase, Type[] parameterTypes)
    {
        if (testCase.Inputs.Count != parameterTypes.Length)
        {
            throw new InvalidOperationException(
                $"A case lists {testCase.Inputs.Count} inputs, and the strategy takes {parameterTypes.Length}.");
        }

        return [.. testCase.Inputs.Zip(parameterTypes, NotationReader.Read)];
    }

    private static bool IsKept(LeetCodeQuestionAttribute question, Type[] parameterTypes, object?[] arguments)
    {
        if (question.Promise is not { } promise)
        {
            return true;
        }

        var isKeptBy = promise.GetMethod(PromiseMethod, BindingFlags.Public | BindingFlags.Static, parameterTypes)
            ?? throw new InvalidOperationException($"{promise.Name} has no {PromiseMethod} taking the strategy's inputs.");

        return isKeptBy.Invoke(null, BindingFlags.DoNotWrapExceptions, binder: null, arguments, culture: null) is true;
    }

    public static LeetCodeQuestionAttribute QuestionOf(Type solution) =>
        solution.GetCustomAttribute<LeetCodeQuestionAttribute>()
            ?? throw new InvalidOperationException($"{solution.Name} names no LeetCode question, so the catalog holds no cases for it.");

    // Every case's value for one parameter, as the type the strategy takes it as.
    public TValue[] Column<TValue>(int parameter) => [.. columns[parameter].Cast<TValue>()];

    // One case's value for one parameter, for a caller that holds the parameter's type only as a Type.
    public object? ValueAt(int parameter, int caseIndex) => columns[parameter][caseIndex];
}
