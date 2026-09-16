using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StreamOfCharactersBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same question - how many of LC 1032's streamed queries match a stored word - one by
// re-testing every suffix so far against a hash set, one by walking this repo's reversed
// LowercaseTrie, so a harness whose arms disagree is timing two different problems. Each arm
// constructs its own checker inside the call, so one harness instance is safe to call twice in
// either order; Setup generates the stream, so the same length must rebuild the same workload.
public sealed partial class StreamOfCharactersBenchmarksTests
{
    private const int SmallestStreamLength = 200;

    [Fact]
    public void Setup_SameStreamLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RescanEverySuffixAgainstHashSet()),
            AnswerText.Of(BuildHarness().RescanEverySuffixAgainstHashSet()));

    [Fact]
    public void RescanEverySuffixAgainstHashSet_AgreesWithReversedTrieBackwardWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.RescanEverySuffixAgainstHashSet(),
            harness.ReversedTrieBackwardWalk());
    }

    [Fact]
    public void ReversedTrieBackwardWalk_AgreesWithRescanEverySuffixAgainstHashSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.ReversedTrieBackwardWalk(),
            harness.RescanEverySuffixAgainstHashSet());
    }

    private static StreamOfCharactersBenchmarks BuildHarness()
    {
        var harness = new StreamOfCharactersBenchmarks { StreamLength = SmallestStreamLength };
        harness.Setup();

        return harness;
    }
}
