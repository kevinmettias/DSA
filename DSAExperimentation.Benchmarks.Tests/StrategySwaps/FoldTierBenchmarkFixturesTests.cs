using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Each tier's entry point, handed the same small tree, answers that tree's size: the property the
// two fold-tier benchmarks rely on to time three tiers of one question.
public sealed partial class FoldTierBenchmarkFixturesTests
{
    private const int SmallTreeSize = 7;

    [Fact]
    public void TreeTier_SmallTree_AnswersItsSize() =>
        Assert.Equal(SmallTreeSize, FoldTierBenchmarkFixtures.TreeTier(BinaryTrees.Balanced(SmallTreeSize)));

    [Fact]
    public void DagTier_SmallTree_AnswersItsSize() =>
        Assert.Equal(SmallTreeSize, FoldTierBenchmarkFixtures.DagTier(BinaryTrees.Balanced(SmallTreeSize)));

    [Fact]
    public void GraphTier_SmallTree_AnswersItsSize() =>
        Assert.Equal(SmallTreeSize, FoldTierBenchmarkFixtures.GraphTier(BinaryTrees.Balanced(SmallTreeSize)));
}
