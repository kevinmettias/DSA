using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BasicCalculatorIVBenchmarks (ARCHITECTURE 17.9): its two arms are BasicCalculatorIVSolution's
// competing strategies for the same question - a BCL Dictionary plus List.Sort against this repo's own HashMap plus
// MergeSort - so a harness whose arms disagree has evaluated two different polynomials. Both return LC 770's sorted
// term list, and the expression is a sum of Length distinct variables with no evalvars, so every term survives and
// the answer must hold exactly one term per variable: a decisive count asserted alongside the arms' agreement.
// Setup builds that expression from the length alone, so the same Length must rebuild the same text.
public sealed partial class BasicCalculatorIVBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] variable counts.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().DictionaryPolynomial()),
            AnswerGraphText.Of(BuildHarness().DictionaryPolynomial()));

    [Fact]
    public void DictionaryPolynomial_TwoHundredVariableSum_AgreesWithHashMapMergeSort()
    {
        var harness = BuildHarness();
        var terms = harness.DictionaryPolynomial();

        Assert.Equal(SmallestLength, terms.Count);
        Assert.Equal(AnswerGraphText.Of(harness.HashMapMergeSort()), AnswerGraphText.Of(terms));
    }

    [Fact]
    public void HashMapMergeSort_TwoHundredVariableSum_AgreesWithDictionaryPolynomial()
    {
        var harness = BuildHarness();
        var terms = harness.HashMapMergeSort();

        Assert.Equal(SmallestLength, terms.Count);
        Assert.Equal(AnswerGraphText.Of(harness.DictionaryPolynomial()), AnswerGraphText.Of(terms));
    }

    private static BasicCalculatorIVBenchmarks BuildHarness()
    {
        var harness = new BasicCalculatorIVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
