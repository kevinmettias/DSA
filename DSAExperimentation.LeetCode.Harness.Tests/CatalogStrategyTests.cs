using DSAExperimentation.LeetCode.Harness.Tests.Probes;
using DSAExperimentation.LeetCode.Solutions.TwoSum;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogStrategyTests
{
    [Fact]
    public void SolutionsAssembly_ByName_IsTheAssemblyDeclaringTheSolutions() =>
        Assert.Same(typeof(TwoSumSolution).Assembly, CatalogStrategy.SolutionsAssembly);

    [Fact]
    public void EnrolledSolutions_AfterDiscovery_IncludeEverySolutionThatNamesItsQuestion() =>
        Assert.Contains(typeof(TwoSumSolution), CatalogStrategy.EnrolledSolutions());

    [Fact]
    public void Of_EnrolledSolution_FindsItsStrategiesInDeclarationOrder() =>
        Assert.Equal(
            [nameof(TwoSumSolution.TryFindIndicesByBruteForce), nameof(TwoSumSolution.TryFindIndicesByHashMap)],
            CatalogStrategy.Of(typeof(TwoSumSolution)).Select(strategy => strategy.Name));

    [Fact]
    public void Of_StrategyNoCaseCanFeed_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => CatalogStrategy.Of(typeof(UnreadableProbeSolution)));

    [Fact]
    public void Of_MethodNamingNoStrategy_LeavesItOut() =>
        Assert.DoesNotContain(nameof(CountingProbeSolution.Halve), CatalogStrategy.Of(typeof(CountingProbeSolution)).Select(strategy => strategy.Name));

    [Fact]
    public void Of_StrategyCallingALocalFunction_FindsTheStrategyAlone()
    {
        var names = CatalogStrategy.Of(typeof(LocalFunctionProbeSolution)).Select(strategy => strategy.Name);

        Assert.Equal([nameof(LocalFunctionProbeSolution.CountByHelper)], names);
    }

    [Fact]
    public void AnswerTo_StrategyAnsweringInPlace_AnswersWithItsInputAsTheCallLeftIt()
    {
        var strategy = CatalogStrategy.Of(typeof(InPlaceProbeSolution)).Single();
        var columns = strategy.Columns();

        var answer = strategy.AnswerTo(columns, 0);

        Assert.Equal(columns.Cases[0].ExpectedOutput, answer.ToJsonString());
    }

    [Fact]
    public void Solution_AnyStrategy_IsTheClassThatDeclaresIt() =>
        Assert.All(CatalogStrategy.Of(typeof(TwoSumSolution)), strategy => Assert.Equal(typeof(TwoSumSolution), strategy.Solution));

    [Fact]
    public void Name_AnyStrategy_IsItsMethodsName() =>
        Assert.Equal(nameof(CountingProbeSolution.CountByLoop), CatalogStrategy.Of(typeof(CountingProbeSolution)).Single().Name);

    [Fact]
    public void Columns_EnrolledStrategy_ReadsTheCatalogsCasesIntoItsInputs() =>
        Assert.NotEmpty(CatalogStrategy.Of(typeof(TwoSumSolution))[0].Columns().Column<int[]>(0));

    // LC 1's first example: nums [2,7,11,15], target 9, answered by indices 0 and 1.
    [Fact]
    public void AnswerTo_TryStrategy_AnswersTheOutputsItFound()
    {
        var strategy = CatalogStrategy.Of(typeof(TwoSumSolution))[0];
        var columns = strategy.Columns();
        var first = Enumerable.Range(0, columns.Count).First(index => columns.Cases[index].Inputs[0] == "[2,7,11,15]");

        Assert.Equal("[0,1]", strategy.AnswerTo(columns, first).ToJsonString());
    }
}
