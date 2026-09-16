using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IsomorphicStringsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - is this pair isomorphic - so a harness whose arms
// disagree is timing two different problems. Setup's workload applies a random substitution
// cipher to the source, so the pair really is isomorphic: the verdict the two arms have to
// agree on is decisively true, and neither can leave early on a mismatch.
public sealed partial class IsomorphicStringsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheIsomorphicPair()
    {
        Assert.Equal(ExpectedVerdict, BuildHarness().IsIsomorphicByDictionary());
        Assert.Equal(ExpectedVerdict, BuildHarness().IsIsomorphicByHashMap());
    }

    [Fact]
    public void IsIsomorphicByDictionary_SubstitutionCipherPair_AgreesWithHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsIsomorphicByHashMap(), harness.IsIsomorphicByDictionary());
    }

    [Fact]
    public void IsIsomorphicByHashMap_SubstitutionCipherPair_AgreesWithDictionary()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsIsomorphicByDictionary(), harness.IsIsomorphicByHashMap());
    }

    private static IsomorphicStringsBenchmarks BuildHarness()
    {
        var harness = new IsomorphicStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
