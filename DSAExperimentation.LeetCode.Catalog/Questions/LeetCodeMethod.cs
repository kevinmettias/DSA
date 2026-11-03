namespace DSAExperimentation.LeetCode.Catalog.Questions;

/// <summary>One method of the class a design question asks for.</summary>
/// <param name="Name">The method's name as LeetCode's call script spells it.</param>
/// <param name="Parameters">The method's inputs, in order.</param>
/// <param name="ReturnType">LeetCode's type name for what the method returns, "void" when nothing.</param>
public sealed record LeetCodeMethod(string Name, IReadOnlyList<LeetCodeParameter> Parameters, string ReturnType);
