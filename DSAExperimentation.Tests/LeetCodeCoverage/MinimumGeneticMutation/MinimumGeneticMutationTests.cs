using DSAExperimentation.LeetCode.MinimumGeneticMutation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumGeneticMutation;

// Harness only: the same DataStructures.Graph.Hamming graph WordLadder uses, narrowed to the
// 4-letter DNA alphabet, with both of MinimumGeneticMutationSolution's strategies
// pinned to LeetCode's examples.
public sealed class MinimumGeneticMutationTests
{
    public static TheoryData<string, string, string[], int> Examples =>
        new()
        {
            { "AACCGGTT", "AACCGGTA", ["AACCGGTA"], 1 },
            { "AACCGGTT", "AAACGGTA", ["AACCGGTA", "AACCGCTA", "AAACGGTA"], 2 },
            { "AACCGGTT", "AAACGGTA", ["AACCGGTA", "AACCGCTA"], -1 },
            { "AACCGGTT", "AACCGGTT", ["AACCGGTT"], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMutationByMutationQueue_LeetCodeExamples_ReturnsFewestMutations(
        string startGene, string endGene, string[] bank, int expected) =>
        Assert.Equal(expected, MinimumGeneticMutationSolution.MinMutationByMutationQueue(startGene, endGene, bank));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMutationByReduceGraph_LeetCodeExamples_ReturnsFewestMutations(
        string startGene, string endGene, string[] bank, int expected) =>
        Assert.Equal(expected, MinimumGeneticMutationSolution.MinMutationByReduceGraph(startGene, endGene, bank));
}
