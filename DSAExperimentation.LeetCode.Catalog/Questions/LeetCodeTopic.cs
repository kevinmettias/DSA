namespace DSAExperimentation.LeetCode.Catalog.Questions;

/// <summary>One of the topic tags LeetCode files a question under.</summary>
/// <param name="Name">The tag as LeetCode displays it, such as "Hash Table".</param>
/// <param name="Slug">The tag as LeetCode's problem list filters by it, such as "hash-table".</param>
public sealed record LeetCodeTopic(string Name, string Slug);
