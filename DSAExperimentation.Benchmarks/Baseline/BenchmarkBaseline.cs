using System.Globalization;
using System.Text;
using System.Text.Json;

namespace DSAExperimentation.Benchmarks.Baseline;

// Reading a BenchmarkDotNet report, recording one as a committed baseline, and comparing
// a fresh run against that baseline. Text goes in and text comes out; where the text came
// from and what to do about the verdict are BaselineCommand's business.
//
// That split is the whole reason this is a separate type: it lets record, parse and
// compare be tested against hand-written reports, with no benchmark run, no artifacts
// directory and no timing noise anywhere in the test.
//
// The baseline is a tab-separated file rather than the JSON it is read from, because the
// one thing a committed baseline is for is being read in a diff, and a JSON report diff
// is a wall of unchanged timestamp and hardware fields around the one number that moved.
internal static class BenchmarkBaseline
{
    private const string FieldSeparator = "\t";
    private const string MetadataPrefix = "# ";
    private const string CommentPrefix = "#";
    private const string Unmeasured = "-";
    private const string Unknown = "unknown";
    private const int RowFieldCount = 3;
    private const int NamesToNameInAFailure = 5;

    private const string JobKey = "job";
    private const string HostKey = "host";
    private const string CapturedKey = "captured";
    private const string FilterKey = "filter";
    private const string RecordCommandKey = "record";
    private const string CompareCommandKey = "compare";
    private const string ColumnsKey = "columns";

    private static readonly string[] MetadataKeys =
    [
        JobKey,
        HostKey,
        CapturedKey,
        FilterKey,
        RecordCommandKey,
        CompareCommandKey,
        ColumnsKey,
    ];

    // One report file, as BenchmarkDotNet wrote it. A run of several benchmark classes
    // writes one file per class, which is why the caller merges rather than this reads a
    // directory.
    public static BenchmarkRun Read(string reportText)
    {
        using var document = JsonDocument.Parse(reportText);
        var root = document.RootElement;

        if (!root.TryGetProperty("Benchmarks", out var benchmarks) || benchmarks.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("This report has no Benchmarks array, so it holds no measurements.");
        }

        var readings = new List<BenchmarkReading>();
        var jobs = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var benchmark in benchmarks.EnumerateArray())
        {
            var name = Required(benchmark, "FullName");

            jobs.Add(JobOf(Required(benchmark, "DisplayInfo")));
            readings.Add(new BenchmarkReading(name, MeanOf(benchmark, name), AllocatedBytesOf(benchmark)));
        }

        if (readings.Count == 0)
        {
            throw new InvalidDataException("This report holds no benchmarks, so there is nothing to record.");
        }

        if (jobs.Count > 1)
        {
            throw new InvalidDataException(
                $"This report mixes {jobs.Count} jobs ({string.Join(", ", jobs)}). A baseline records one job: "
                + "comparing across jobs measures the job, not the code.");
        }

