using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ProductOfArrayExceptSelfBenchmarks (ARCHITECTURE 17.9): the class carries a
// single arm - the prefix/suffix pass LC 238 asks for - so there is no second strategy to reconcile it
// against and the assertion has to be supplied instead. The workload is a seeded array the class
// comment documents (BoundedProductDraws' signed factors, inside LC 238's [-30, 30]), so the expected
// answer is derived here from a rebuilt array by the definition of the problem - every element but
// index i, multiplied out - rather than restated from the arm. The length and the magnitude bound are
// the benchmark's own operands; the rebuilt array is what a change to them would move, so the arm is
// pinned against arithmetic rather than against itself.
public sealed partial class ProductOfArrayExceptSelfBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int RandomSeed = 238;

    private const int MagnitudeBoundExclusive = 31;

    [Fact]
    public void Setup_SeededNumbers_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().PrefixSuffixPass()),
            AnswerGraphText.Of(BuildHarness().PrefixSuffixPass()));

    [Fact]
    public void PrefixSuffixPass_SeededNumbers_MatchesTheIndependentlyMultipliedProducts() =>
        Assert.Equal(
            AnswerGraphText.Of(IndependentProductsExceptSelf(RebuildNumbers())),
            AnswerGraphText.Of(BuildHarness().PrefixSuffixPass()));

    private static ProductOfArrayExceptSelfBenchmarks BuildHarness()
    {
        var harness = new ProductOfArrayExceptSelfBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own workload, rebuilt from its documented shape: seeded Random(238) handed to
    // BoundedProductDraws for signed factors under the same magnitude bound.
    private static int[] RebuildNumbers() =>
        BoundedProductDraws.SignedFactors(SmallestLength, MagnitudeBoundExclusive, new Random(RandomSeed));

    // The definition of the answer written out directly: every element except index i, multiplied
    // together. No prefix/suffix pass and no division, and the multiplication is checked, so an answer
    // that left the 32 bits LC 238 guarantees would throw here rather than wrap into agreement.
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
                product = checked(product * nums[index]);
            }
        }

        return product;
    }
}
