using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class TrieFailuresTests
{
    [Fact]
    public void ThrowNotAChildRank_NamesTheRank() =>
        Assert.Equal("rank", Assert.Throws<ArgumentOutOfRangeException>(() => TrieFailures.ThrowNotAChildRank(2)).ParamName);
}
