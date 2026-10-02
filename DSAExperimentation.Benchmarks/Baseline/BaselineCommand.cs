using System.Globalization;
using System.Reflection;
using System.Text.Json;

using BenchmarkDotNet.Running;

namespace DSAExperimentation.Benchmarks.Baseline;

// The `baseline` verb: record a run, or compare one against the recorded baseline.
//
// It sits behind the same executable as the benchmarks because it has to be able to run
// them - the whole point of the command is that one line reproduces a run and another
// checks it - and because a baseline is only comparable to something measured by the job
// BenchmarkConfig pins, which is over there.
//
// Both modes run the benchmarks unless `--report` points at files that already exist, so
// the numbers a comparison reads are the numbers a recording would have written. That is
// also what makes the command testable: everything decided here is decided from paths and
// text, and a test hands it both.
internal static class BaselineCommand
{
    public const string Name = "baseline";

    private const int Success = 0;
    private const int Regressed = 1;
    private const int Refused = 2;

    private const string ReportSuffix = "-report-full.json";
    private const string ArtifactsDirectory = "BenchmarkDotNet.Artifacts";
    private const string ResultsFolder = "results";
    private const string ProjectFolder = "DSAExperimentation.Benchmarks";
    private const string BaselineFileName = "baseline.tsv";
    private const string RootMarker = "ARCHITECTURE.md";
    private const double BytesPerKilobyte = 1_000;
    private const double BytesPerMegabyte = 1_000_000;
    private const double NanosecondsPerMicrosecond = 1_000;
    private const double NanosecondsPerMillisecond = 1_000_000;
    private const double Percent = 100;

    // The exit codes, because a comparison nobody can branch on is a report nobody reads:
    // 0 the run matches the baseline, 1 it regressed, 2 there is no verdict at all -
    // either the command could not run, or it ran and the two runs were not comparable.
    // A caller that treats 2 as "green" has inverted the meaning of the check.
    private const string Usage =
        """
        usage: dotnet run -c Release --project DSAExperimentation.Benchmarks -- baseline record  [--filter <glob>] [--job <name>] [--report <path>] [--baseline <path>]
               dotnet run -c Release --project DSAExperimentation.Benchmarks -- baseline compare [--filter <glob>] [--job <name>] [--report <path>] [--baseline <path>] [--tolerance <ratio>]

          --filter      which benchmarks to run, as BenchmarkDotNet's glob. Defaults to everything.
          --job         a BenchmarkDotNet job name, passed through. Leave it off to use the pinned
                        recording job; passing --job dry records a smoke run, and the job is written
                        into the baseline so that a later comparison against it fails rather than lies.
          --report      read existing -report-full.json files (a file or a directory) instead of running.
          --baseline    the baseline file, read by compare and written by record.
                        Defaults to <repo>/DSAExperimentation.Benchmarks/baseline.tsv
          --tolerance   compare only: the relative change treated as noise. Defaults to 0.10.
        """;

    public static bool Handles(string[] args) =>
        args.Length > 0 && string.Equals(args[0], Name, StringComparison.Ordinal);

    public static int Run(string[] args, TextWriter output, TextWriter error)
    {
        if (!BaselineOptions.TryParse(args, out var options, out var problem))
        {
            error.WriteLine($"{Name}: {problem}");
            error.WriteLine(Usage);

            return Refused;
        }

        var baselinePath = options.Baseline ?? DefaultBaselinePath();

        try
        {
            var run = BenchmarkBaseline.Merge(ReportTexts(options, output).Select(BenchmarkBaseline.Read));

            return options.Recording
                ? Record(options, baselinePath, run, output)
                : Compare(options, baselinePath, run, output);
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or JsonException or UnauthorizedAccessException)
        {
            error.WriteLine($"{Name}: {exception.Message}");

            return Refused;
        }
    }

    private static int Record(BaselineOptions options, string baselinePath, BenchmarkRun run, TextWriter output)
    {
        File.WriteAllText(baselinePath, BenchmarkBaseline.Record(run, options.Filter, DateOnly.FromDateTime(DateTime.Now)));

        output.WriteLine($"{Name}: recorded {run.Readings.Count} benchmarks to {baselinePath}");
        output.WriteLine($"{Name}: job {run.Job}");

        return Success;
    }

