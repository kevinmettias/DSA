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
    // The four operation names the script and the replay must agree on. They are
    // named once here because the switch in `Apply` below is the only place that
    // decides what a name means, and a `Case` that spelled one differently from
    // the switch would be rejected at run time as an unknown operation rather
    // than at compile time as a typo.
    private const string Push = "push";
    private const string Pop = "pop";
    private const string Top = "top";
    private const string GetMin = "getMin";

    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<IReadOnlyList<LeetCodeOperation>, List<int?>>("min-stack")
            .Strategy("StackPrimitive", Replay)
            .MatchingAnswersWith(LeetCodeAnswers.IsSequenceEqual)
            .Case(
                "example-1",
                [
                    LeetCodeOperation.Of(Push, -2),
                    LeetCodeOperation.Of(Push, 0),
                    LeetCodeOperation.Of(Push, -3),
                    LeetCodeOperation.Of(GetMin),
                    LeetCodeOperation.Of(Pop),
                    LeetCodeOperation.Of(Top),
                    LeetCodeOperation.Of(GetMin),
                ],
                [null, null, null, -3, null, 0, -2])
            .Case(
                "minimum-restored-after-popping-it",
                [
                    LeetCodeOperation.Of(Push, 5),
                    LeetCodeOperation.Of(Push, 1),
                    LeetCodeOperation.Of(GetMin),
                    LeetCodeOperation.Of(Pop),
                    LeetCodeOperation.Of(GetMin),
                ],
                [null, null, 1, null, 5])

            // The classic Min Stack bug, and the reason this script is worth
            // keeping separate from the one above: when the minimum is pushed
            // TWICE, popping one copy must leave it the minimum. An implementation
            // that stores each distinct minimum once - rather than once per push -
            // reports 3 and then loses 5 entirely on the second pop.
            .Case(
                "duplicate-minimum-survives-one-pop",
                [
                    LeetCodeOperation.Of(Push, 5),
                    LeetCodeOperation.Of(Push, 5),
                    LeetCodeOperation.Of(GetMin),
                    LeetCodeOperation.Of(Pop),
                    LeetCodeOperation.Of(GetMin),
                    LeetCodeOperation.Of(Push, 3),
                    LeetCodeOperation.Of(GetMin),
                    LeetCodeOperation.Of(Pop),
                    LeetCodeOperation.Of(GetMin),
                ],
                [null, null, 5, null, 5, null, 3, null, 5])
            .Build();

    private static List<int?> Replay(IReadOnlyList<LeetCodeOperation> script)
    {
        var stack = MinStackSolution.CreateByStackPrimitive();
        var results = new List<int?>(script.Count);

        foreach (var operation in script)
        {
            var result = Apply(stack, operation);
            results.Add(result);
        }

        return results;
    }

    private static int? Apply(MinStackSolution.MinStackOperations stack, LeetCodeOperation operation)
    {
        switch (operation.Name)
        {
            case Push:
                stack.Push(operation.Arguments[0]);
                return null;
            case Pop:
                stack.Pop();
                return null;
            case Top:
                return stack.Top();
            case GetMin:
                return stack.GetMin();
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(operation), operation.Name, "Min Stack has no such operation.");
        }
    }
}
