using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllPeopleWithSecretBenchmarks (ARCHITECTURE 17.9): the generic arms check
// only compares the arms with each other, so this holds the baseline's answer to what Setup's
// construction fixes independently of any arm. Setup builds one long chain hanging off the informed
// pair and one long chain from a person nothing else reaches, so the secret reaches the first and
// none of the second.
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
    public void RepeatedRelaxation_TwoSameTimestampChains_InformsOnlyTheReachableChain()
    {
        var knowers = BuildHarness().RepeatedRelaxation();

        Assert.Contains(SecretOrigin, knowers);
        Assert.Contains(FirstPerson, knowers);
        Assert.DoesNotContain(UnreachableChainTail, knowers);
    }

    private static FindAllPeopleWithSecretBenchmarks BuildHarness()
    {
        var harness = new FindAllPeopleWithSecretBenchmarks { ChainLength = SmallestChainLength };
        harness.Setup();

        return harness;
    }
}
