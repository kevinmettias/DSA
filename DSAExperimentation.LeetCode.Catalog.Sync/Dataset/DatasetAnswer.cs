namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// A dataset case's expected answer once it has been judged usable. Notation is null for a question
// that answers by changing its input, whose changed value the dataset never recorded - a kept
// case with no answer, which is why "no answer" and "dropped" need different shapes.
internal readonly record struct DatasetAnswer(string? Notation);
