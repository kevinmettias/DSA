using DSAExperimentation.Benchmarks.Inlining;

namespace DSAExperimentation.Benchmarks.Tests.Inlining;

// The listener's bookkeeping, fed the runtime's events by name and fields as the JIT reports them. A
// test run is a Debug build, whose code the JIT compiles without optimization and so without inlines
// to report, so the events are written here rather than provoked.
public sealed partial class InliningListenerTests
{
    // The runtime's event names, as the JIT writes them.
    private const string CompileStarted = "MethodJittingStarted_V1";
    private const string InlineFailed = "MethodJitInliningFailed";
    private const string InlineSucceeded = "MethodJitInliningSucceeded";

    private const string Watched = "WatchedArm";
    private const string Other = "AnotherMethod";
    private const string Refused = "Refused";
    private const string Small = "Small";
    private const string Big = "Big";
    private const string TooBig = "unprofitable inline";
    private const string Forbidden = "noinline per IL/cached result";
    private const string InlineeNamespace = "Lib";

    [Fact]
    public void Record_EachCompileOfTheWatchedMethod_StartsABatch()
    {
        using var listener = new InliningListener(Watched);

        listener.Record(CompileStarted, Compile(Watched));
        listener.Record(CompileStarted, Compile(Other));
        listener.Record(CompileStarted, Compile(Watched));

        Assert.Equal([0, 0], listener.Compiles.Select(compile => compile.Count));
    }

    [Fact]
    public void Record_TheLatestBatch_HoldsTheInlinesTriedSinceItStarted()
    {
        var last = AfterTwoCompiles().Compiles[^1];

        Assert.Equal([$"{InlineeNamespace}.{Small}", $"{InlineeNamespace}.{Big}"], last.Select(decision => decision.Inlinee));
    }

    [Fact]
    public void Record_AnInline_KeepsWhetherItWasMadeAndWhyNot()
    {
        var last = AfterTwoCompiles().Compiles[^1];

        Assert.Equal([(true, null), (false, TooBig)], last.Select(decision => (decision.IsInlined, decision.FailReason)));
    }

    [Fact]
    public void Record_AnInlineWhileCompilingAnotherMethod_IsLeftOut()
    {
        using var listener = new InliningListener(Watched);

        listener.Record(CompileStarted, Compile(Watched));
        listener.Record(InlineFailed, Inline(Other, Refused, TooBig));

        Assert.Empty(listener.Compiles[0]);
    }

    // An inline reported before any compile of the method was seen has no batch to join.
    [Fact]
    public void Compiles_OfAMethodNeverCompiled_AreNone()
    {
        using var listener = new InliningListener(Watched);

        listener.Record(InlineFailed, Inline(Watched, Refused, TooBig));

        Assert.Empty(listener.Compiles);
    }

    // Two compiles of the watched method: the first refuses Refused, the second inlines Small and
    // refuses Big.
    private static InliningListener AfterTwoCompiles()
    {
        (string Name, Dictionary<string, string> Payload)[] events =
        [
            (CompileStarted, Compile(Watched)),
            (InlineFailed, Inline(Watched, Refused, Forbidden)),
            (CompileStarted, Compile(Watched)),
            (InlineSucceeded, Inline(Watched, Small, reason: null)),
            (InlineFailed, Inline(Watched, Big, TooBig)),
        ];
        var listener = new InliningListener(Watched);

        foreach (var (name, payload) in events)
        {
            listener.Record(name, payload);
        }

        return listener;
    }

    private static Dictionary<string, string> Compile(string method) => new() { [InliningListener.MethodNameField] = method };

    private static Dictionary<string, string> Inline(string compiled, string inlinee, string? reason)
    {
        var payload = new Dictionary<string, string>
        {
            [InliningListener.CompiledMethodField] = compiled,
            [InliningListener.InlinerNamespaceField] = string.Empty,
            [InliningListener.InlinerNameField] = compiled,
            [InliningListener.InlineeNamespaceField] = InlineeNamespace,
            [InliningListener.InlineeNameField] = inlinee,
        };

        if (reason is not null)
        {
            payload[InliningListener.FailReasonField] = reason;
        }

        return payload;
    }
}
