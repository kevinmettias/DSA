using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Dataset;

public sealed partial class NestingScannerTests
{
    [Fact]
    public void IsAtTopLevel_AfterEachCharacter_TracksBracketsAndStrings()
    {
        var scanner = new NestingScanner();
        var levels = new List<bool>();

        foreach (var character in "a[1,'x]'],b")
        {
            scanner.Advance(character);
            levels.Add(scanner.IsAtTopLevel);
        }

        Assert.Equal([true, false, false, false, false, false, false, false, true, true, true], levels);
    }

    [Fact]
    public void Advance_EscapedQuote_KeepsTheStringOpen()
    {
        var scanner = new NestingScanner();

        foreach (var character in "'a\\'b")
        {
            scanner.Advance(character);
        }

        Assert.False(scanner.IsAtTopLevel);
    }
}