    private static int Compare(BaselineOptions options, string baselinePath, BenchmarkRun run, TextWriter output)
    {
        if (!File.Exists(baselinePath))
        {
            throw new FileNotFoundException(
                $"There is no baseline at {baselinePath}. Record one with '{Name} record' first.");
        }

        var comparison = BenchmarkBaseline.Compare(
            BenchmarkBaseline.Parse(File.ReadAllText(baselinePath)),
            run,
            options.Tolerance);

        Render(comparison, options.Tolerance, output);

        return comparison switch
        {
            { IsUsable: false } => Refused,
            { Regressed: true } => Regressed,
            _ => Success,
        };
    }

    private static void Render(BaselineComparison comparison, double tolerance, TextWriter output)
    {
        output.WriteLine($"{Name}: baseline job {comparison.BaselineJob}");
        output.WriteLine($"{Name}: this run   {comparison.CurrentJob}");

        if (!comparison.JobsMatch)
        {
            output.WriteLine(
                $"{Name}: the two runs used different jobs, so their numbers measure the job as much as the "
                + "code. Re-record the baseline under this job, or run the comparison without one.");
        }

        foreach (var name in comparison.Missing)
        {
            output.WriteLine($"{Name}: {name} is in the baseline and not in this run.");
        }

        WriteDeltas("regressed", comparison.Regressions, output);
        WriteDeltas("improved", comparison.Improvements, output);

        if (comparison.Added.Count > 0)
        {
            output.WriteLine(
                $"{Name}: {comparison.Added.Count} arms are newer than the baseline: "
                + $"{string.Join(", ", comparison.Added.Take(5))}. Re-record to cover them.");
        }

        output.WriteLine(
            $"{Name}: {comparison.WithinTolerance} arms within "
            + $"{(tolerance * Percent).ToString("0.#", CultureInfo.InvariantCulture)}% of the baseline");

        // "worse", not "slower": an arm can be reported for allocating more while its clock
        // improved, and calling that slower would send the reader looking at the wrong number.
        output.WriteLine(comparison switch
        {
            { IsUsable: false } => $"{Name}: NO VERDICT - the two runs are not comparable, see above",
            { Regressed: true } => $"{Name}: REGRESSED - {Count(comparison.Regressions.Count, "arm")} worse than the baseline",
            _ => $"{Name}: no regression",
        });
    }

    private static string Count(int number, string noun) =>
        number == 1 ? $"{number} {noun}" : $"{number} {noun}s";

    private static void WriteDeltas(string label, IReadOnlyList<BenchmarkDelta> deltas, TextWriter output)
    {
        if (deltas.Count == 0)
        {
            return;
        }

        output.WriteLine($"{Name}: {Count(deltas.Count, "arm")} {label}");

        foreach (var delta in deltas)
        {
            var change = (delta.Ratio * Percent - Percent).ToString("+0.0;-0.0;0.0", CultureInfo.InvariantCulture);

            output.WriteLine($"  {delta.Name}");
            output.WriteLine(
                $"      {change}% ({delta.Ratio.ToString("0.00", CultureInfo.InvariantCulture)}x): "
                + $"{FormatTime(delta.BaselineNanoseconds)} -> {FormatTime(delta.CurrentNanoseconds)}");
            output.WriteLine(
                $"      allocated {FormatBytes(delta.BaselineBytes)} -> {FormatBytes(delta.CurrentBytes)}"
                + FormatAllocationChange(delta));
        }
    }

    private static IReadOnlyList<string> ReportTexts(BaselineOptions options, TextWriter output)
    {
        if (options.Report is { } path)
        {
            return ExistingReports(path).Select(File.ReadAllText).ToList();
        }

        return RunBenchmarks(options, output);
    }

    private static IReadOnlyList<string> ExistingReports(string path)
    {
        if (File.Exists(path))
        {
            return [path];
        }

        if (Directory.Exists(path))
        {
            var reports = Directory
                .EnumerateFiles(path, $"*{ReportSuffix}")
                .OrderBy(file => file, StringComparer.Ordinal)
                .ToList();

            return reports.Count > 0
                ? reports
                : throw new InvalidDataException($"There are no {ReportSuffix} files in {path}.");
        }

        throw new FileNotFoundException($"There is no report file or directory at {path}.");
    }

    private static IReadOnlyList<string> RunBenchmarks(BaselineOptions options, TextWriter output)
    {
        List<string> arguments = ["--filter", options.Filter];

        if (options.Job is { } job)
        {
            arguments.Add("--job");
            arguments.Add(job);
        }

        var benchmarkArguments = arguments.ToArray();

        output.WriteLine($"{Name}: running {options.Filter}, which takes as long as the benchmarks do");

        var started = DateTime.UtcNow;

        BenchmarkSwitcher
            .FromAssembly(Assembly.GetExecutingAssembly())
            .Run(benchmarkArguments, BenchmarkConfig.For(benchmarkArguments));

        var produced = ReportsProducedSince(started);

        if (produced.Count == 0)
        {
            throw new InvalidDataException(
                $"The run produced no {ReportSuffix} files under {ResultsFolder}, so '{options.Filter}' matched no "
                + "benchmark. A filter that matches nothing is indistinguishable from a clean run if it is allowed "
                + "to pass.");
        }

        return produced.Select(File.ReadAllText).ToList();
    }

    // BenchmarkDotNet writes its reports under the working directory, and it names each one
    // after the class that produced it rather than after the run, so a run of several classes
    // is a set of files and cannot be found by name. What a run did produce is therefore
    // decided by when, not by what - everything rewritten since the run began.
    private static IReadOnlyList<string> ReportsProducedSince(DateTime startedUtc)
    {
        var results = Path.Combine(Environment.CurrentDirectory, ArtifactsDirectory, ResultsFolder);

        if (!Directory.Exists(results))
        {
            return [];
        }

        return Directory
            .EnumerateFiles(results, $"*{ReportSuffix}")
            .Where(file => File.GetLastWriteTimeUtc(file) >= startedUtc)
            .OrderBy(file => file, StringComparer.Ordinal)
            .ToList();
    }

    private static string DefaultBaselinePath() =>
        Path.Combine(RepositoryRoot(), ProjectFolder, BaselineFileName);

    // The same walk Architecture/RepositoryFiles.cs does in the test project, which cannot be
    // shared with this one because the test project is not on the benchmark project's
    // reference list - deliberately, since the benchmark project must not depend on its tests.
    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, RootMarker)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Environment.CurrentDirectory;
    }

    private static string FormatTime(double nanoseconds) => nanoseconds switch
    {
        >= NanosecondsPerMillisecond => FormattableString.Invariant(
            $"{nanoseconds / NanosecondsPerMillisecond:0.###} ms"),
        >= NanosecondsPerMicrosecond => FormattableString.Invariant(
            $"{nanoseconds / NanosecondsPerMicrosecond:0.###} us"),
        _ => FormattableString.Invariant($"{nanoseconds:0.###} ns"),
    };

    // The change in bytes, not only the two figures either side of it. Two large allocations
    // round to the same megabyte count when one has moved by a few hundred bytes, so without
    // this the reader is told an arm regressed with no way to see why.
    private static string FormatAllocationChange(BenchmarkDelta delta) => delta.AllocationChange switch
    {
        null => string.Empty,
        0 => string.Empty,
        var change => FormattableString.Invariant($" {change:+#,0;-#,0} B"),
    };

    private static string FormatBytes(long? bytes)
    {
        if (bytes is not { } allocated)
        {
            return "not measured";
        }

        if (allocated >= BytesPerMegabyte)
        {
            return FormattableString.Invariant($"{allocated / BytesPerMegabyte:0.##} MB");
        }

        return allocated >= BytesPerKilobyte
            ? FormattableString.Invariant($"{allocated / BytesPerKilobyte:0.##} KB")
            : FormattableString.Invariant($"{allocated} B");
    }
}
