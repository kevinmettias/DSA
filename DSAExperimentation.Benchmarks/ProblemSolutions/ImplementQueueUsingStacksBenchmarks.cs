using DSAExperimentation.LeetCode.ImplementQueueUsingStacks;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementQueueUsingStacksSolution's two-stack queue, on
// the BCL's Stack<int> and on this repo's, the same classes
// ImplementQueueUsingStacksSolutionTests proves correct. [GlobalSetup] draws one fixed
// call sequence of interleaved push/pop/peek calls (never popping/peeking past what has
// actually been pushed) and writes it out once per queue type, so script construction is
// not charged to the measured replay and each arm's entries call its own queue's methods
// directly rather than through an interface. Each arm returns every value pop and peek
// answered, in order - the same "return the real answer, not a weaker proxy" shape
// LRUCacheBenchmarks/BinarySearchTreeIteratorBenchmarks already follow. A push answers
// nothing, which its script entry reports as null. OperationCount stops at LC 232's
// 100-call cap, and pushed values are drawn from its [1, 9].
public class ImplementQueueUsingStacksBenchmarks
{
    private const int Seed = 232;
    private const int MinPushedValue = 1;
    private const int MaxPushedValueExclusive = 10;

    private List<Func<ImplementQueueUsingStacksSolution.BclTwoStackQueue, int?>> _bclScript = new();

    private List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>> _script = new();

    // Every value pop and peek answer, in order. How many the random script holds is
    // not known up front, so this is a list given the most it can need in setup and
    // cleared by each replay.
    private List<int> _answers = [];

    [Params(10, 100)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var steps = BuildSteps(OperationCount, new Random(Seed));
        _bclScript = BclScriptOf(steps);
        _script = ScriptOf(steps);
        _answers = new List<int>(OperationCount);
    }

    private static List<ScriptStep> BuildSteps(int operationCount, Random random)
    {
        var steps = new List<ScriptStep>();
        var pending = 0;

        for (var i = 0; i < operationCount; i++)
        {
            pending = AppendNextStep(steps, pending, random);
        }

        return steps;
    }

    // One step, plus the pending-push count it leaves behind. roll == 0 always pushes,
    // drawing the value it pushes; rolls 1/2 peek or pop, but only once something has
    // actually been pushed and not yet popped.
    private static int AppendNextStep(List<ScriptStep> steps, int pending, Random random)
    {
        var roll = pending > 0 ? random.Next(0, 3) : 0;

        if (roll == 0)
        {
            var value = random.Next(MinPushedValue, MaxPushedValueExclusive);
            steps.Add(new ScriptStep(StepKind.Push, value));

            return pending + 1;
        }

        if (roll == 1)
        {
            steps.Add(new ScriptStep(StepKind.Peek, 0));

            return pending;
        }

        steps.Add(new ScriptStep(StepKind.Pop, 0));

        return pending - 1;
    }

    private static List<Func<ImplementQueueUsingStacksSolution.BclTwoStackQueue, int?>> BclScriptOf(
        List<ScriptStep> steps) =>
        [.. steps.Select(BclEntryOf)];

    private static List<Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?>> ScriptOf(List<ScriptStep> steps) =>
        [.. steps.Select(EntryOf)];

    private static Func<ImplementQueueUsingStacksSolution.BclTwoStackQueue, int?> BclEntryOf(ScriptStep step) =>
        step.Kind switch
        {
            StepKind.Push => queue =>
            {
                queue.Push(step.Value);
                return null;
            },
            StepKind.Peek => queue => queue.Peek(),
            _ => queue => queue.Pop(),
        };

    private static Func<ImplementQueueUsingStacksSolution.TwoStackQueue, int?> EntryOf(ScriptStep step) =>
        step.Kind switch
        {
            StepKind.Push => queue =>
            {
                queue.Push(step.Value);
                return null;
            },
            StepKind.Peek => queue => queue.Peek(),
            _ => queue => queue.Pop(),
        };

    [Benchmark(Baseline = true)]
    public List<int> TwoBclStackTransfer()
    {
        var queue = ImplementQueueUsingStacksSolution.CreateByTwoBclStacks();
        _answers.Clear();

        foreach (var op in _bclScript)
        {
            if (op(queue) is { } answer)
            {
                _answers.Add(answer);
            }
        }

        return _answers;
    }

    [Benchmark]
    public List<int> TwoStackTransfer()
    {
        var queue = ImplementQueueUsingStacksSolution.CreateByTwoStacks();
        _answers.Clear();

        foreach (var op in _script)
        {
            if (op(queue) is { } answer)
            {
                _answers.Add(answer);
            }
        }

        return _answers;
    }

    private enum StepKind
    {
        Push,
        Peek,
        Pop,
    }

    // One call in the drawn sequence, and the value it pushes when it is a push.
    private readonly record struct ScriptStep(StepKind Kind, int Value);
}
