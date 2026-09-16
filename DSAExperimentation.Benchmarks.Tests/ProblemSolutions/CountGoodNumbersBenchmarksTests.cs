using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodNumbersBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - multiplying by the digit-choice count once per position against
// raising the same powers by squaring - so a harness whose arms disagree is timing two different
// problems. Both arms return an int, so they are compared directly. The class carries no
// [GlobalSetup] - the length is the whole workload - so the product the class comment states,
// 5^ceil(n/2) * 4^floor(n/2) mod 1e9+7, is asserted from the arm tests at the smallest [Params]
// length, computed here independently of both arms.
public sealed partial class CountGoodNumbersBenchmarksTests
{
    private const long SmallestLength = 1_000;
    private const int GoodNumbersOfLengthOneThousand = 36020987;

    [Fact]
    public void RepeatedMultiplication_LengthOneThousand_AgreesWithExponentiationBySquaringAndTheDigitProduct()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ExponentiationBySquaring(), harness.RepeatedMultiplication());
        Assert.Equal(GoodNumbersOfLengthOneThousand, harness.RepeatedMultiplication());
    }

    [Fact]
    public void ExponentiationBySquaring_LengthOneThousand_AgreesWithRepeatedMultiplicationAndTheDigitProduct()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedMultiplication(), harness.ExponentiationBySquaring());
        Assert.Equal(GoodNumbersOfLengthOneThousand, harness.ExponentiationBySquaring());
    }

    private static CountGoodNumbersBenchmarks BuildHarness() =>
        new() { Length = SmallestLength };
}
