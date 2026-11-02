using System.Diagnostics;
using System.Reflection;

namespace DSAExperimentation.Benchmarks.Inlining;

// `inlining <Class> <Arm> [seconds] [method]`: which calls the JIT declined to inline, and why, in
// one benchmark arm or in a method it runs, such as the solution method it calls. ARCHITECTURE 19.8
// records what a one-shot helper's inlined IL cost a hot loop. That cost was found by reading the
// JIT's own decisions in this process, and this verb makes that a command rather than a script
// written each time.
//
// It builds the class as BenchmarkDotNet would: each [Params] property set to its first value, then
// each [GlobalSetup] run, and an arm with [ArgumentsSource] handed that source's first value. It
// calls the arm for the given seconds, long enough for it to tier up to an optimized compile, and
// prints that compile's failed inlines. Run it from a Release build: a Debug build's code is
// compiled without optimization, so the JIT tries no inlines to report. A watched method other than
// the arm is named as the JIT names it: its bare method name.
internal static class InliningCommand
{
    public const string Name = "inlining";

    private const int ExitFound = 0;
    private const int ExitUsage = 2;
    private const int ExitNoCompile = 3;
    private const double DefaultSeconds = 3;

    // The verb, the class and the arm; then the seconds, then the watched method.
    private const int MinimumArguments = 3;
    private const int SecondsArgument = 3;
    private const int MethodArgument = 4;

    // Events reach the listener on the runtime's own thread, a little after the compile they report.
    private static readonly TimeSpan EventDrain = TimeSpan.FromMilliseconds(500);

    public static bool IsAskedFor(string[] args) =>
        args.Length > 0 && string.Equals(args[0], Name, StringComparison.Ordinal);

    public static async Task<int> RunAsync(string[] args, TextWriter output, TextWriter error)
    {
        if (Resolve(args) is not { } resolved)
        {
            error.WriteLine("usage: inlining <BenchmarkClass> <Arm> [seconds] [method], naming a class and one of its public arms");
            return ExitUsage;
        }

        var (type, arm) = resolved;
        var watched = WatchedMethod(args, arm);
        using var listener = new InliningListener(watched);
        Drive(type, arm, TimeSpan.FromSeconds(Seconds(args)));
        await Task.Delay(EventDrain);

        return Report(listener, new Watch($"{type.Name}.{arm.Name}", watched), output, error);
    }

    // The benchmark class and arm the arguments name, when they name both.
    private static (Type Type, MethodInfo Arm)? Resolve(string[] args)
    {
        if (args.Length < MinimumArguments)
        {
            return null;
        }

        var type = BenchmarkType(args[1]);
        var arm = type?.GetMethod(args[2]);

        if (type is null || arm is null)
        {
            return null;
        }

        return (type, arm);
    }

    // A benchmark class by its name, with or without the Benchmarks suffix.
    private static Type? BenchmarkType(string name)
        => typeof(InliningCommand).Assembly.GetTypes()
            .FirstOrDefault(type => type.Name == name || type.Name == $"{name}Benchmarks");

    // The method whose compiles are read: the one named after the seconds, or the arm itself.
    private static string WatchedMethod(string[] args, MethodInfo arm)
    {
        if (args.Length > MethodArgument)
        {
            return args[MethodArgument];
        }

        return arm.Name;
    }

    private static void Drive(Type type, MethodInfo arm, TimeSpan duration)
    {
        var instance = Activator.CreateInstance(type);
        var arguments = ArgumentsFor(type, arm, instance);
        var clock = Stopwatch.StartNew();

        while (clock.Elapsed < duration)
        {
            arm.Invoke(instance, arguments);
        }
    }

    // Sets each [Params] property to its first value and runs each [GlobalSetup], as BenchmarkDotNet
    // does before a run, then reads the arm's first [ArgumentsSource] value, if it takes one.
    private static object?[]? ArgumentsFor(Type type, MethodInfo arm, object? instance)
    {
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<ParamsAttribute>() is { Values.Length: > 0 } values)
            {
                property.SetValue(instance, values.Values[0]);
            }
        }

        foreach (var setup in type.GetMethods().Where(method => method.GetCustomAttribute<GlobalSetupAttribute>() is not null))
        {
            setup.Invoke(instance, null);
        }

        if (arm.GetCustomAttribute<ArgumentsSourceAttribute>() is not { } source)
        {
            return null;
        }

        var first = SourceValues(type, source.Name, instance).Cast<object?>().First();

        return first as object?[] ?? [first];
    }

    private static System.Collections.IEnumerable SourceValues(Type type, string member, object? instance)
    {
        var value = type.GetProperty(member)?.GetValue(instance) ?? type.GetMethod(member)?.Invoke(instance, null);

        return value as System.Collections.IEnumerable
            ?? throw new InvalidOperationException($"{type.Name}.{member} is not an [ArgumentsSource] this command can read.");
    }

    private static double Seconds(string[] args)
    {
        if (args.Length > SecondsArgument && double.TryParse(args[SecondsArgument], out var given))
        {
            return given;
        }

        return DefaultSeconds;
    }

    private static int Report(InliningListener listener, Watch watch, TextWriter output, TextWriter error)
    {
        var compiles = listener.Compiles;

        if (compiles.Count == 0)
        {
            error.WriteLine($"{watch.Method} was never compiled while {watch.Arm} ran; nothing to report.");
            return ExitNoCompile;
        }

        var last = compiles[^1];
        output.WriteLine($"{watch.Method}, driven by {watch.Arm}: compile {compiles.Count}, {last.Count} inline(s) tried");

        foreach (var decision in last.Where(decision => !decision.IsInlined))
        {
            output.WriteLine($"FAIL {decision.Inliner} -> {decision.Inlinee}: {decision.FailReason}");
        }

        return ExitFound;
    }

    // The arm that runs, and the method whose compiles are read: the arm itself, or one it calls.
    private readonly record struct Watch(string Arm, string Method);
}
