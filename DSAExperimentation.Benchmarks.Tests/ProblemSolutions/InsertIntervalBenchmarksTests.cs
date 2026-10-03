using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertIntervalBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: the
// merged intervals themselves, which the fixture fixes without consulting either arm. Interval i is [3i, 3i+1] and
// the new interval is [Length, 2 * Length], so at Length = 200 the new interval [200, 400] swallows the chain of
// intervals 67..133 - whose starts 201..399 all fall inside it, the last ending at 400 - and nothing else, since
// interval 66 ends at 199 and interval 134 starts at 402. That leaves the 67 intervals before the chain, the one
// merged block [200, 400], and the 66 after it: 134 in all. Each arm returns the merged intervals.
public sealed partial class InsertIntervalBenchmarksTests
{
    private const int SmallestLength = 200;

    // The fixture's spacing: interval i is [Spacing * i, Spacing * i + 1].
    private const int IntervalSpacing = 3;

    // 67 intervals before the swallowed chain, one merged block, 66 after it.
    private const int ExpectedMergedIntervalCount = 134;

    private const int FirstSwallowedInterval = 67;
    private const int FirstIntervalAfterTheBlock = 134;

    // The new interval [Length, 2 * Length] at the smallest Length.
    private const int NewIntervalStart = SmallestLength;
    private const int NewIntervalEnd = 2 * SmallestLength;

    [Fact]
    public void ListInsertAndMerge_NewIntervalSwallowsAChain_MergesItIntoOneBlock()
    {
        var merged = BuildHarness().ListInsertAndMerge();

        Assert.Equal(ExpectedMergedIntervalCount, merged.Count);
        Assert.Equal(ExpectedIntervals(), merged);
    }

    [Fact]
    public void IntervalSetAdd_NewIntervalSwallowsAChain_MergesItIntoOneBlock()
    {
        var merged = BuildHarness().IntervalSetAdd();

        Assert.Equal(ExpectedMergedIntervalCount, merged.Count);
        Assert.Equal(ExpectedIntervals(), merged);
    }

    private static IEnumerable<(int Start, int End)> ExpectedIntervals() =>
        Enumerable.Range(0, FirstSwallowedInterval).Select(FixtureInterval)
            .Append((NewIntervalStart, NewIntervalEnd))
            .Concat(Enumerable.Range(FirstIntervalAfterTheBlock, SmallestLength - FirstIntervalAfterTheBlock).Select(FixtureInterval));

    private static (int Start, int End) FixtureInterval(int index) =>
        (index * IntervalSpacing, (index * IntervalSpacing) + 1);

    private static InsertIntervalBenchmarks BuildHarness()
    {
        var harness = new InsertIntervalBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
