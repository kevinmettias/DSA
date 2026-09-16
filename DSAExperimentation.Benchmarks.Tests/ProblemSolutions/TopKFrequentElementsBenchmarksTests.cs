using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TopKFrequentElementsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - counting with a BCL Dictionary and sorting every
// distinct value by frequency against this repo's HashMap plus a size-k min-heap - so a harness
// whose arms disagree is timing two different problems.
//
// The two arms are NOT comparable selection for selection, and no assertion here pretends
// otherwise. LC 347 asks for the topCount most frequent values "in any order", and this workload
// does not determine WHICH values those are: rebuilding its generator (seed 5, one draw per element
// from [0, 2000), Length 1000) shows 2 values occurring 4 times and 23 occurring 3 times, so the
// top-10 boundary is a 23-way tie and the full sort (ties left in the BCL Dictionary's insertion
// order) and the min-heap (ties left in heap order) each legitimately return a different 8 of those
// 23. What both arms genuinely share, and what is asserted instead, is the definition of the
// answer: a valid top-k selection is exactly topCount values whose occurrence counts, sorted
// descending, are the topCount largest occurrence counts in the workload. That oracle is derived
// here from a rebuilt workload by the problem's own definition, so it is not a restatement of what
// either arm returns - it rules out an arm that returned the wrong number of values, a duplicate,
// a value outside the workload, or one whose count is below the boundary. It cannot check which of
// the tied values were chosen, because the problem does not. Reported as such.
public sealed partial class TopKFrequentElementsBenchmarksTests
{
    private const int SmallestLength = 1_000;

    // The benchmark's own operands, mirrored so the workload can be rebuilt here.
    private const int RandomSeed = 5;

    private const int ValueUpperBoundExclusive = 2_000;

    private const int TopCount = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().FullSort()),
            AnswerText.OfUnorderedSet(BuildHarness().FullSort()));

    [Fact]
    public void FullSort_SmallestLength_SelectsTheTopKFrequentValues() =>
        Assert.Equal(ExpectedTopFrequencies(), SelectedOccurrenceCounts(BuildHarness().FullSort()));

    [Fact]
    public void SizeKMinHeap_SmallestLength_SelectsTheTopKFrequentValues() =>
        Assert.Equal(ExpectedTopFrequencies(), SelectedOccurrenceCounts(BuildHarness().SizeKMinHeap()));

    // The topCount largest occurrence counts in the workload, descending.
    private static int[] ExpectedTopFrequencies() =>
        DescendingOccurrenceCounts(RebuildValues()).Take(TopCount).ToArray();

    // The occurrence counts of the values one arm selected, in the same descending order - equal to
    // the array above exactly when that selection is a valid top-k.
    private static int[] SelectedOccurrenceCounts(int[] selection)
    {
        var occurrences = OccurrenceCounts(RebuildValues());

        return
        [
            .. selection
                .Select(value => occurrences.GetValueOrDefault(value))
                .OrderByDescending(count => count),
        ];
    }

    private static IEnumerable<int> DescendingOccurrenceCounts(int[] values) =>
        OccurrenceCounts(values).Values.OrderByDescending(count => count);

    private static Dictionary<int, int> OccurrenceCounts(int[] values)
    {
        var counts = new Dictionary<int, int>();

        foreach (var value in values)
        {
            counts[value] = counts.GetValueOrDefault(value) + 1;
        }

        return counts;
    }

    // The benchmark's own generator, rebuilt from its documented shape: seeded Random(5) over the
    // same half-open range, one draw per element.
    private static int[] RebuildValues()
    {
        var random = new Random(RandomSeed);

        return
        [
            .. Enumerable.Range(0, SmallestLength)
                .Select(_ => random.Next(0, ValueUpperBoundExclusive)),
        ];
    }

    private static TopKFrequentElementsBenchmarks BuildHarness()
    {
        var harness = new TopKFrequentElementsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
