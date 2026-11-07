using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

public sealed partial class NoTranspositionsTests
{
    [Fact]
    public void Remember_ThenTryRecall_FindsNothing()
    {
        var table = default(NoTranspositions<int, int>);
        table.Remember(0, new TableEntry<int>(1, 1, ScoreBound.Exact, 0));

        Assert.False(table.TryRecall(0, out _));
    }
}
