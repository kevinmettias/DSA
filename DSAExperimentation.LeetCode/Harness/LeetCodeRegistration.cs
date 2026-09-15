namespace DSAExperimentation.LeetCode.Harness;

// Everything a problem's registration states about itself except the slug that names
// it: the strategies, the cases they are run on, the workloads that separate them, and
// how two answers compare. LeetCodeProblemBuilder produces exactly these four and
// TypedLeetCodeProblem consumes exactly these four, so at that one call site nothing
// but position told three IReadOnlyLists and a function apart - they are one argument
// that says what they are.
internal readonly record struct LeetCodeRegistration<TInput, TOutput>(
    IReadOnlyList<(string Name, Func<TInput, TOutput> Run)> Strategies,
    IReadOnlyList<LeetCodeCase<TInput, TOutput>> Cases,
    IReadOnlyList<(string Name, TInput Input, IReadOnlyList<string> StrategyNames)> Workloads,
    Func<TOutput, TOutput, bool> AnswersMatch);
