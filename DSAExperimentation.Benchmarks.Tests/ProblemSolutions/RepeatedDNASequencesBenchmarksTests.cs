using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RepeatedDNASequencesBenchmarks (ARCHITECTURE 17.9): the class carries a single
// arm - the fixed-window scan - so there is no second strategy to reconcile it against and the
// assertion has to come from the arm's own declared contract instead. That contract is decisive: the
// result is every 10-letter window occurring more than once, each reported once, in order of its first
// occurrence. The oracle below re-derives exactly that from the same seeded fixture by counting windows
// in a dictionary, which is a genuinely different route to the answer than the arm's two sliding-window
// passes over this repo's Set<string>. [Params] sweeps 200 and 5,000, and the 5,000 sweep is the one
// used here: at 200 a random A/C/G/T string of that length is expected to hold no repeated 10-mer at
// all, which would make the comparison [] == [] and witness nothing.
public sealed partial class RepeatedDNASequencesBenchmarksTests
{
    // The benchmark sweeps 200 and 5,000; 5,000 is the sweep whose workload actually holds repeats.
    private const int RepeatBearingLength = 5_000;

    // Mirrors the benchmark's own seed so the oracle counts windows of the very string the arm scans.
    private const int WorkloadSeed = 187;
    private const int TenMerLength = 10;
    private const int OccurrencesThatMakeItRepeated = 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().FixedWindowSet()),
            AnswerText.Of(BuildHarness().FixedWindowSet()));

    [Fact]
    public void FixedWindowSet_SeededSequence_ReportsEveryRepeatedTenMerOnceInFirstOccurrenceOrder()
    {
        var harness = BuildHarness();
        var expected = RepeatedTenMers();
        var reported = harness.FixedWindowSet();

        Assert.NotEmpty(expected);
        Assert.Equal(AnswerText.Of(expected), AnswerText.Of(reported));
    }

    private static RepeatedDNASequencesBenchmarks BuildHarness()
    {
        var harness = new RepeatedDNASequencesBenchmarks { Length = RepeatBearingLength };
        harness.Setup();

        return harness;
    }

    private static string[] RepeatedTenMers()
    {
        var sequence = DnaSequenceWorkloads.BuildSequence(RepeatBearingLength, WorkloadSeed);
        var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);

        for (var i = 0; i + TenMerLength <= sequence.Length; i++)
        {
            var window = sequence.Substring(i, TenMerLength);
            occurrences[window] = occurrences.GetValueOrDefault(window) + 1;
        }

        var repeated = new List<string>();
        var reported = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i + TenMerLength <= sequence.Length; i++)
        {
            var window = sequence.Substring(i, TenMerLength);

            if (occurrences[window] >= OccurrencesThatMakeItRepeated && reported.Add(window))
            {
                repeated.Add(window);
            }
        }

        return [.. repeated];
    }
}
