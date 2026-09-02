using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Index, int Remaining)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Time Needed to Buy Tickets (LC 2073): this repo's own Queue<(int,int)> directly
// simulating the line (TimeNeededToBuyTicketsTests precedent) - O(total tickets
// purchased before person k finishes) - vs. the O(n) closed-form sum (everyone at
// or ahead of k contributes min(their tickets, k's tickets); everyone behind k
// contributes min(their tickets, k's tickets - 1)). _tickets uses a large, uniform
// ticket count per person so the queue simulation is forced through its full
// O(n * ticketsPerPerson) worst case instead of finishing after a handful of turns.
[MemoryDiagnoser]
public class TimeNeededToBuyTicketsBenchmarks
{
    private const int RandomSeed = 2073; // LC problem number
    private const int MinTickets = 500;
    private const int MaxTicketsExclusive = 1_000;
    private const int MidpointDivisor = 2;

    [Params(50, 300)]
    public int Length;

    private int[] _tickets = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _tickets = Enumerable.Range(0, Length).Select(_ => random.Next(MinTickets, MaxTicketsExclusive)).ToArray();
        _k = Length / MidpointDivisor;
    }

    [Benchmark(Baseline = true)]
    public int QueueSimulation()
    {
        var line = new RepoQueue();

        for (var i = 0; i < _tickets.Length; i++)
        {
            line.Enqueue((i, _tickets[i]));
        }

        var time = 0;

        while (line.TryDequeue(out var person))
        {
            time++;

            if (ProcessTurn(line, person))
            {
                break;
            }
        }

        return time;
    }

    private bool ProcessTurn(RepoQueue line, (int Index, int Remaining) person)
    {
        var remaining = person.Remaining - 1;

        if (remaining == 0 && person.Index == _k)
        {
            return true;
        }

        if (remaining > 0)
        {
            line.Enqueue((person.Index, remaining));
        }

        return false;
    }

    [Benchmark]
    public int ClosedFormSum()
    {
        var target = _tickets[_k];
        var time = 0;

        for (var i = 0; i < _tickets.Length; i++)
        {
            time += i <= _k ? Math.Min(_tickets[i], target) : Math.Min(_tickets[i], target - 1);
        }

        return time;
    }
}
