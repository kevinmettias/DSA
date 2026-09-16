using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GroupAnagramsBenchmarks (ARCHITECTURE 17.9): both arms are
// GroupAnagramsSolution's - the Dictionary grouping against the HashMap grouping - so a harness
// whose arms disagree is timing two different problems. Each arm reports the number of groups it
// built rather than the groups themselves, which narrows what agreement can witness to the group
// count; the count is decisive here anyway, because [GlobalSetup] fills the array with only "eat"
// and "tea", which are anagrams of each other, so every word belongs to exactly one group. The
// array is a pure function of Length, so the same Length must rebuild the same array and the same
// count.
public sealed partial class GroupAnagramsBenchmarksTests
{
    private const int SmallestLength = 200;

    // "eat" and "tea" are anagrams, so the whole array is a single group.
    private const int ExpectedGroupCount = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DictionaryGroup(), BuildHarness().DictionaryGroup());

    [Fact]
    public void DictionaryGroup_TwoAnagramWordsOnly_AgreesWithHashMapGroup()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGroupCount, harness.DictionaryGroup());
        Assert.Equal(harness.HashMapGroup(), harness.DictionaryGroup());
    }

    [Fact]
    public void HashMapGroup_TwoAnagramWordsOnly_AgreesWithDictionaryGroup()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGroupCount, harness.HashMapGroup());
        Assert.Equal(harness.DictionaryGroup(), harness.HashMapGroup());
    }

    private static GroupAnagramsBenchmarks BuildHarness()
    {
        var harness = new GroupAnagramsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
