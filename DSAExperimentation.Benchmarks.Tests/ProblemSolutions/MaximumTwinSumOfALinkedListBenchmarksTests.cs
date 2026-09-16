using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumTwinSumOfALinkedListBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - copying the chain into an array and walking it from
// both ends against draining a deque of the same values front and back - so a harness whose arms
// disagree is timing two different problems. Both arms return the twin sum as an int, so they are
// compared directly, and neither strategy mutates the chain it is handed, so one harness is safe to
// read twice in either order. Setup draws the values from one fixed seed, so the same Length must
// rebuild the same chain and with it the same twin sum.
public sealed partial class MaximumTwinSumOfALinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArrayIndexTwoPointer(), BuildHarness().ArrayIndexTwoPointer());

    [Fact]
    public void ArrayIndexTwoPointer_SeededChain_AgreesWithDequeFrontBackDrain()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DequeFrontBackDrain(), harness.ArrayIndexTwoPointer());
    }

    [Fact]
    public void DequeFrontBackDrain_SeededChain_AgreesWithArrayIndexTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayIndexTwoPointer(), harness.DequeFrontBackDrain());
    }

    private static MaximumTwinSumOfALinkedListBenchmarks BuildHarness()
    {
        var harness = new MaximumTwinSumOfALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
