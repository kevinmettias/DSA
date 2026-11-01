using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IsomorphicStringsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on the smallest workload, so this asserts only the verdict the fixture
// fixes independently of either arm. Setup's workload applies a random substitution cipher to the
// source, so the pair really is isomorphic: the verdict is decisively true, and neither arm can
// leave early on a mismatch.
public sealed partial class IsomorphicStringsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = true;

    [Fact]
    public void IsIsomorphicByDictionary_SubstitutionCipherPair_ReportsIsomorphic() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsIsomorphicByDictionary());

    private static IsomorphicStringsBenchmarks BuildHarness()
    {
        var harness = new IsomorphicStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
