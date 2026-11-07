using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

public sealed partial class TranspositionTableTests
{
    private const int Position = 7;

    private static readonly TableEntry<int> Older = new(2, 1, ScoreBound.Exact, 0);
    private static readonly TableEntry<int> Newer = new(3, -4, ScoreBound.AtMost, 2);

    [Fact]
    public void TryRecall_APositionNeverSearched_FindsNothing() =>
        Assert.False(new TranspositionTable<int, int>().TryRecall(Position, out _));

    [Fact]
    public void Remember_ThenTryRecall_FindsTheEntry()
    {
        var table = new TranspositionTable<int, int>();
        table.Remember(Position, Older);

        Assert.Equal((true, Older), (table.TryRecall(Position, out var entry), entry));
    }

    [Fact]
    public void Remember_APositionAgain_KeepsTheLatest()
    {
        var table = new TranspositionTable<int, int>();
        table.Remember(Position, Older);
        table.Remember(Position, Newer);

        Assert.Equal((true, Newer), (table.TryRecall(Position, out var entry), entry));
    }
}
