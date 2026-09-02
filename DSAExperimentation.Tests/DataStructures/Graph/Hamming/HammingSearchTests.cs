using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed class HammingSearchTests
{
    [Fact]
    public void MutationDistance_ReachableTarget_ReturnsEdgesWalked()
    {
        var allowed = new Set<string>(["hot", "dot", "dog", "cog"]);

        Assert.Equal(4, HammingSearch.MutationDistance("hit", "cog", allowed, StandardAlphabets.LowercaseLatin));
    }

    [Fact]
    public void MutationDistance_StartEqualsTarget_ReturnsZero()
    {
        var allowed = new Set<string>(["aaa"]);

        Assert.Equal(0, HammingSearch.MutationDistance("aaa", "aaa", allowed, StandardAlphabets.LowercaseLatin));
    }

    [Fact]
    public void MutationDistance_StartNeedNotBeAMemberOfTheAllowedSet()
    {
        // LeetCode's beginWord/startGene convention: the start is walked from even
        // when the dictionary omits it.
        var allowed = new Set<string>(["ab"]);

        Assert.Equal(1, HammingSearch.MutationDistance("aa", "ab", allowed, StandardAlphabets.LowercaseLatin));
    }

    [Fact]
    public void MutationDistance_NoPathThroughTheAllowedSet_ReturnsNull()
    {
        var allowed = new Set<string>(["hot", "dot", "dog"]);

        Assert.Null(HammingSearch.MutationDistance("hit", "cog", allowed, StandardAlphabets.LowercaseLatin));
    }

    [Fact]
    public void MutationDistance_RespectsTheAlphabetItIsGiven()
    {
        // 'z' is unreachable over the DNA alphabet however permissive the set is.
        var allowed = new Set<string>(["z"]);

        Assert.Null(HammingSearch.MutationDistance("A", "z", allowed, StandardAlphabets.Dna));
    }

    [Fact]
    public void MutationDistance_TakesTheShortestRouteWhenSeveralExist()
    {
        var allowed = new Set<string>(["ab", "bb", "bc", "ac"]);

        Assert.Equal(2, HammingSearch.MutationDistance("aa", "bc", allowed, StandardAlphabets.LowercaseLatin));
    }
}
