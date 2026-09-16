using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed partial class HammingSearchTests
{
    [Fact]
    public void MutationDistance_ReachableTarget_ReturnsEdgesWalked()
    {
        var allowed = new Set<string>(["hot", "dot", "dog", "cog"]);
        var distance = HammingSearch.MutationDistance(
            new MutationStart("hit"), new MutationTarget("cog"), allowed, StandardAlphabets.LowercaseLatin);

        Assert.Equal(4, distance);
    }

    [Fact]
    public void MutationDistance_StartEqualsTarget_ReturnsZero()
    {
        var allowed = new Set<string>(["aaa"]);
        var distance = HammingSearch.MutationDistance(
            new MutationStart("aaa"), new MutationTarget("aaa"), allowed, StandardAlphabets.LowercaseLatin);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void MutationDistance_StartNeedNotBeAMemberOfTheAllowedSet()
    {
        // LeetCode's beginWord/startGene convention: the start is walked from even
        // when the dictionary omits it.
        var allowed = new Set<string>(["ab"]);
        var distance = HammingSearch.MutationDistance(
            new MutationStart("aa"), new MutationTarget("ab"), allowed, StandardAlphabets.LowercaseLatin);

        Assert.Equal(1, distance);
    }

    [Fact]
    public void MutationDistance_NoPathThroughTheAllowedSet_ReturnsNull()
    {
        var allowed = new Set<string>(["hot", "dot", "dog"]);
        var distance = HammingSearch.MutationDistance(
            new MutationStart("hit"), new MutationTarget("cog"), allowed, StandardAlphabets.LowercaseLatin);

        Assert.Null(distance);
    }

    [Fact]
    public void MutationDistance_RespectsTheAlphabetItIsGiven()
    {
        // 'z' is unreachable over the DNA alphabet however permissive the set is.
        var allowed = new Set<string>(["z"]);
        var distance = HammingSearch.MutationDistance(
            new MutationStart("A"), new MutationTarget("z"), allowed, StandardAlphabets.Dna);

        Assert.Null(distance);
    }

    [Fact]
    public void MutationDistance_TakesTheShortestRouteWhenSeveralExist()
    {
        var allowed = new Set<string>(["ab", "bb", "bc", "ac"]);
        var distance = HammingSearch.MutationDistance(
            new MutationStart("aa"), new MutationTarget("bc"), allowed, StandardAlphabets.LowercaseLatin);

        Assert.Equal(2, distance);
    }
}
