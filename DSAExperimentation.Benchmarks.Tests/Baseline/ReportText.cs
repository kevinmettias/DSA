using System.Text.Json;

namespace DSAExperimentation.Benchmarks.Tests.Baseline;

// A BenchmarkDotNet report as text, built rather than run: the baseline reads and writes
// text, so its tests need reports and not benchmarks, and a test that ran one would be
// measuring the machine it happens to be on.
//
// The shape below is the shape a real -report-full.json has, including the parts nothing
// reads - DisplayInfo's job suffix with its generated hash, and the parameter list after
// it. A fixture that only held the fields the reader wants would keep passing after the
// real exporter moved one, which is the one failure this fixture exists to catch.
internal static class ReportText
{
    public const string PinnedJob = "(IterationCount=15, LaunchCount=1, WarmupCount=6)";
    public const string FirstHash = "Job-ABCDEF";
    public const string SecondHash = "Job-ZYXWVU";

    private const string ParameterSuffix = " [Size=10000]";

    public static string Of(params (string Benchmark, double Mean, long? Bytes)[] readings) =>
        WithJob(PinnedJob, readings);

    public static string WithJob(
        string job,
        params (string Benchmark, double Mean, long? Bytes)[] readings) =>
        WithDisplayJob($"{FirstHash}{job}", readings);

    // The same job under a different generated hash, which is what two runs of an unchanged
    // benchmark produce. Anything comparing runs has to see these as one job.
    public static string WithRehashedJob(
        string job,
        params (string Benchmark, double Mean, long? Bytes)[] readings) =>
        WithDisplayJob($"{SecondHash}{job}", readings);

    // The job exactly as BenchmarkDotNet prints it inside DisplayInfo, for the cases where a
    // test needs to control the suffix itself rather than have one composed for it.
    public static string WithDisplayJob(
        string displayJob,
        params (string Benchmark, double Mean, long? Bytes)[] readings)
    {
        var benchmarks = readings.Select(reading => new Dictionary<string, object?>
        {
            ["FullName"] = reading.Benchmark,
            ["DisplayInfo"] = $"{WithoutParameters(reading.Benchmark)}: {displayJob}{ParameterSuffix}",
            ["Statistics"] = new Dictionary<string, object?> { ["Mean"] = reading.Mean },
            ["Memory"] = reading.Bytes is { } bytes
                ? new Dictionary<string, object?> { ["BytesAllocatedPerOperation"] = bytes }
                : null,
        });

        var report = new Dictionary<string, object?>
        {
            ["Title"] = "DSAExperimentation.Benchmarks",
            ["HostEnvironmentInfo"] = new Dictionary<string, object?>
            {
                ["ProcessorName"] = "Test Processor",
                ["RuntimeVersion"] = ".NET 10.0.0",
            },
            ["Benchmarks"] = benchmarks,
        };

        return JsonSerializer.Serialize(report);
    }

    // BenchmarkDotNet names a benchmark twice, differently, and the difference is the whole
    // reason this helper composes the two fields separately. FullName carries the parameters
    // inside the method's own parentheses - "ReduceOrderBenchmarks.Recursive(Size: 10000)" -
    // while DisplayInfo leaves them off the name and puts them after the job instead. That
    // matters because "(Size: 10000)" contains a colon and a space, so a reader that looked
    // for the first ": " in a DisplayInfo built the other way round would find the parameter
    // list and take the rest of the name for the job. It would then fail, silently and
    // symmetrically, to tell one job from another.
    public static string WithoutParameters(string benchmark)
    {
        var parameters = benchmark.IndexOf('(', StringComparison.Ordinal);

        return parameters < 0 ? benchmark : benchmark[..parameters];
    }
}
