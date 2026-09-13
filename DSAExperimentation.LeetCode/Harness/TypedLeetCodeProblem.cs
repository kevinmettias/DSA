using System.Text.Json;

namespace DSAExperimentation.LeetCode.Harness;

// The typed half of LeetCodeProblem: everything here still knows its real
// TInput/TOutput, so running a case is a direct, checked call - the erasure to
// object happens only on the way back out, in the rendered LeetCodeRunOutcome.
//
// AnswersMatch is required rather than defaulted for the reason
// ILeetCodeTestCaseAdapter's own doc comment already gives: EqualityComparer<T>
// .Default compares arrays and lists by REFERENCE, so a shared default would pass
// only when actual and expected happened to be the same instance. Every problem
// states its own equality - sequence equality for an ordered collection, an
// order-independent comparison for a return-in-any-order problem, a tolerance for
// a floating-point answer.
internal sealed class TypedLeetCodeProblem<TInput, TOutput> : LeetCodeProblem
{
    private static readonly JsonSerializerOptions RenderOptions = new() { WriteIndented = false };

    private readonly Dictionary<string, Func<TInput, TOutput>> _strategies;
    private readonly Dictionary<string, LeetCodeCase<TInput, TOutput>> _cases;
    private readonly Dictionary<string, TInput> _workloads;
    private readonly Func<TOutput, TOutput, bool> _answersMatch;

    public override string TitleSlug { get; }

    public override IReadOnlyList<string> StrategyNames { get; }

    public override IReadOnlyList<string> CaseNames { get; }

    public override IReadOnlyList<LeetCodeArm> WorkloadArms { get; }

    public TypedLeetCodeProblem(
        string titleSlug,
        IReadOnlyList<(string Name, Func<TInput, TOutput> Run)> strategies,
        IReadOnlyList<LeetCodeCase<TInput, TOutput>> cases,
        IReadOnlyList<(string Name, TInput Input, IReadOnlyList<string> StrategyNames)> workloads,
        Func<TOutput, TOutput, bool> answersMatch)
    {
        TitleSlug = titleSlug;
        _answersMatch = answersMatch;
        _strategies = strategies.ToDictionary(strategy => strategy.Name, strategy => strategy.Run);
        _cases = cases.ToDictionary(example => example.Name);
        _workloads = workloads.ToDictionary(workload => workload.Name, workload => workload.Input);
        StrategyNames = strategies.Select(strategy => strategy.Name).ToList();
        CaseNames = cases.Select(example => example.Name).ToList();
        WorkloadArms =
        [
            .. workloads.SelectMany(
                workload => MeasuredStrategies(workload.StrategyNames).Select(
                    strategy => new LeetCodeArm(titleSlug, strategy, workload.Name))),
        ];

        IEnumerable<string> MeasuredStrategies(IReadOnlyList<string> requested)
            => requested.Count == 0 ? StrategyNames : requested;
    }

    public override LeetCodeRunOutcome RunCase(string strategyName, string caseName)
    {
        var strategy = Resolve(_strategies, strategyName, nameof(strategyName));
        var example = Resolve(_cases, caseName, nameof(caseName));

        var actual = strategy(example.Input);

        return new LeetCodeRunOutcome
        {
            Matched = _answersMatch(actual, example.Expected),
            Expected = Render(example.Expected),
            Actual = Render(actual),
        };
    }

    public override Func<object?> BindWorkload(string strategyName, string workloadName)
    {
        var strategy = Resolve(_strategies, strategyName, nameof(strategyName));
        var input = Resolve(_workloads, workloadName, nameof(workloadName));

        return () => strategy(input);
    }

    // JSON rather than ToString(): every answer shape these problems produce is
    // either a primitive or a collection of them, and ToString() on a collection
    // prints its type name, which is useless in a failure message.
    private static string Render(TOutput answer)
    {
        try
        {
            return JsonSerializer.Serialize(answer, RenderOptions);
        }
        catch (NotSupportedException)
        {
            // A registration is free to use an input/answer type JSON cannot walk
            // (a graph node with a cycle, say). A failure message is not worth
            // failing the run over, so fall back to whatever the type prints.
            return answer?.ToString() ?? "null";
        }
    }

    private TValue Resolve<TValue>(Dictionary<string, TValue> entries, string name, string parameterName)
        => entries.TryGetValue(name, out var value)
            ? value
            : throw new ArgumentOutOfRangeException(
                parameterName, name, $"'{TitleSlug}' has no entry named '{name}'.");
}
