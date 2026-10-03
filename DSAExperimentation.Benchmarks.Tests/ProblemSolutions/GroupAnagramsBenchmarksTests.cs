using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GroupAnagramsBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: how the
// words group, known from Setup's construction rather than from either arm. [GlobalSetup] fills the array with only
// "eat" and "tea", which are anagrams of each other, so every word belongs to one single group, and each arm
// returns the groups it built.
public sealed partial class GroupAnagramsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void DictionaryGroup_TwoAnagramWordsOnly_PutsEveryWordInOneGroup() =>
        AssertPutsEveryWordInOneGroup(BuildHarness().DictionaryGroup());

    [Fact]
    public void HashMapGroup_TwoAnagramWordsOnly_PutsEveryWordInOneGroup() =>
        AssertPutsEveryWordInOneGroup(BuildHarness().HashMapGroup());

    // "eat" and "tea" are anagrams, so the whole array is a single group.
    private static void AssertPutsEveryWordInOneGroup(List<List<string>> groups) =>
        Assert.Equal(SmallestLength, Assert.Single(groups).Count);

    private static GroupAnagramsBenchmarks BuildHarness()
    {
        var harness = new GroupAnagramsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
