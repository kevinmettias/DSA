using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Tests.Algorithms.Searching;

// AtLeast holds from its threshold upward and AtMost up to its threshold, so each search's answer is
// known in closed form and every case below is a statement about the boundary, not about the rule.
// The range [10, 20] keeps "none holds" distinguishable from a real answer at either end: FirstTrue
// reports none as 21 and LastTrue as 9, neither of which a rule inside the range can produce.
public sealed partial class MonotonePredicateSearchTests
{
    private const int Low = 10;
    private const int High = 20;

    [Fact]
    public void FirstTrue_ThresholdInsideRange_ReturnsTheThreshold() =>
        Assert.Equal(14, MonotonePredicateSearch.FirstTrue(Low, High, new AtLeast(14)));

    [Fact]
    public void FirstTrue_HoldsEverywhere_ReturnsLow() =>
        Assert.Equal(Low, MonotonePredicateSearch.FirstTrue(Low, High, new AtLeast(3)));

    [Fact]
    public void FirstTrue_HoldsOnlyAtHigh_ReturnsHigh() =>
        Assert.Equal(High, MonotonePredicateSearch.FirstTrue(Low, High, new AtLeast(High)));

    [Fact]
    public void FirstTrue_HoldsNowhere_ReturnsOnePastHigh() =>
        Assert.Equal(High + 1, MonotonePredicateSearch.FirstTrue(Low, High, new AtLeast(99)));

    // low = high + 1 states the empty range; there is nothing to ask, so the answer is "none".
    [Fact]
    public void FirstTrue_EmptyRange_ReturnsLowWithoutAsking() =>
        Assert.Equal(Low, MonotonePredicateSearch.FirstTrue(Low, Low - 1, new NeverAsked()));

    // The reason the search is generic: a boundary past int.MaxValue, found in long.
    [Fact]
    public void FirstTrue_LongRangePastInt_FindsTheBoundary() =>
        Assert.Equal(
            50_000_000_000L,
            MonotonePredicateSearch.FirstTrue(1L, 100_000_000_000L, new LongAtLeast(50_000_000_000L)));

    [Fact]
    public void FirstTrue_HighAtMaxValue_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MonotonePredicateSearch.FirstTrue(0, int.MaxValue, new AtLeast(5)));

    [Fact]
    public void FirstTrue_LowPastHighPlusOne_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MonotonePredicateSearch.FirstTrue(Low, Low - 2, new AtLeast(5)));

    // [-2, int.MaxValue - 1] holds int.MaxValue + 1 candidates, one more than int can count.
    [Fact]
    public void FirstTrue_WidthPastMaxValue_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MonotonePredicateSearch.FirstTrue(-2, int.MaxValue - 1, new AtLeast(5)));

    [Fact]
    public void FirstTrue_NegativeRangeThatFits_FindsTheBoundary() =>
        Assert.Equal(-3, MonotonePredicateSearch.FirstTrue(-10, 10, new AtLeast(-3)));

    [Fact]
    public void LastTrue_ThresholdInsideRange_ReturnsTheThreshold() =>
        Assert.Equal(17, MonotonePredicateSearch.LastTrue(Low, High, new AtMost(17)));

    [Fact]
    public void LastTrue_HoldsEverywhere_ReturnsHigh() =>
        Assert.Equal(High, MonotonePredicateSearch.LastTrue(Low, High, new AtMost(99)));

    [Fact]
    public void LastTrue_HoldsOnlyAtLow_ReturnsLow() =>
        Assert.Equal(Low, MonotonePredicateSearch.LastTrue(Low, High, new AtMost(Low)));

    [Fact]
    public void LastTrue_HoldsNowhere_ReturnsOneBeforeLow() =>
        Assert.Equal(Low - 1, MonotonePredicateSearch.LastTrue(Low, High, new AtMost(3)));

    [Fact]
    public void LastTrue_EmptyRange_ReturnsHighWithoutAsking() =>
        Assert.Equal(Low - 1, MonotonePredicateSearch.LastTrue(Low, Low - 1, new NeverAsked()));

    [Fact]
    public void LastTrue_LowAtMinValue_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MonotonePredicateSearch.LastTrue(int.MinValue, 0, new AtMost(5)));

    [Fact]
    public void LastTrue_HighAtMaxValue_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MonotonePredicateSearch.LastTrue(0, int.MaxValue, new AtMost(5)));

    // Every candidate the search asks about must lie in [low, high]; a rule may index unguarded.
    [Fact]
    public void FirstTrue_EveryProbe_StaysInsideTheRange()
    {
        var probes = new List<int>();

        MonotonePredicateSearch.FirstTrue(Low, High, new RecordingAtLeast(15, probes));

        Assert.All(probes, probe => Assert.InRange(probe, Low, High));
    }

    [Fact]
    public void LastTrue_EveryProbe_StaysInsideTheRange()
    {
        var probes = new List<int>();

        MonotonePredicateSearch.LastTrue(Low, High, new RecordingAtMost(12, probes));

        Assert.All(probes, probe => Assert.InRange(probe, Low, High));
    }

    private readonly struct AtLeast(int threshold) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int candidate) => candidate >= threshold;
    }

    private readonly struct AtMost(int threshold) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int candidate) => candidate <= threshold;
    }

    private readonly struct LongAtLeast(long threshold) : IMonotonePredicate<long>
    {
        public bool IsSatisfiedBy(long candidate) => candidate >= threshold;
    }

    private readonly struct NeverAsked : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int candidate) => throw new InvalidOperationException("An empty range has no candidate to ask about.");
    }

    // The list is a reference, so the copy the search makes still records into it.
    private readonly struct RecordingAtLeast(int threshold, List<int> probes) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int candidate)
        {
            probes.Add(candidate);
            return candidate >= threshold;
        }
    }

    private readonly struct RecordingAtMost(int threshold, List<int> probes) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int candidate)
        {
            probes.Add(candidate);
            return candidate <= threshold;
        }
    }
}
