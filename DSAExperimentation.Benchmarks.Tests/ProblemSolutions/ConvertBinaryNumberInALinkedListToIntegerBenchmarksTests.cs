using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConvertBinaryNumberInALinkedListToIntegerBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the single left-to-right shift-fold
// against collecting every bit into a buffer and folding positional weights - so a harness whose
// arms disagree is timing two different problems. Both arms return an int, so they are compared
// directly, and both fold under the same int arithmetic, so they agree value for value however long
// the list runs. The proxy those ints are worth here is honest and weak: both [Params] lengths
// exceed LC 1290's own thirty-bit contract, so the returned value is the low thirty-two bits of the
// bit string rather than its decimal value - still the same folding work per node, and still the
// same answer from both arms, but not a number the problem would ever ask for. Setup draws from one
// fixed seed, so the same Length must rebuild the same bit list.
public sealed partial class ConvertBinaryNumberInALinkedListToIntegerBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameBitList() =>
        Assert.Equal(BuildHarness().CollectThenFold(), BuildHarness().CollectThenFold());

    [Fact]
    public void CollectThenFold_TwoHundredBits_AgreesWithSinglePassShift()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SinglePassShift(), harness.CollectThenFold());
    }

    [Fact]
    public void SinglePassShift_TwoHundredBits_AgreesWithCollectThenFold()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CollectThenFold(), harness.SinglePassShift());
    }

    private static ConvertBinaryNumberInALinkedListToIntegerBenchmarks BuildHarness()
    {
        var harness = new ConvertBinaryNumberInALinkedListToIntegerBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
