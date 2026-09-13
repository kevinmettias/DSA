using DSAExperimentation.LeetCode.KthSymbolInGrammar;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSymbolInGrammar;

// Harness only: both strategies live in KthSymbolInGrammarSolution and are asserted
// against the same examples, including every symbol of rows 2 and 3 so a flipped half
// cannot pass, and the naive row-expansion baseline that was previously only ever run
// by the benchmark.
public sealed class KthSymbolInGrammarTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 1, 1, 0 },
            { 2, 1, 0 },
            { 2, 2, 1 },
            { 3, 1, 0 },
            { 3, 2, 1 },
            { 3, 3, 1 },
            { 3, 4, 0 },
            { 4, 5, 1 },
            { 4, 1, 0 },
            { 4, 8, 1 },
            { 5, 16, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthGrammarByRowExpansion_LeetCodeExamples_ReturnsExpectedSymbol(int n, int k, int expected) =>
        Assert.Equal(expected, KthSymbolInGrammarSolution.KthGrammarByRowExpansion(n, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthGrammarByRecursiveHalving_LeetCodeExamples_ReturnsExpectedSymbol(int n, int k, int expected) =>
        Assert.Equal(expected, KthSymbolInGrammarSolution.KthGrammarByRecursiveHalving(n, k));
}
