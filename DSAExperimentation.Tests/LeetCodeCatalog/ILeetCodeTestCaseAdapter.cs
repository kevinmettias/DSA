namespace DSAExperimentation.Tests.LeetCodeCatalog;

// The "common contract to inject a solution" this catalog needs: LeetCode's raw
// test case text is untyped and per-problem-shaped (one param per line, in the
// question's own metaData param order - see LeetCodeQuestionMapper), so a generic
// runner (LeetCodeSolutionValidator) can drive ANY problem's solution only if each
// problem supplies its own translation from raw text to real TInput/TOutput
// values. IsMatch has no default implementation deliberately: EqualityComparer<T>
// .Default compares arrays/lists by REFERENCE, not content, so a naive shared
// default would silently pass only when actual and expected happen to be the same
// instance - every adapter must state its own real equality (sequence equality
// for a collection result, an order-independent set comparison for a
// return-in-any-order problem like Two Sum, etc.).
internal interface ILeetCodeTestCaseAdapter<TInput, TOutput>
{
    TInput ParseInput(string rawInput);

    TOutput ParseExpectedOutput(string rawExpectedOutput);

    bool IsMatch(TOutput actual, TOutput expected);
}
