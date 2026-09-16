using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ProductOfArrayExceptSelfBenchmarks (ARCHITECTURE 17.9): the class carries a
// single arm - the prefix/suffix pass LC 238 asks for - so there is no second strategy to reconcile it
// against and the assertion has to be supplied instead. The workload is a seeded random array whose
// generator the class comment documents (one draw per element from the same half-open range), so the
// expected answer is derived here from a rebuilt array by the definition of the problem - every
// element but index i, multiplied out - rather than restated from the arm. The length cap and the
// half-open bounds are the benchmark's own operands; the rebuilt array is what a change to them would
// move, so the arm is pinned against arithmetic rather than against itself.
public sealed partial class ProductOfArrayExceptSelfBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int RandomSeed = 238;

    private const int MinValue = -1_000;

    private const int MaxValueExclusive = 1_000;

    [Fact]
    public void Setup_SeededNumbers_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PrefixSuffixPass()),
            AnswerText.Of(BuildHarness().PrefixSuffixPass()));

    [Fact]
    public void PrefixSuffixPass_SeededNumbers_MatchesTheIndependentlyMultipliedProducts() =>
        Assert.Equal(
            AnswerText.Of(IndependentProductsExceptSelf(RebuildNumbers())),
            AnswerText.Of(BuildHarness().PrefixSuffixPass()));

    private static ProductOfArrayExceptSelfBenchmarks BuildHarness()
    {
        var harness = new ProductOfArrayExceptSelfBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own generator, rebuilt from its documented shape: seeded Random(238) over the
    // same half-open range, one draw per element.
    private static int[] RebuildNumbers()
    {
        var random = new Random(RandomSeed);

        return
        [
            .. Enumerable.Range(0, SmallestLength)
                .Select(_ => random.Next(MinValue, MaxValueExclusive)),
        ];
    }

    // The definition of the answer written out directly: every element except index i, multiplied
    // together. No prefix/suffix pass and no division, so the zero values the range admits need no
    // special case - and because int multiplication wraps modulo 2^32, the order of the factors
    // cannot make this disagree with a correct prefix/suffix pass.
    private static int[] IndependentProductsExceptSelf(int[] nums)
    {
        var products = new int[nums.Length];

        for (var index = 0; index < nums.Length; index++)
        {
            products[index] = ProductOfEveryElementExcept(nums, index);
        }

        return products;
    }

    private static int ProductOfEveryElementExcept(int[] nums, int excludedIndex)
    {
        var product = 1;

        for (var index = 0; index < nums.Length; index++)
        {
            if (index != excludedIndex)
            {
                product *= nums[index];
            }
        }

        return product;
    }
}
