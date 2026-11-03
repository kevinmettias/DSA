namespace DSAExperimentation.LeetCode.Catalog.Questions;

/// <summary>One input of a LeetCode signature, named and typed in LeetCode's own vocabulary.</summary>
/// <param name="Name">The parameter's name, which is also the name a question's constraints use for it.</param>
/// <param name="Type">LeetCode's type name, such as "integer[]", "ListNode" or "list&lt;string&gt;".</param>
public sealed record LeetCodeParameter(string Name, string Type);
