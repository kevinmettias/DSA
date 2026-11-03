using DSAExperimentation.LeetCode.Solutions.NumberOfSubarraysThatMatchAPatternII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfSubarraysThatMatchAPatternII;

// Harness only. Both strategies live in NumberOfSubarraysThatMatchAPatternIISolution, and
// CatalogCaseTests holds them to LeetCode's published examples (ARCHITECTURE 17.13). This
// file pins the alphabet the matching strategy encodes signs and pattern entries into.
public sealed partial class NumberOfSubarraysThatMatchAPatternIISolutionTests
{
    // Down, level, up between consecutive elements become '0', '1', '2'.
    [Fact]
    public void EncodeDiffs_ConsecutiveSigns_BecomeOneSymbolEach() =>
        Assert.Equal("2102", NumberOfSubarraysThatMatchAPatternIISolution.EncodeDiffs([1, 4, 4, 1, 3]));

    // A pattern entry -1, 0 or 1 lands on the same symbol as the sign it stands for.
    [Fact]
    public void EncodePattern_Entries_UseTheSameAlphabetAsTheSigns() =>
        Assert.Equal("210", NumberOfSubarraysThatMatchAPatternIISolution.EncodePattern([1, 0, -1]));
}
