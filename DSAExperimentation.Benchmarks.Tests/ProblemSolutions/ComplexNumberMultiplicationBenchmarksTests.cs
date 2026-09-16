using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ComplexNumberMultiplicationBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - parsing each "a+bi" operand with string.Split against
// slicing it as a ReadOnlySpan - so a harness whose arms disagree is timing two different problems.
// Setup draws the operand pairs from one fixed seed, so the same Length must rebuild the same pairs;
// otherwise two published numbers were never comparable in the first place.
//
// The pairs are private and each arm reports only the product of the last of them, which is the whole
// observable of that workload: a product in the "a+bi" shape the arms format LeetCode's answer with,
// identical on a second, independently built harness. That is a proxy, not the answer - agreement
// here witnesses that both parsing strategies read the final pair the same way, and says nothing
// about the other 199 pairs the measured loop walks.
public sealed partial class ComplexNumberMultiplicationBenchmarksTests
{
    private const int SmallestLength = 200;
    private const string ComplexProductPattern = @"^-?\d+\+-?\d+i$";

    [Fact]
    public void Setup_SameLength_RebuildsTheSameOperandPairs()
    {
        Assert.Matches(ComplexProductPattern, BuildHarness().StringSplitParse());
        Assert.Equal(BuildHarness().SpanParse(), BuildHarness().SpanParse());
    }

    [Fact]
    public void StringSplitParse_SeededOperandPairs_AgreesWithSpanParse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SpanParse(), harness.StringSplitParse());
    }

    [Fact]
    public void SpanParse_SeededOperandPairs_AgreesWithStringSplitParse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StringSplitParse(), harness.SpanParse());
    }

    private static ComplexNumberMultiplicationBenchmarks BuildHarness()
    {
        var harness = new ComplexNumberMultiplicationBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
