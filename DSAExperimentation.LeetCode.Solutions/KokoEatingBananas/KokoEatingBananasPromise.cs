namespace DSAExperimentation.LeetCode.Solutions.KokoEatingBananas;

// LC 875's promise about its input: there are at least as many hours as piles, so some speed
// finishes in time. LeetCode's judge never poses an input that breaks it. With fewer hours than
// piles no speed finishes, and the two bisections stop at different speeds. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class KokoEatingBananasPromise
{
    public static bool IsKeptBy(int[] piles, int hourBudget) => hourBudget >= piles.Length;
}
