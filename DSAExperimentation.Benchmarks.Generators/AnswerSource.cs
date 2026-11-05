namespace DSAExperimentation.Benchmarks.Generators;

// Where a generated arm takes each case's answer from: what the strategy returns, or, for a question its
// attribute says is answered in place (AnswersInPlace), the strategy's first input as the call leaves it.
internal enum AnswerSource
{
    ReturnValue,
    FirstInput,
}
