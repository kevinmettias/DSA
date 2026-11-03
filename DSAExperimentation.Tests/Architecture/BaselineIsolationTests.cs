using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.5: a benchmark's baseline is the problem's simple form, written
// by hand, so nothing it runs binds to a library implementation - not a container, not an engine, not
// a mechanism. It may read what the library states: Domain's facts, a contract, a constant or an enum.
// The baseline is the arm a ProblemSolutions benchmark marks Baseline = true. BaselineTrace follows it
// through the compiled code, from the solution members the arm's harness reaches, and
// BaselineTraceTests states what it follows and what it excuses: what a baseline may read, the input
// the harness hands it, and LeetCode's own I/O shapes.
public sealed partial class BaselineIsolationTests
{
    // Fewer baselines than this means the sweep stopped finding them, not that they are all clean.
    private const int ExpectedBaselinesAtLeast = 1000;

    // Keyed by benchmark class: a baseline the trace finds reaching a library implementation, and why
    // that is not a baseline written with the library.
    private static readonly Dictionary<string, string> AllowedReaches = new(StringComparer.Ordinal)
    {
    };

    // Every baseline arm's trace, made once for the tests below.
    private static readonly Lazy<List<TracedArm>> Sweep = new(TraceEveryArm);

    [Fact]
    public void EveryBaseline_UsesNoLibraryImplementation()
    {
        var offences = Offences()
            .Where(offence => !AllowedReaches.ContainsKey(offence.BenchmarkClass))
            .Select(offence => offence.Describe())
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    // An entry whose reach has gone would otherwise sit in the list excusing nothing.
    [Fact]
    public void AllowedReaches_AreAllStillReached()
    {
        var reached = Offences().Select(offence => offence.BenchmarkClass).ToHashSet();

        Assert.DoesNotContain(AllowedReaches.Keys, benchmark => !reached.Contains(benchmark));
    }

    // A sweep that found no baseline would report no offence while checking nothing.
    [Fact]
    public void BaselineSweep_FindsTheBenchmarkBaselines()
        => Assert.True(Sweep.Value.Count >= ExpectedBaselinesAtLeast, $"{Sweep.Value.Count} baseline arms found.");

    // An arm whose trace reached no solution member checked nothing either.
    [Fact]
    public void EveryBaseline_ReachesTheSolutionMemberItCalls()
        => Assert.Empty(Sweep.Value.Where(armTrace => armTrace.Trace.Entries.Count == 0).Select(armTrace => armTrace.BenchmarkClass));

    private static IEnumerable<Offence> Offences()
        => Sweep.Value.Where(armTrace => armTrace.Trace.Reaches.Count > 0).Select(armTrace => new Offence(armTrace));

    private static List<TracedArm> TraceEveryArm()
    {
        var models = new SourceModels([RepositoryCompilation.Benchmarks, RepositoryCompilation.Solutions, RepositoryCompilation.Framework]);

        return [.. BaselineTrace.ArmsIn(RepositoryCompilation.Benchmarks).Select(arm => new TracedArm(arm, BaselineTrace.From(arm, models)))];
    }

    // One baseline arm and what its trace found.
    internal sealed record TracedArm(IMethodSymbol Arm, BaselineTrace.ArmTrace Trace)
    {
        public string BenchmarkClass => Arm.ContainingType.Name;
    }

    private readonly record struct Offence(TracedArm Traced)
    {
        // What every offence says about how to clear it.
        private const string Remedy =
            "A baseline is the problem's simple form, so it writes its own version of a library implementation, "
            + "implementing the layer's contract where there is one (ARCHITECTURE.md 17.5); "
            + "a reach through code the baseline never runs goes in AllowedReaches with why.";

        public string BenchmarkClass => Traced.BenchmarkClass;

        public string Describe()
        {
            var reaches = Traced.Trace.Reaches.Select(reach => $"{reach.Name}, through {string.Join(" > ", reach.Chain)}");

            return $"{BenchmarkClass}.{Traced.Arm.Name}: the baseline reaches {string.Join("; ", reaches)}. {Remedy}";
        }
    }
}
