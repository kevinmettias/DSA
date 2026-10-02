using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;

namespace DSAExperimentation.Benchmarks;

// The one place a benchmark run is configured. Benchmark classes declare only arms and workloads
// (ARCHITECTURE 17.7); how those arms are measured is decided here, so that a number in a baseline
// file can be traced back to the job that produced it. Program.cs passes this to the switcher.
//
// The recording job is pinned rather than left as BenchmarkDotNet's default, and its counts are
// written out rather than named, because "Default" is BenchmarkDotNet's answer to give and it has
// changed between versions. A baseline is only worth committing if the thing that produced it is
// nailed down.
//
// It is dropped, though, when the command line asks for a job of its own. BenchmarkDotNet treats a
// command-line --job as one MORE job rather than a replacement, so a pinned job here would make
// `--job dry` run the dry job and the full one back to back - turning the smoke run used to check
// a harness into the most expensive thing you can do. Replacing the recording job is exactly what
// someone typing --job is asking for, so that is what happens.
//
// JsonExporter is the one export the defaults leave out, and it is the only machine-comparable one:
// the Markdown, HTML and CSV exports are for reading, and comparing two runs by eye is the practice
// this replaces. It lands under BenchmarkDotNet.Artifacts/ beside the rest.
//
// MemoryDiagnoser is declared here as well as on individual classes. Duplicating it is harmless -
// BenchmarkDotNet collapses repeat diagnosers - and it means a new benchmark class gets allocation
// numbers without having to remember the attribute.
internal static class BenchmarkConfig
{
    private const int WarmupCount = 6;
    private const int IterationCount = 15;
    private const int LaunchCount = 1;
    private const string ShortJobOption = "-j";
    private const string LongJobOption = "--job";

    public static ManualConfig For(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(JsonExporter.Full);

        if (!AsksForItsOwnJob(args))
        {
            config.AddJob(Job.Default
                .WithWarmupCount(WarmupCount)
                .WithIterationCount(IterationCount)
                .WithLaunchCount(LaunchCount));
        }

        return config;
    }

    // Both spellings, and --job=dry as well as --job dry, because BenchmarkDotNet accepts all
    // three and a missed spelling would silently cost the caller a second, full-length job.
    private static bool AsksForItsOwnJob(string[] args) =>
        args.Any(argument =>
            argument.StartsWith(LongJobOption, StringComparison.Ordinal)
            || argument.Equals(ShortJobOption, StringComparison.Ordinal));
}
