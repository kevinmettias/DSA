using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindSubstringWithGivenHashValueBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - recomputing each window's hash from its characters
// against reading it off this repo's precomputed RollingHash table - so a harness whose arms disagree
// has searched for two different hash values. Both answers are one bool, so they are compared
// directly.
//
// The agreement asserted here is WEAK BY CONSTRUCTION, and honestly so: Setup asks for an unreachable
// hash value (-1, outside [0, Modulo)) so that neither strategy may exit early, which means both arms
// return false on every run. Agreement therefore witnesses that both swept every window and rejected
// each one, not that they agreed on a found window - an arm that gave up after the first window would
// agree just as well. What it does catch is an arm that ever reports a match the other does not.
//
// The text is the same seeded draw for a given Length, so the same parameters must rebuild the same
// search space.
public sealed partial class FindSubstringWithGivenHashValueBenchmarksTests
{
    // The smaller of Setup's [Params(500, 20_000)] text lengths.
    private const int SmallestLength = 500;

    // Setup's target is outside [0, Modulo), so no window's hash can ever equal it.
    private const bool ExpectedMatch = false;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUnreachableSearch() =>
        Assert.Equal(
            BuildHarness().TryFindSubstringByWindowRehash(),
            BuildHarness().TryFindSubstringByWindowRehash());

    [Fact]
    public void TryFindSubstringByWindowRehash_UnreachableHashValue_AgreesWithTryFindSubstringByRollingHash()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatch, harness.TryFindSubstringByWindowRehash());
        Assert.Equal(
            harness.TryFindSubstringByRollingHash(),
            harness.TryFindSubstringByWindowRehash());
    }

    [Fact]
    public void TryFindSubstringByRollingHash_UnreachableHashValue_AgreesWithTryFindSubstringByWindowRehash()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatch, harness.TryFindSubstringByRollingHash());
        Assert.Equal(
            harness.TryFindSubstringByWindowRehash(),
            harness.TryFindSubstringByRollingHash());
    }

    private static FindSubstringWithGivenHashValueBenchmarks BuildHarness()
    {
        var harness = new FindSubstringWithGivenHashValueBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
