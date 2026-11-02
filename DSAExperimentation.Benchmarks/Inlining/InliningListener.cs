using System.Diagnostics.Tracing;

namespace DSAExperimentation.Benchmarks.Inlining;

// The JIT's own inlining decisions, read from the runtime's event source in this process. Each time
// the JIT compiles a method it reports every inline it tried, and why it declined one, which is the
// only first-hand account of what a benchmark arm's machine code still calls. A method compiles more
// than once as it tiers up, so the listener keeps one batch of decisions per compile of the method
// it watches; the last batch is the optimized compile that runs last.
internal sealed class InliningListener(string methodName) : EventListener
{
    private const string RuntimeSource = "Microsoft-Windows-DotNETRuntime";

    // The runtime's Jit keyword, which carries each compile's start, and JitTracing, which carries
    // each inline's verdict.
    private const EventKeywords JitAndJitTracing = (EventKeywords)0x1010;

    // The runtime's names for the events and fields the listener reads.
    internal const string CompileStartedEvent = "MethodJittingStarted";
    internal const string InliningEvent = "MethodJitInlining";
    internal const string MethodNameField = "MethodName";
    internal const string CompiledMethodField = "MethodBeingCompiledName";
    internal const string InlinerNamespaceField = "InlinerNamespace";
    internal const string InlinerNameField = "InlinerName";
    internal const string InlineeNamespaceField = "InlineeNamespace";
    internal const string InlineeNameField = "InlineeName";
    internal const string FailReasonField = "FailReason";

    // An inlining event's name ends MethodJitInliningFailed or MethodJitInliningSucceeded.
    private const string FailedSuffix = "Failed";

    private readonly List<List<InlineDecision>> _compiles = [];

    /// <summary>Each compile of the watched method seen so far, its inlining decisions in the order the JIT made them.</summary>
    public IReadOnlyList<IReadOnlyList<InlineDecision>> Compiles
    {
        get
        {
            lock (_compiles)
            {
                return [.. _compiles.Select(compile => (IReadOnlyList<InlineDecision>)[.. compile])];
            }
        }
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == RuntimeSource)
        {
            EnableEvents(eventSource, EventLevel.Verbose, JitAndJitTracing);
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName is not { } name)
        {
            return;
        }

        if (eventData.PayloadNames is not { } fields || eventData.Payload is not { } values)
        {
            return;
        }

        var payload = fields.Zip(values)
            .ToDictionary(field => field.First, field => field.Second?.ToString() ?? string.Empty);

        Record(name, payload);
    }

    // One runtime event, by its name and fields: a compile of the watched method starts a batch, and
    // each inline the JIT tried while compiling it joins the latest batch.
    internal void Record(string eventName, IReadOnlyDictionary<string, string> payload)
    {
        if (eventName.StartsWith(CompileStartedEvent, StringComparison.Ordinal))
        {
            CompileStarted(payload);
        }
        else if (eventName.StartsWith(InliningEvent, StringComparison.Ordinal))
        {
            Decided(eventName, payload);
        }
    }

    private void CompileStarted(IReadOnlyDictionary<string, string> payload)
    {
        if (payload.GetValueOrDefault(MethodNameField) == methodName)
        {
            lock (_compiles)
            {
                _compiles.Add([]);
            }
        }
    }

    private void Decided(string eventName, IReadOnlyDictionary<string, string> payload)
    {
        if (payload.GetValueOrDefault(CompiledMethodField) != methodName)
        {
            return;
        }

        lock (_compiles)
        {
            if (_compiles.Count > 0)
            {
                var decision = DecisionIn(eventName, payload);
                _compiles[^1].Add(decision);
            }
        }
    }

    private static InlineDecision DecisionIn(string eventName, IReadOnlyDictionary<string, string> payload)
        => new(
            Inliner: $"{payload.GetValueOrDefault(InlinerNamespaceField)}.{payload.GetValueOrDefault(InlinerNameField)}",
            Inlinee: $"{payload.GetValueOrDefault(InlineeNamespaceField)}.{payload.GetValueOrDefault(InlineeNameField)}",
            FailReason: payload.GetValueOrDefault(FailReasonField),
            IsInlined: !eventName.Contains(FailedSuffix, StringComparison.Ordinal));

    /// <summary>One inline the JIT tried while compiling the watched method.</summary>
    /// <param name="Inliner">The method the call sits in, namespace and type first.</param>
    /// <param name="Inlinee">The method called.</param>
    /// <param name="FailReason">Why the JIT declined, when it did.</param>
    /// <param name="IsInlined">Whether the call was inlined.</param>
    public readonly record struct InlineDecision(string Inliner, string Inlinee, string? FailReason, bool IsInlined);
}
