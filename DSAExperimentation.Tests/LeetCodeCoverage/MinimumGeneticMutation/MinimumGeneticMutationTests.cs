using DSAExperimentation.LeetCode.MinimumGeneticMutation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumGeneticMutation;

// Harness only: the same DataStructures.Graph.Hamming graph WordLadder uses, narrowed to the
// 4-letter DNA alphabet, with both of MinimumGeneticMutationSolution's strategies
// pinned to LeetCode's examples.
public sealed partial class MinimumGeneticMutationTests
{
    public static TheoryData<MutationExample> Examples =>
        new()
        {
            { new MutationExample(StartGene: "AACCGGTT", EndGene: "AACCGGTA", Bank: ["AACCGGTA"], Expected: 1) },
            {
                new MutationExample(
                    StartGene: "AACCGGTT",
                    EndGene: "AAACGGTA",
                    Bank: ["AACCGGTA", "AACCGCTA", "AAACGGTA"],
                    Expected: 2)
            },
            {
                new MutationExample(
                    StartGene: "AACCGGTT",
                    EndGene: "AAACGGTA",
                    Bank: ["AACCGGTA", "AACCGCTA"],
                    Expected: -1)
            },
            { new MutationExample(StartGene: "AACCGGTT", EndGene: "AACCGGTT", Bank: ["AACCGGTT"], Expected: 0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMutationByMutationQueue_LeetCodeExamples_ReturnsFewestMutations(MutationExample example)
    {
        var actual = MinimumGeneticMutationSolution.MinMutationByMutationQueue(
            new MinimumGeneticMutationSolution.StartGene(example.StartGene),
            new MinimumGeneticMutationSolution.EndGene(example.EndGene),
            example.Bank);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMutationByReduceGraph_LeetCodeExamples_ReturnsFewestMutations(MutationExample example)
    {
        var actual = MinimumGeneticMutationSolution.MinMutationByReduceGraph(
            new MinimumGeneticMutationSolution.StartGene(example.StartGene),
            new MinimumGeneticMutationSolution.EndGene(example.EndGene),
            example.Bank);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the gene the sequence starts from, the gene it has to end
    // at, the bank of valid intermediate genes, and the fewest mutations that get
    // there. StartGene and EndGene are both `string` and the question is not
    // symmetric, so the row names the roles instead of leaving two adjacent positions
    // a caller could swap with the compiler none the wiser.
    public readonly record struct MutationExample(
        string StartGene,
        string EndGene,
        string[] Bank,
        int Expected);
}
