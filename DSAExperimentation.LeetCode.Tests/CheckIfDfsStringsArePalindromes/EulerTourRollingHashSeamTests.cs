using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.CheckIfDfsStringsArePalindromes;

namespace DSAExperimentation.LeetCode.Tests.CheckIfDfsStringsArePalindromes;

// The seam between CheckIfDfsStringsArePalindromesSolution's two arms, across
// DataStructures' parent-array tree and its rolling hash.
//
// GetPalindromeFlagsByBruteForce rebuilds dfs(i) per node and scans it with two
// pointers. GetPalindromeFlagsByEulerTourRollingHash instead hands the parent array
// to ParentArrayTree.Build, walks the tree it returns exactly once recording every
// node's [start, end) run inside that single global string, then answers each
// node's query with two RollingHash lookups - the forward hash of the run against
// the same run read off a hash of the reversed string at index nodeCount - end[i].
//
// That index arithmetic and the "a subtree's run is contiguous" fact are the whole
// contract between the two libraries: if ParentArrayTree.Build handed back children
// in any other order, or if the reversed-string offset were off by one, the hashes
// would compare runs that do not correspond and the flags would be quietly wrong.
// The brute-force arm does not share a line of that reasoning, which is what makes
// it a reference.
public sealed partial class EulerTourRollingHashSeamTests
{
    [Fact]
    public void PalindromeFlags_MixedSiblingOrder_MatchesRebuiltDfsStrings()
    {
        int[] parent = [-1, 0, 1, 0];

        Assert.Equal(new[] { false, false, true, true }, FlagsBothWays(parent, "abcd").Composed);
    }

    [Fact]
    public void PalindromeFlags_TwoChildRoot_MatchesRebuiltDfsStrings()
    {
        int[] parent = [-1, 0, 0];

        AssertSameFlags(parent, "aab");
    }

    [Fact]
    public void PalindromeFlags_DeepChain_MatchesRebuiltDfsStrings()
        => AssertSameFlags(ChainParents(6), "abaaba");

    // A chain is the deepest tree for its node count, so every subtree run ends where
    // its parent's begins - the boundary where a reversed-hash offset of one is
    // indistinguishable from correct at every node but the root.
    [Fact]
    public void PalindromeFlags_LongerDeepChain_MatchesRebuiltDfsStrings()
        => AssertSameFlags(ChainParents(12), "aabbaabbaabb");

    [Fact]
    public void PalindromeFlags_SingleNode_MatchesRebuiltDfsStrings()
        => AssertSameFlags([-1], "z");

    // The composed arm's node overload takes the tree ParentArrayTree.Build returns
    // directly, so the two overloads are the same walk reading the same structure -
    // and neither may drift from the arm that never builds one.
    [Fact]
    public void PalindromeFlags_PrebuiltNodeOverload_MatchesParentArrayOverload()
    {
        int[] parent = [-1, 0, 0, 2, 2, 1];

        Assert.Equal(
            CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByEulerTourRollingHash(parent, "abccba"),
            CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByEulerTourRollingHash(
                ParentArrayTree.Build(parent), "abccba"));
    }

    // The root's run is the whole tour, so the composed arm's flag for node 0 is the
    // seam read from the other side: this test rebuilds the tour from ParentArrayTree
    // and decides it with a pair of RollingHash instances itself, and the arm has to
    // reach the same verdict without being told where the run starts or ends.
    [Fact]
    public void RootFlag_IndependentlyBuiltTour_AgreesWithItsOwnRollingHashVerdict()
    {
        int[] parent = [-1, 0, 1, 1];
        const string NodeCharacters = "abba";

        var tour = BuildPostOrderTour(parent, NodeCharacters);
        var flags = CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByEulerTourRollingHash(
            parent, NodeCharacters);

        Assert.Equal(IsRunAPalindrome(tour, start: 0, length: tour.Length), flags[0]);
    }

    private static void AssertSameFlags(int[] parent, string nodeCharacters)
        => Assert.Equal(FlagsBothWays(parent, nodeCharacters).Reference, FlagsBothWays(parent, nodeCharacters).Composed);

    private static (bool[] Reference, bool[] Composed) FlagsBothWays(int[] parent, string nodeCharacters) => (
        CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByBruteForce(parent, nodeCharacters),
        CheckIfDfsStringsArePalindromesSolution.GetPalindromeFlagsByEulerTourRollingHash(parent, nodeCharacters));

    private static int[] ChainParents(int nodeCount)
    {
        var parent = new int[nodeCount];
        parent[0] = -1;

        for (var i = 1; i < nodeCount; i++)
        {
            parent[i] = i - 1;
        }

        return parent;
    }

    // The verdict the composed arm derives from a forward and a reversed RollingHash
    // over one run of the tour, restated here against a tour this test built itself.
    private static bool IsRunAPalindrome(char[] tour, int start, int length)
    {
        var reversed = new char[tour.Length];
        Array.Copy(tour, reversed, tour.Length);
        Array.Reverse(reversed);
        var offsetFromTheEnd = tour.Length - start - length;

        return new RollingHash(tour).Hash(start, length)
            == new RollingHash(reversed).Hash(offsetFromTheEnd, length);
    }

    private static char[] BuildPostOrderTour(int[] parent, string nodeCharacters)
    {
        var nodes = ParentArrayTree.Build(parent);
        var tour = new List<char>();

        AppendSubtree(nodes[0], nodeCharacters, tour);

        return [.. tour];
    }

    private static void AppendSubtree(RootedTreeNode node, string nodeCharacters, List<char> tour)
    {
        foreach (var child in node.Children)
        {
            AppendSubtree(child, nodeCharacters, tour);
        }

        tour.Add(nodeCharacters[node.Id]);
    }
}
