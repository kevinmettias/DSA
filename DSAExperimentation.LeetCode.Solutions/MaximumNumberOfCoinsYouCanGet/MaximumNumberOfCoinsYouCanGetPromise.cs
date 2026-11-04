namespace DSAExperimentation.LeetCode.Solutions.MaximumNumberOfCoinsYouCanGet;

// LC 1561's promise about its input: the piles come in rounds of three. LeetCode's judge never
// poses an input that breaks it. A pile left over has no round, and the round simulation reads past
// the end for it. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class MaximumNumberOfCoinsYouCanGetPromise
{
    public static bool IsKeptBy(int[] piles) => piles.Length % MaximumNumberOfCoinsYouCanGetSolution.GroupSize == 0;
}
