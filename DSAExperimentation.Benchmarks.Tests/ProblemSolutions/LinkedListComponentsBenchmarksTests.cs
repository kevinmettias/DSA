using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListComponentsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on the smallest workload, so this asserts only the count the fixture
// fixes independently of either arm. Setup builds a 0..Length-1 chain and includes only its even
// values in nums, in descending order. The chain alternates even and odd, so no two values in nums
// are adjacent in the list and every included node is a component of its own - Length / 2
// singleton components.
public sealed partial class LinkedListComponentsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int EvenDivisor = 2;
    private const int ExpectedSingletonComponents = SmallestLength / EvenDivisor;

    [Fact]
    public void LinearScanPerNode_EveryOtherValueIncluded_CountsOneComponentPerIncludedNode() =>
        Assert.Equal(ExpectedSingletonComponents, BuildHarness().LinearScanPerNode());

    private static LinkedListComponentsBenchmarks BuildHarness()
    {
        var harness = new LinkedListComponentsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
