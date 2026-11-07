namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A solution-shaped class for CatalogStrategyTests, outside the solutions project and never enrolled: an
// in-place question's strategy written as a Try method, which sorts the array it is handed and returns whether
// it succeeded - always false here, so a harness that answered with the result rather than the array would be
// seen to. Named for Sort Colors so its cases come from that question.
[LeetCodeQuestion("sort-colors", LeetCodeAnswerOrder.AsWritten, AnswersInPlace = true)]
internal static class TryInPlaceProbeSolution
{
    public static bool TrySortByArraySort(int[] values)
    {
        Array.Sort(values);

        return false;
    }
}
