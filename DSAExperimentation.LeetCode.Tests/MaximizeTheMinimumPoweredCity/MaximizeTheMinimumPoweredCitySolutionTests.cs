using DSAExperimentation.LeetCode.MaximizeTheMinimumPoweredCity;

namespace DSAExperimentation.LeetCode.Tests.MaximizeTheMinimumPoweredCity;

// Harness only. Both strategies are MaximizeTheMinimumPoweredCitySolution's - the
// descending linear scan that used to live only in the benchmark's baseline arm, and
// the predicate search over the target range that the test used to inline as an
// infeasibility sequence. The greedy feasibility sweep and the rule the search asks
// about both moved down beside the solution.
public sealed partial class MaximizeTheMinimumPoweredCitySolutionTests
{
    // LC 2528's own ceilings: 10^5 cities, 10^5 stations at each, a budget of 10^9.
    private const int CityCountAtScale = 100_000;
    private const int StationsPerCityAtScale = 100_000;
    private const int ExtraStationsAtScale = 1_000_000_000;

    // Each city is covered by its own stations alone.
    private const int SelfOnlyRadius = 0;

    public static TheoryData<int[], int, int, long> Examples =>
        new()
        {
            // LC examples 1 and 2.
            { [1, 2, 4, 5, 0], 1, 2, 5 },
            { [4, 4, 4, 4], 0, 3, 4 },

            // One city that reaches nothing else, so every extra station goes to it.
            { [3], 0, 5, 8 },

            // Nothing anywhere and nothing to build: the weakest city stays at zero.
            { [0], 0, 0, 0 },

            // Two cities that each cover the other, so both extra stations count twice.
            { [0, 0], 1, 2, 2 },

            // A radius wider than the whole array: every station powers every city.
            { [1, 1, 1], 5, 3, 6 },

            // The greedy placement matters: the single extra station can lift the two
            // ends or the middle, never both, so the answer stays at 1.
            { [1, 0, 0, 0, 1], 1, 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPowerByDescendingLinearScan_LeetCodeExamples_ReturnsMaximizedMinimumPower(
        int[] stations, int radius, int extraStations, long expected)
    {
        var actual = MaximizeTheMinimumPoweredCitySolution.MaxPowerByDescendingLinearScan(
            stations, radius, extraStations);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPowerByPredicateSearch_LeetCodeExamples_ReturnsMaximizedMinimumPower(
        int[] stations, int radius, int extraStations, long expected)
    {
        var actual = MaximizeTheMinimumPoweredCitySolution.MaxPowerByPredicateSearch(
            stations, radius, extraStations);

        Assert.Equal(expected, actual);
    }

    // At the problem's ceilings sum(stations) + extraStations is 1.1 * 10^10, past
    // int.MaxValue, so a target range cut down to an int index loses its top and the
    // search answers for a range that does not exist. With radius 0 no station helps a
    // neighbour, so the best the budget can do is lift every city by the same share -
    // a closed form, not another arm. Only the bisecting arm is asked: the descending
    // scan would walk 10^10 targets.
    [Fact]
    public void MaxPowerByPredicateSearch_TargetRangePastIntMaxValue_LiftsEveryCityByAnEvenShare()
    {
        int[] stations = [.. Enumerable.Repeat(StationsPerCityAtScale, CityCountAtScale)];

        var actual = MaximizeTheMinimumPoweredCitySolution.MaxPowerByPredicateSearch(
            stations, SelfOnlyRadius, ExtraStationsAtScale);

        Assert.Equal(StationsPerCityAtScale + ((long)ExtraStationsAtScale / CityCountAtScale), actual);
    }
}
