namespace DSAExperimentation.Tests.LeetCodeCatalog;

// Runs an injected solution against one question's cached example test cases and
// reports pass/fail per case - deliberately returns a report rather than
// asserting itself, so it stays usable from a plain method call (a future sync
// tool, a REPL) and not just from inside an xUnit [Fact].
internal static class LeetCodeSolutionValidator
{
    private const string MissingExpectedOutputReason = "no expected output was available to compare against";

    public static IReadOnlyList<LeetCodeTestCaseResult> Validate<TInput, TOutput>(
        LeetCodeQuestion question, Func<TInput, TOutput> solution, ILeetCodeTestCaseAdapter<TInput, TOutput> adapter)
    {
        var results = new List<LeetCodeTestCaseResult>(question.ExampleTestCases.Count);

        foreach (var testCase in question.ExampleTestCases)
        {
            var result = ValidateOne(testCase, solution, adapter);
            results.Add(result);
        }

        return results;
    }

    private static LeetCodeTestCaseResult ValidateOne<TInput, TOutput>(
        LeetCodeTestCase testCase, Func<TInput, TOutput> solution, ILeetCodeTestCaseAdapter<TInput, TOutput> adapter)
    {
        if (testCase.RawExpectedOutput is null)
        {
            return new LeetCodeTestCaseResult(testCase.RawInput, Passed: false, MissingExpectedOutputReason);
        }

        var actual = solution(adapter.ParseInput(testCase.RawInput));
        var expected = adapter.ParseExpectedOutput(testCase.RawExpectedOutput);

        return adapter.IsMatch(actual, expected)
            ? new LeetCodeTestCaseResult(testCase.RawInput, Passed: true, FailureReason: null)
            : new LeetCodeTestCaseResult(
                testCase.RawInput, Passed: false, $"expected `{testCase.RawExpectedOutput}`, got `{actual}`");
    }
}
