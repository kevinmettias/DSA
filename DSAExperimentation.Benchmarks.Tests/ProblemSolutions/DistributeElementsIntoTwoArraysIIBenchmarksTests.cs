using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeElementsIntoTwoArraysIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - counting "strictly greater than" by a linear scan
// of each growing list against a Fenwick tree per list over the same coordinate-compressed ranks -
// so a harness whose arms disagree is timing two different problems. AnswerText.Of, not
// OfUnorderedSet: LC 3072's answer is the concatenation of arr1 and arr2 in that order, so the
// sequence itself is what both arms must reproduce. Setup draws Length values from one fixed seed,
// and every element lands in exactly one of the two lists, so the concatenation is always Length
// long.
public sealed partial class DistributeElementsIntoTwoArraysIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SeededValueArray_PlacesEveryElementAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var distributed = harness.BruteForce();

        Assert.Equal(SmallestLength, distributed.Length);
        Assert.Equal(AnswerText.Of(distributed), AnswerText.Of(BuildHarness().BruteForce()));
    }

    [Fact]
    public void BruteForce_SeededValueArray_AgreesWithFenwickTree()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.FenwickTree()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void FenwickTree_SeededValueArray_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.FenwickTree()));
    }

    private static DistributeElementsIntoTwoArraysIIBenchmarks BuildHarness()
    {
        var harness = new DistributeElementsIntoTwoArraysIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
