using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllPeopleWithSecretBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - repeated relaxation passes over each timestamp group
// against a keyed disjoint set built fresh per group - so a harness whose arms disagree is
// propagating two different secrets. Both arms return the informed people through the same
// ascending-id collector, so the returned order is the same positional answer on both sides and the
// arrays are compared as ordered sequences. Setup builds one long chain hanging off the informed
// pair and one long chain from a person nothing else reaches, so both arms must walk every hop of
// the first and none of the second; the same ChainLength must rebuild the same schedule.
public sealed partial class FindAllPeopleWithSecretBenchmarksTests
{
    private const int SmallestChainLength = 50;
    private const int SecretOrigin = 0;
    private const int FirstPerson = 1;

    // Setup's unreachable chain starts at person ChainLength + 2 and runs ChainLength people, so its
    // farthest person is 2 * ChainLength + 1. That component never touches the seed meeting, which
    // is what the workload exists to make the strategies walk past.
    private const int UnreachableChainTail = (2 * SmallestChainLength) + 1;

    [Fact]
    public void Setup_SameChainLength_RebuildsTheSameSchedule()
    {
        var knowers = BuildHarness().RepeatedRelaxation();

        Assert.Contains(SecretOrigin, knowers);
        Assert.Contains(FirstPerson, knowers);
        Assert.DoesNotContain(UnreachableChainTail, knowers);

        Assert.Equal(
            AnswerText.Of(BuildHarness().RepeatedRelaxation()),
            AnswerText.Of(BuildHarness().RepeatedRelaxation()));
    }

    [Fact]
    public void RepeatedRelaxation_TwoSameTimestampChains_AgreesWithKeyedDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RootMergedWithKeyedDisjointSet()),
            AnswerText.Of(harness.RepeatedRelaxation()));
    }

    [Fact]
    public void RootMergedWithKeyedDisjointSet_TwoSameTimestampChains_AgreesWithRepeatedRelaxation()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RepeatedRelaxation()),
            AnswerText.Of(harness.RootMergedWithKeyedDisjointSet()));
    }

    private static FindAllPeopleWithSecretBenchmarks BuildHarness()
    {
        var harness = new FindAllPeopleWithSecretBenchmarks { ChainLength = SmallestChainLength };
        harness.Setup();

        return harness;
    }
}
