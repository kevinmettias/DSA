namespace DSAExperimentation.LeetCode.Catalog.Questions;

// A pointer to another question, not the question itself: LeetCode lists similar questions the
// repo may not cover, so this carries what LeetCode says about the pointer and nothing a load of
// the other question would have to supply.
/// <summary>A question LeetCode lists as similar to another one.</summary>
/// <param name="Title">The similar question's title.</param>
/// <param name="TitleSlug">The similar question's URL slug.</param>
/// <param name="Difficulty">The similar question's difficulty.</param>
public sealed record LeetCodeRelatedQuestion(string Title, string TitleSlug, LeetCodeDifficulty Difficulty);
