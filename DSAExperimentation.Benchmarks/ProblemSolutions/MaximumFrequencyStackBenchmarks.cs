using DSAExperimentation.LeetCode.MaximumFrequencyStack;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumFrequencyStackSolution's factories, the same ones
// MaximumFrequencyStackSolutionTests proves correct. A naive List<int>-backed stack that finds
// the value to pop by rescanning the whole list every time (recount every value's
// frequency, then walk back from the top for the most-recent max-frequency entry - O(n)
// per pop, O(n^2) over a full push/pop sequence) vs. this repo's own
// HashMap<TKey,TValue> tracking each value's running push frequency plus a second
// HashMap of frequency -> Stack<int> holding the values pushed at that frequency, so
// Pop is O(1) amortized. [GlobalSetup] builds the whole push/pop script so script
// construction is charged to setup and only the replay is measured.
public class MaximumFrequencyStackBenchmarks
{
    private const int RandomSeed = 7;
    private const double PushProbability = 0.6;
    private const int ValueRange = 50;

    private (bool IsPush, int Value)[] _operations = [];

    // What every Pop returned, in script order - what each arm returns; a Push returns nothing.
    private int[] _popped = [];

    [Params(200, 3_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var operations = new (bool IsPush, int Value)[OperationCount];
        var depth = 0;

        for (var i = 0; i < OperationCount; i++)
        {
            // Push-biased so a pop always has something to remove - depth==0 forces
            // a push, otherwise pushes still outweigh pops, the same push-heavy
            // shape TopKFrequentElementsBenchmarks' bounded-range setup documents
            // for producing real frequency skew.
            var isPush = depth == 0 || random.NextDouble() < PushProbability;
            operations[i] = (isPush, random.Next(0, ValueRange));
            depth += isPush ? 1 : -1;
        }

        _operations = operations;
        _popped = new int[operations.Count(operation => !operation.IsPush)];
    }

    [Benchmark(Baseline = true)]
    public int[] RescanListOnEveryPop() => Replay(MaximumFrequencyStackSolution.CreateByListRescan());

    [Benchmark]
    public int[] HashMapAndStackByFrequency() => Replay(MaximumFrequencyStackSolution.CreateByHashMapAndStack());

    private int[] Replay(MaximumFrequencyStackSolution.IFreqStack freqStack)
    {
        var popCount = 0;

        foreach (var (isPush, value) in _operations)
        {
            if (isPush)
            {
                freqStack.Push(value);
            }
            else
            {
                _popped[popCount++] = freqStack.Pop();
            }
        }

        return _popped;
    }
}
