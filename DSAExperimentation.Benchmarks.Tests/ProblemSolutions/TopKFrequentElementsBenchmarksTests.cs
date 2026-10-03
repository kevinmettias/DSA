using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TopKFrequentElementsBenchmarks (ARCHITECTURE 17.9), for the one thing
// BenchmarkArmsTests cannot pin: WHICH values the arms select. The workload plants its answer -
// the values 0..9 each outnumber every other value by construction - so the expected selection is
// known without consulting either arm. It is checked at both benchmarked lengths because the
// planted margin is computed from Length, and the generic check runs only the smallest.
//
// An earlier workload drew every value at random and tied 23 values at the top-10 boundary,
// outside LC 347's guarantee of a unique answer; each arm returned a different 8 of the 23, and
// this file could only check that each selection had the right occurrence counts. These tests
// are what would catch a tie again.
public sealed partial class TopKFrequentElementsBenchmarksTests
{
    private const int SmallestLength = 1_000;
    private const int LargestLength = 50_000;
    private const int TopCount = 10;

    // Both [Params] values the benchmark measures.
    public static TheoryData<int> BenchmarkedLengths => new() { SmallestLength, LargestLength };

    [Theory]
    [MemberData(nameof(BenchmarkedLengths))]
    public void FullSort_PlantedWorkload_SelectsExactlyThePlantedValues(int length) =>
        Assert.Equal(PlantedValues(), BuildHarness(length).FullSort().Order());

    [Theory]
    [MemberData(nameof(BenchmarkedLengths))]
    public void SizeKMinHeap_PlantedWorkload_SelectsExactlyThePlantedValues(int length) =>
        Assert.Equal(PlantedValues(), BuildHarness(length).SizeKMinHeap().Order());

    private static IEnumerable<int> PlantedValues() => Enumerable.Range(0, TopCount);

    private static TopKFrequentElementsBenchmarks BuildHarness(int length)
    {
        var harness = new TopKFrequentElementsBenchmarks { Length = length };
        harness.Setup();

        return harness;
    }
}
