namespace DSAExperimentation.LeetCode.Harness;

// The registration API, and the whole reason the erasure in LeetCodeProblem costs
// nothing at the point of use: TInput and TOutput are fixed by the opening
// LeetCodeProblem.For<...> call, so every Strategy and every Case below it is
// checked by the compiler against those types. A case whose input does not fit
// the strategy, or whose expected value is the wrong shape, does not build.
//
// Build() refuses an incomplete registration rather than producing a problem that
// silently tests nothing - a strategy list with no cases would otherwise
// contribute zero theory rows and look exactly like a passing problem.
internal sealed class LeetCodeProblemBuilder<TInput, TOutput>(string titleSlug)
{
    private readonly List<(string Name, Func<TInput, TOutput> Run)> _strategies = [];
    private readonly List<LeetCodeCase<TInput, TOutput>> _cases = [];
    private readonly List<(string Name, TInput Input, IReadOnlyList<string> StrategyNames)> _workloads = [];

    private Func<TOutput, TOutput, bool>? _answersMatch;

    public LeetCodeProblemBuilder<TInput, TOutput> Strategy(string name, Func<TInput, TOutput> run)
    {
        _strategies.Add((name, run));
        return this;
    }

    public LeetCodeProblemBuilder<TInput, TOutput> Case(string name, TInput input, TOutput expected)
    {
        _cases.Add(new LeetCodeCase<TInput, TOutput>(name, input, expected));
        return this;
    }

    // A benchmark input, kept separate from Case's because the two answer
    // different questions: a case is small and has a known answer, a workload is
    // large enough to separate the strategies and has no asserted answer at all.
    //
    // Naming strategies restricts the workload to those arms; naming none measures
    // every arm. The restriction exists because a naive baseline is often
    // asymptotically worse on purpose - measuring it at the size that separates
    // the real implementations would not finish - and the alternative, dropping
    // the workload entirely, would leave the good arm unmeasured too.
    public LeetCodeProblemBuilder<TInput, TOutput> Workload(
        string name, TInput input, params string[] strategyNames)
    {
        _workloads.Add((name, input, strategyNames));
        return this;
    }

    public LeetCodeProblemBuilder<TInput, TOutput> MatchingAnswersWith(Func<TOutput, TOutput, bool> answersMatch)
    {
        _answersMatch = answersMatch;
        return this;
    }

    public LeetCodeProblem Build()
    {
        if (_strategies.Count == 0)
        {
            throw new InvalidOperationException($"'{titleSlug}' registered no strategies.");
        }

        if (_cases.Count == 0)
        {
            throw new InvalidOperationException($"'{titleSlug}' registered no cases.");
        }

        var answersMatch = _answersMatch
            ?? throw new InvalidOperationException(
                $"'{titleSlug}' did not state how answers compare - call MatchingAnswersWith.");

        return new TypedLeetCodeProblem<TInput, TOutput>(titleSlug, _strategies, _cases, _workloads, answersMatch);
    }
}
