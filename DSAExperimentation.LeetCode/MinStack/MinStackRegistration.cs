using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.MinStack;

// Shape under test: a DESIGN problem - a stateful object driven by a sequence of
// calls, not a function from arguments to an answer. This is the shape the
// existing catalog explicitly gave up on: LeetCodeMetaDataDto's own doc comment
// records that ILeetCodeTestCaseAdapter's single Func<TInput, TOutput> "doesn't
// fit" a call sequence, so the mapper leaves ExampleTestCases empty for these.
//
// It fits here because TInput is free. The input is the script, the output is the
// per-call results with null for the calls that return nothing - exactly
// LeetCode's own [null,null,0,...] shape - and replaying the script is ordinary
// typed code inside the registration.
internal sealed class MinStackRegistration : ILeetCodeProblemRegistration
{
    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<IReadOnlyList<LeetCodeOperation>, List<int?>>("min-stack")
            .Strategy("StackPrimitive", Replay)
            .MatchingAnswersWith(LeetCodeAnswers.SequenceEqual)
            .Case(
                "example-1",
                [
                    LeetCodeOperation.Of("push", -2),
                    LeetCodeOperation.Of("push", 0),
                    LeetCodeOperation.Of("push", -3),
                    LeetCodeOperation.Of("getMin"),
                    LeetCodeOperation.Of("pop"),
                    LeetCodeOperation.Of("top"),
                    LeetCodeOperation.Of("getMin"),
                ],
                [null, null, null, -3, null, 0, -2])
            .Case(
                "minimum-restored-after-popping-it",
                [
                    LeetCodeOperation.Of("push", 5),
                    LeetCodeOperation.Of("push", 1),
                    LeetCodeOperation.Of("getMin"),
                    LeetCodeOperation.Of("pop"),
                    LeetCodeOperation.Of("getMin"),
                ],
                [null, null, 1, null, 5])
            .Build();

    private static List<int?> Replay(IReadOnlyList<LeetCodeOperation> script)
    {
        var stack = MinStackSolution.CreateByStackPrimitive();
        var results = new List<int?>(script.Count);

        foreach (var operation in script)
        {
            results.Add(Apply(stack, operation));
        }

        return results;
    }

    private static int? Apply(MinStackSolution.MinStackOperations stack, LeetCodeOperation operation)
    {
        switch (operation.Name)
        {
            case "push":
                stack.Push(operation.Arguments[0]);
                return null;
            case "pop":
                stack.Pop();
                return null;
            case "top":
                return stack.Top();
            case "getMin":
                return stack.GetMin();
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(operation), operation.Name, "Min Stack has no such operation.");
        }
    }
}
