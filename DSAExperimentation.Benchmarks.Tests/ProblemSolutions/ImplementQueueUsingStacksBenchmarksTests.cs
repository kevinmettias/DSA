using DSAExperimentation.Benchmarks.ProblemSolutions;
using BclQueue = System.Collections.Generic.Queue<int>;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementQueueUsingStacksBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm - this repo's two-stack queue - so there is no second strategy to reconcile it
// against and the assertion has to come from the arm's own declared contract instead. Its
// [GlobalSetup] builds one fixed, valid script of interleaved push/pop/peek entries (a pop or
// peek only ever follows a push that has not yet been popped) and the arm replays that script
// through the two-stack queue, returning every value pop and peek answered. A FIFO queue's
// contract fixes those answers: the reference replay below drives the identical script - same
// seed, same roll and value draws, same order - through a BCL Queue<int>, so the two sequences
// agree only if the measured queue is genuinely FIFO.
public sealed partial class ImplementQueueUsingStacksBenchmarksTests
{
    private const int SmallestOperationCount = 10;

    // The seed [GlobalSetup] builds its script from, restated here because the script itself is
    // private to the harness.
    private const int ScriptSeed = 232;

    // The script's three rolls: 0 pushes the next value, 1 peeks, 2 pops.
    private const int OperationKindCount = 3;

    // The band [GlobalSetup] draws pushed values from: LC 232's [1, 9].
    private const int MinPushedValue = 1;
    private const int MaxPushedValueExclusive = 10;

    [Fact]
    public void TwoStackTransfer_InterleavedScript_MatchesBclQueueReference()
    {
        var harness = BuildHarness();

        Assert.Equal(ReferenceFifoAnswers(SmallestOperationCount), harness.TwoStackTransfer());
    }

    private static ImplementQueueUsingStacksBenchmarks BuildHarness()
    {
        var harness = new ImplementQueueUsingStacksBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }

    // The script the harness builds, replayed through a BCL Queue<int> instead of the two-stack
    // queue: roll 0 pushes the next value, roll 1 peeks, roll 2 pops - and while nothing is
    // pending the roll is forced to a push without drawing, exactly as the harness does.
    private static List<int> ReferenceFifoAnswers(int operationCount)
    {
        var random = new Random(ScriptSeed);
        var reference = new BclQueue();
        var answers = new List<int>();
        var pending = 0;

        for (var i = 0; i < operationCount; i++)
        {
            var roll = pending > 0 ? random.Next(0, OperationKindCount) : 0;

            if (roll == 0)
            {
                reference.Enqueue(random.Next(MinPushedValue, MaxPushedValueExclusive));
                pending++;
            }
            else if (roll == 1)
            {
                answers.Add(reference.Peek());
            }
            else
            {
                answers.Add(reference.Dequeue());
                pending--;
            }
        }

        return answers;
    }
}
