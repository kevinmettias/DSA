using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllOneDataStructureBenchmarks (ARCHITECTURE 17.9): its two arms are
// AllOneDataStructureSolution's competing strategies for the same question - a dictionary scan against a bucketed
// linked list - so a harness whose arms disagree is replaying two different operation scripts. Both arms report a
// checksum of the key lengths GetMaxKey/GetMinKey hand back, which is what makes a wrong answer observable at all;
// a checksum of zero would mean the script never placed a key, so both arms assert it positively as well as
// against each other. Setup seeds Length distinct keys and then draws from them under one seed, so the same Length
// must rebuild the same script.
public sealed partial class AllOneDataStructureBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] script lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceDictionaryScan(), BuildHarness().BruteForceDictionaryScan());

    [Fact]
    public void BruteForceDictionaryScan_TwoHundredKeyScript_AgreesWithBucketedLinkedListOnePass()
    {
        var harness = BuildHarness();

        Assert.True(harness.BruteForceDictionaryScan() > 0);
        Assert.Equal(harness.BucketedLinkedListOnePass(), harness.BruteForceDictionaryScan());
    }

    [Fact]
    public void BucketedLinkedListOnePass_TwoHundredKeyScript_AgreesWithBruteForceDictionaryScan()
    {
        var harness = BuildHarness();

        Assert.True(harness.BucketedLinkedListOnePass() > 0);
        Assert.Equal(harness.BruteForceDictionaryScan(), harness.BucketedLinkedListOnePass());
    }

    private static AllOneDataStructureBenchmarks BuildHarness()
    {
        var harness = new AllOneDataStructureBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
