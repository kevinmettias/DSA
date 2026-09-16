using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.BasicCalculator;
using DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;
using DSAExperimentation.LeetCode.DecodeString;

namespace DSAExperimentation.Benchmarks.Tests;

// Harness coverage for the seam between the benchmark tree and the LeetCode tree
// (ARCHITECTURE 17.9). Every benchmark class under ProblemSolutions is a harness whose
// arms are supposed to be one LeetCode solution's own competing strategies - the class
// comment says so, and the number the benchmark publishes is only meaningful if it
// does. Nothing in the per-benchmark coverage proves that binding: comparing two arms
// to each other passes just as happily when both arms call the wrong method, or when
// one arm quietly stops being the strategy its name and comment claim.
//
// Each test below closes that gap by naming the LeetCode side directly: it builds the
// workload through the same public Fixtures builder the harness's [GlobalSetup] uses,
// calls the strategy the arm is named after, and asserts the harness arm returns
// exactly that answer. The workload builders are the seed-free ones, so the input is
// reproducible from the public [Params] value alone and no private constant has to be
// restated here.
public sealed partial class LeetCodeSeamTests
{
    private const int SmallestLength = 200;
    private const int SmallestCodeLength = 8;

    [Fact]
    public void RecursiveDescent_ExpressionBuiltFromTheWorkload_ReturnsWhatTheLeetCodeStrategyReturns()
    {
        var harness = BuildBasicCalculatorHarness();
        var expression = BasicCalculatorWorkloads.BuildExpression(SmallestLength);

        Assert.Equal(BasicCalculatorSolution.CalculateByRecursiveDescent(expression), harness.RecursiveDescent());
    }

    [Fact]
    public void StackScan_ExpressionBuiltFromTheWorkload_ReturnsWhatTheLeetCodeStrategyReturns()
    {
        var harness = BuildBasicCalculatorHarness();
        var expression = BasicCalculatorWorkloads.BuildExpression(SmallestLength);

        Assert.Equal(BasicCalculatorSolution.CalculateByStackScan(expression), harness.StackScan());
    }

    [Fact]
    public void RecursiveDescent_EncodedTextBuiltFromTheWorkload_ReturnsWhatTheLeetCodeStrategyReturns()
    {
        var harness = BuildDecodeStringHarness();
        var encoded = DecodeStringWorkloads.BuildEncoded(SmallestLength);

        Assert.Equal(DecodeStringSolution.DecodeByRecursiveDescent(encoded), harness.RecursiveDescent());
    }

    [Fact]
    public void StackScan_EncodedTextBuiltFromTheWorkload_ReturnsWhatTheLeetCodeStrategyReturns()
    {
        var harness = BuildDecodeStringHarness();
        var encoded = DecodeStringWorkloads.BuildEncoded(SmallestLength);

        Assert.Equal(DecodeStringSolution.DecodeByStackScan(encoded), harness.StackScan());
    }

    // This arm takes the code length as an argument rather than reading it off a field,
    // so the length the harness hands the strategy is checked here too: an arm passing
    // a hardcoded length would answer a different question than the strategy it wraps.
    [Fact]
    public void HasAllCodesByCodeSubstringSearch_CoveringTextBuiltFromTheWorkload_ReturnsWhatTheLeetCodeStrategyReturns()
    {
        var harness = BuildAllBinaryCodesHarness();
        var text = BinaryCodeTextWorkloads.BuildCoveringText(SmallestCodeLength);

        Assert.Equal(
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesByCodeSubstringSearch(text, SmallestCodeLength),
            harness.HasAllCodesByCodeSubstringSearch());
    }

    [Fact]
    public void HasAllCodesBySlidingBitmask_CoveringTextBuiltFromTheWorkload_ReturnsWhatTheLeetCodeStrategyReturns()
    {
        var harness = BuildAllBinaryCodesHarness();
        var text = BinaryCodeTextWorkloads.BuildCoveringText(SmallestCodeLength);

        Assert.Equal(
            CheckIfAStringContainsAllBinaryCodesOfSizeKSolution.HasAllCodesBySlidingBitmask(text, SmallestCodeLength),
            harness.HasAllCodesBySlidingBitmask());
    }

    private static BasicCalculatorBenchmarks BuildBasicCalculatorHarness()
    {
        var harness = new BasicCalculatorBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static DecodeStringBenchmarks BuildDecodeStringHarness()
    {
        var harness = new DecodeStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks BuildAllBinaryCodesHarness()
    {
        var harness = new CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks { CodeLength = SmallestCodeLength };
        harness.Setup();

        return harness;
    }
}
