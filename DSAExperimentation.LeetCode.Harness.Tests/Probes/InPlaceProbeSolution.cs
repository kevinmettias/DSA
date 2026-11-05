namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A solution-shaped class for CatalogStrategyTests, outside the solutions project and never enrolled: an
// in-place question's strategy, which sorts the array it is handed and returns nothing, named for Sort
// Colors so its cases come from that question.
[LeetCodeQuestion("sort-colors", LeetCodeAnswerOrder.AsWritten, AnswersInPlace = true)]
internal static class InPlaceProbeSolution
{
    public static void SortByArraySort(int[] values) => Array.Sort(values);
}
