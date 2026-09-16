using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListComponentsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a linear scan of nums for every list node against this repo's
// own Set<int> membership - so a harness whose arms disagree is timing two different problems. Setup
// builds a 0..Length-1 chain and includes only its even values in nums, in descending order, so the
// same Length must rebuild the same pair; otherwise two published numbers were never comparable.
//
// That same documented workload fixes the answer independently of either arm: the chain alternates
// even and odd, so no two values in nums are adjacent in the list and every included node is a
// component of its own - Length / 2 singleton components. The Setup test asserts the arm against that
// count, so a harness that silently changed the workload fails here rather than only in agreement.
public sealed partial class LinkedListComponentsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int EvenDivisor = 2;
    private const int ExpectedSingletonComponents = SmallestLength / EvenDivisor;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSingletonComponents()
    {
        Assert.Equal(ExpectedSingletonComponents, BuildHarness().LinearScanPerNode());
        Assert.Equal(BuildHarness().SetMembership(), BuildHarness().SetMembership());
    }

    [Fact]
    public void LinearScanPerNode_EveryOtherValueIncluded_AgreesWithSetMembership()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetMembership(), harness.LinearScanPerNode());
    }

    [Fact]
    public void SetMembership_EveryOtherValueIncluded_AgreesWithLinearScanPerNode()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanPerNode(), harness.SetMembership());
    }

    private static LinkedListComponentsBenchmarks BuildHarness()
    {
        var harness = new LinkedListComponentsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
