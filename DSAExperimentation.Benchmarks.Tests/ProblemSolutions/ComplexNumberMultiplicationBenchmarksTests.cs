using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ComplexNumberMultiplicationBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins every product against the workload's construction alone.
//
// Setup draws each operand's real and imaginary parts from one seeded Random in a fixed order, so
// the pairs can be restated here and multiplied with plain integer arithmetic, (a+bi)(c+di) =
// (ac-bd) + (ad+bc)i, in LeetCode's "real+imaginaryi" format - a negative imaginary part keeps its
// "+", as in LC 537's "0+-2i".
public sealed partial class ComplexNumberMultiplicationBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int RandomSeed = 7;
    private const int MinComponentValue = -100;
    private const int MaxComponentValueExclusive = 101;

    [Fact]
    public void StringSplitParse_SeededOperandPairs_ReturnsEveryPairsProduct() =>
        Assert.Equal(ExpectedProducts(), BuildHarness().StringSplitParse());

    [Fact]
    public void SpanParse_SeededOperandPairs_ReturnsEveryPairsProduct() =>
        Assert.Equal(ExpectedProducts(), BuildHarness().SpanParse());

    // [GlobalSetup]'s pairs, restated: real then imaginary for A, then for B, one pair at a time.
    private static string[] ExpectedProducts()
    {
        var random = new Random(RandomSeed);
        var products = new string[SmallestLength];

        for (var i = 0; i < SmallestLength; i++)
        {
            var (realA, imaginaryA) = (Draw(random), Draw(random));
            var (realB, imaginaryB) = (Draw(random), Draw(random));

            products[i] = $"{(realA * realB) - (imaginaryA * imaginaryB)}+{(realA * imaginaryB) + (imaginaryA * realB)}i";
        }

        return products;
    }

    private static int Draw(Random random) => random.Next(MinComponentValue, MaxComponentValueExclusive);

    private static ComplexNumberMultiplicationBenchmarks BuildHarness()
    {
        var harness = new ComplexNumberMultiplicationBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
