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

    public TypedLeetCodeProblem(string titleSlug, LeetCodeRegistration<TInput, TOutput> registration)
    {
        var strategies = registration.Strategies;
        var cases = registration.Cases;
        var workloads = registration.Workloads;
        var answersMatch = registration.AnswersMatch;

        TitleSlug = titleSlug;
        _answersMatch = answersMatch;
        _strategies = strategies.ToDictionary(strategy => strategy.Name, strategy => strategy.Run);
        _cases = cases.ToDictionary(example => example.Name);
        _workloads = workloads.ToDictionary(workload => workload.Name, workload => workload.Input);
        StrategyNames = strategies.Select(strategy => strategy.Name).ToList();
        CaseNames = cases.Select(example => example.Name).ToList();
        WorkloadArms = BuildWorkloadArms(titleSlug, workloads, StrategyNames);
    }

    // The arms this problem is measured over: its workloads crossed with the strategies each
    // one names. A workload that names none means "all of them", which is what StrategiesFor
    // resolves. Lifted out of the constructor, which is left to wire the problem up.
    private static IReadOnlyList<LeetCodeArm> BuildWorkloadArms(
        string titleSlug,
        IReadOnlyList<(string Name, TInput Input, IReadOnlyList<string> StrategyNames)> workloads,
        IReadOnlyList<string> strategyNames)
        =>
        [
            .. workloads.SelectMany(
                workload => StrategiesFor(workload.StrategyNames, strategyNames).Select(
                    strategy => new LeetCodeArm(titleSlug, strategy, workload.Name))),
        ];

    private static IReadOnlyList<string> StrategiesFor(
        IReadOnlyList<string> requested, IReadOnlyList<string> fallback)
        => requested.Count == 0 ? fallback : requested;

    public override LeetCodeRunOutcome RunCase(StrategyName strategyName, CaseName caseName)
    {
        var strategy = Resolve(
            _strategies, new EntryName(strategyName.Text), new ParameterLabel(nameof(strategyName)));
        var example = Resolve(_cases, new EntryName(caseName.Text), new ParameterLabel(nameof(caseName)));

        var actual = strategy(example.Input);

        return new LeetCodeRunOutcome
        {
            Matched = _answersMatch(actual, example.Expected),
            Expected = Render(example.Expected),
            Actual = Render(actual),
        };
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

    public override IBoundWorkload BindWorkload(StrategyName strategyName, WorkloadName workloadName)
    {
        var strategy = Resolve(
            _strategies, new EntryName(strategyName.Text), new ParameterLabel(nameof(strategyName)));
        var input = Resolve(
            _workloads, new EntryName(workloadName.Text), new ParameterLabel(nameof(workloadName)));

        return new BoundWorkload(strategy, input);
    }

    // The measured region as a collaborator rather than a closure: the two
    // lookups above already happened, so Run() is one delegate invocation over an
    // input built once, and a null answer means the strategy produced null rather
    // than "nothing was bound".
    private sealed class BoundWorkload(Func<TInput, TOutput> strategy, TInput input) : IBoundWorkload
    {
        public object? Run() => strategy(input);
    }

    private TValue Resolve<TValue>(Dictionary<string, TValue> entries, EntryName name, ParameterLabel parameterName)
    {
        if (entries.TryGetValue(name.Text, out var value))
        {
            return value;
        }

        throw new ArgumentOutOfRangeException(
            parameterName.Text, name.Text, $"'{TitleSlug}' has no entry named '{name.Text}'.");
    }

    // The two roles a lookup here has, which the pair of `string`s it used to take did
    // not name: the entry to look up among the registered strategies, cases or
    // workloads, and the caller's own parameter name, which is what the thrown
    // exception blames. They are not interchangeable - transposed, the lookup fails
    // naming an entry nobody asked for and the message points at the wrong parameter -
    // so each position gets the type that says which one it is.
    private readonly record struct EntryName(string Text);

    private readonly record struct ParameterLabel(string Text);
}