        return Unduplicated(jobs.First(), HostOf(root), readings);
    }

    // Every report a run produced, as one run. The benchmark classes in a single run share
    // a job and a machine by construction, so a difference here means files from two runs
    // were collected, and merging them would compare a run against itself.
    public static BenchmarkRun Merge(IEnumerable<BenchmarkRun> runs)
    {
        var all = runs.ToList();

        if (all.Count == 0)
        {
            throw new InvalidDataException("There are no reports to merge.");
        }

        var jobs = all.Select(run => run.Job).Distinct(StringComparer.Ordinal).ToList();
        var hosts = all.Select(run => run.Host).Distinct(StringComparer.Ordinal).ToList();

        if (jobs.Count > 1 || hosts.Count > 1)
        {
            throw new InvalidDataException(
                $"These reports come from {jobs.Count} jobs on {hosts.Count} machines "
                + $"({string.Join(", ", jobs)} / {string.Join(", ", hosts)}), so they are not one run.");
        }

        var readings = all
            .SelectMany(run => run.Readings)
            .OrderBy(reading => reading.Name, StringComparer.Ordinal)
            .ToList();

        return Unduplicated(jobs[0], hosts[0], readings);
    }

    // The baseline file. Its header carries the two commands that reproduce and check it,
    // so the file is self-describing once it is in the repository and read on its own.
    public static string Record(BenchmarkRun run, string filter, DateOnly captured)
    {
        var text = new StringBuilder();

        text.AppendLine("# DSA benchmark baseline: one line per benchmark arm, sorted by name, so that a");
        text.AppendLine("# change is one changed line in a diff. Record it on a machine doing nothing else.");
        text.AppendLine(CommentPrefix);
        text.AppendLine(Metadata(RecordCommandKey, Command("record", filter)));
        text.AppendLine(Metadata(CompareCommandKey, Command("compare", filter)));
        text.AppendLine(Metadata(FilterKey, filter));
        text.AppendLine(Metadata(JobKey, run.Job));
        text.AppendLine(Metadata(HostKey, run.Host));
        text.AppendLine(Metadata(CapturedKey, captured.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        text.AppendLine(Metadata(ColumnsKey, $"name{FieldSeparator}meanNanoseconds{FieldSeparator}allocatedBytes"));

        foreach (var reading in run.Readings.OrderBy(reading => reading.Name, StringComparer.Ordinal))
        {
            text.AppendLine(string.Join(
                FieldSeparator,
                reading.Name,
                FormatMean(reading.MeanNanoseconds),
                FormatBytes(reading.AllocatedBytes)));
        }

        return text.ToString();
    }

    public static BenchmarkRun Parse(string baselineText)
    {
        var readings = new List<BenchmarkReading>();
        var job = Unknown;
        var host = Unknown;
        var lineNumber = 0;

        foreach (var line in Lines(baselineText))
        {
            lineNumber++;

            if (line.Length == 0 || line.StartsWith(CommentPrefix, StringComparison.Ordinal))
            {
                var (key, value) = MetadataOf(line);

                if (key == JobKey)
                {
                    job = value;
                }

                if (key == HostKey)
                {
                    host = value;
                }

                continue;
            }

            readings.Add(ParseRow(line, lineNumber));
        }

        if (readings.Count == 0)
        {
            throw new InvalidDataException("This baseline has no rows, so it is not one.");
        }

        return Unduplicated(job, host, readings);
    }

    // Arms that got worse, arms that got better, arms the baseline expects that this run does
    // not have, and arms this run has that the baseline does not. An arm is worse if its time
    // rose past the tolerance OR its allocation did, so a change that trades one for the other
    // is reported rather than cancelled out; see BenchmarkDelta for why allocation needs a
    // tolerance at all.
    public static BaselineComparison Compare(BenchmarkRun baseline, BenchmarkRun current, double tolerance)
    {
        var byName = current.Readings.ToDictionary(reading => reading.Name, StringComparer.Ordinal);
        var baselineNames = baseline.Readings.Select(reading => reading.Name).ToHashSet(StringComparer.Ordinal);

        var regressions = new List<BenchmarkDelta>();
        var improvements = new List<BenchmarkDelta>();
        var missing = new List<string>();
        var withinTolerance = 0;

        foreach (var before in baseline.Readings)
        {
            if (!byName.TryGetValue(before.Name, out var after))
            {
                missing.Add(before.Name);
                continue;
            }

            var delta = new BenchmarkDelta(
                before.Name,
                before.MeanNanoseconds,
                after.MeanNanoseconds,
                before.AllocatedBytes,
                after.AllocatedBytes);

            if (delta.Ratio > 1 + tolerance || delta.AllocationRegressed(tolerance))
            {
                regressions.Add(delta);
            }
            else if (delta.Ratio < 1 - tolerance || delta.AllocationImproved(tolerance))
            {
                improvements.Add(delta);
            }
            else
            {
                withinTolerance++;
            }
        }

        var added = current.Readings
            .Where(reading => !baselineNames.Contains(reading.Name))
            .Select(reading => reading.Name)
            .ToList();

        return new BaselineComparison(
            baseline.Job,
            current.Job,
            regressions,
            improvements,
            missing,
            added,
            withinTolerance);
    }

    private static string Command(string verb, string filter) =>
        $"dotnet run -c Release --project DSAExperimentation.Benchmarks -- baseline {verb} --filter \"{filter}\"";

    private static string Metadata(string key, string value) => $"{MetadataPrefix}{key}{FieldSeparator}{value}";

    // A prose comment and a metadata line both start with '#'; only the metadata line has a
    // recognised key before its first tab. An unrecognised key reads as prose rather than as
    // an error, so a later version can add a field without an older reader rejecting the file.
    private static (string Key, string Value) MetadataOf(string line)
    {
        if (!line.StartsWith(MetadataPrefix, StringComparison.Ordinal))
        {
            return (string.Empty, string.Empty);
        }

        var separator = line.IndexOf(FieldSeparator, StringComparison.Ordinal);

        if (separator < 0)
        {
            return (string.Empty, string.Empty);
        }

        var key = line[MetadataPrefix.Length..separator];

        return MetadataKeys.Contains(key, StringComparer.Ordinal)
            ? (key, line[(separator + 1)..])
            : (string.Empty, string.Empty);
    }

    private static BenchmarkReading ParseRow(string line, int lineNumber)
    {
        var fields = line.Split(FieldSeparator);

        if (fields.Length != RowFieldCount)
        {
            throw new InvalidDataException(
                $"Baseline line {lineNumber} has {fields.Length} fields; a row has {RowFieldCount}.");
        }

        if (!double.TryParse(fields[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var mean))
        {
            throw new InvalidDataException($"Baseline line {lineNumber} does not record a mean time: '{fields[1]}'.");
        }

        return new BenchmarkReading(fields[0], mean, ParseBytes(fields[2], lineNumber));
    }

    private static long? ParseBytes(string field, int lineNumber)
    {
        if (field == Unmeasured)
        {
            return null;
        }

        return long.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bytes)
            ? bytes
            : throw new InvalidDataException($"Baseline line {lineNumber} does not record an allocation: '{field}'.");
    }

    // "BalancedTreeFoldBenchmarks.Recursive: Job-ABCDEF(IterationCount=15, ...) [Size=200000]".
    // The job is what sits between the colon and the parameters, minus the hash
    // BenchmarkDotNet generates for a job it cannot name: the hash changes between runs that
    // measure the same way, so keeping it would make every baseline disagree with every other
    // about an identical job, and the job check would fail open on the mistake it exists for.
    private static string JobOf(string displayInfo)
    {
        var separator = displayInfo.IndexOf(": ", StringComparison.Ordinal);
        var rest = separator < 0 ? displayInfo : displayInfo[(separator + 2)..];
        var open = rest.IndexOf('(');

        // The job does not end at the first space: its counts are separated by ", ", so
        // cutting there would keep "IterationCount=15," and throw away the two counts that
        // are the whole point of recording the job. It ends at the parenthesis that closes
        // it, which is the last one before the space that introduces the parameter list.
        if (open > 0 && rest.StartsWith("Job", StringComparison.Ordinal))
        {
            var close = rest.IndexOf(')', open);

            if (close > open)
            {
                return rest[open..(close + 1)];
            }
        }

        // A named job - Dry, ShortRun - prints as the name alone, with no counts to lose.
        var end = rest.IndexOf(' ');

        return end < 0 ? rest : rest[..end];
    }

    private static double MeanOf(JsonElement benchmark, string name)
    {
        if (benchmark.TryGetProperty("Statistics", out var statistics)
            && statistics.ValueKind == JsonValueKind.Object
            && statistics.TryGetProperty("Mean", out var mean)
            && mean.TryGetDouble(out var value))
        {
            return value;
        }

        throw new InvalidDataException(
            $"{name} has no mean in this report. A benchmark that did not run is not a baseline row: "
            + "narrow the filter past it, or fix the run, before recording.");
    }

    private static long? AllocatedBytesOf(JsonElement benchmark) =>
        benchmark.TryGetProperty("Memory", out var memory)
        && memory.ValueKind == JsonValueKind.Object
        && memory.TryGetProperty("BytesAllocatedPerOperation", out var bytes)
        && bytes.TryGetInt64(out var value)
            ? value
            : null;

    private static string HostOf(JsonElement root)
    {
        if (!root.TryGetProperty("HostEnvironmentInfo", out var host) || host.ValueKind != JsonValueKind.Object)
        {
            return Unknown;
        }

        return $"{Optional(host, "ProcessorName")} | {Optional(host, "RuntimeVersion")}";
    }

    private static string Required(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? Unknown
            : throw new InvalidDataException($"A benchmark in this report has no {property}.");

    private static string Optional(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? Unknown
            : Unknown;

    private static BenchmarkRun Unduplicated(string job, string host, IReadOnlyList<BenchmarkReading> readings)
    {
        var duplicates = readings
            .GroupBy(reading => reading.Name, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .Take(NamesToNameInAFailure)
            .ToList();

        if (duplicates.Count > 0)
        {
            throw new InvalidDataException(
                $"These benchmarks appear more than once: {string.Join(", ", duplicates)}. Two rows with the "
                + "same name cannot both be compared against one baseline row.");
        }

        return new BenchmarkRun(job, host, readings);
    }

    private static string FormatMean(double nanoseconds) =>
        nanoseconds.ToString("0.######", CultureInfo.InvariantCulture);

    private static string FormatBytes(long? bytes) =>
        bytes is { } value ? value.ToString(CultureInfo.InvariantCulture) : Unmeasured;

    private static IEnumerable<string> Lines(string text) =>
        text.Split('\n').Select(line => line.TrimEnd('\r'));
}
