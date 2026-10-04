using DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

namespace DSAExperimentation.LeetCode.Tests.ImplementRand10UsingRand7;

// Harness only: both strategies live in ImplementRand10UsingRand7Solution. A seeded
// System.Random stands in for the black-box Rand7() the real LeetCode judge
// supplies (this suite's established randomness-problem convention - see
// ShuffleAnArrayTests, RandomPickIndexTests, LinkedListRandomNodeTests).
//
// Both strategies are supposed to be uniform, so both are held to the same claims:
// LeetCode's published examples, the range, a seeded uniformity check, and an exact
// one - every sequence of four Rand7() faces played in turn, each equally likely, so
// a strategy is uniform exactly when the sequences it finishes on split evenly over
// 1..10. The expected splits and average call counts are derived by hand in the
// comments beside them.
public sealed partial class ImplementRand10UsingRand7SolutionTests
{
    // The sample size the seeded tests draw. A constant rather than a local: its
    // scope is a claim about where the value is authoritative, and the tolerance bands
    // the tests assert against are chosen from this magnitude, not from the loop.
    private const int Trials = 20_000;

    private const int DieFaces = 7;
    private const int Rand10Maximum = 10;

    // Every sequence of four faces: 7^4 of them.
    private const int ScriptLength = 4;
    private const int ScriptCount = 2_401;

    // LeetCode's published examples. n is how many times the judge calls rand10(),
    // and the output is one draw of that many values - any values in 1..10 would do -
    // so each example asserts n values, each in range, as each published value is.
    public static TheoryData<int, int[]> Examples =>
        new()
        {
            { 1, [2] },
            { 2, [2, 8] },
            { 3, [3, 8, 10] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Rand10ByRejectionSampling_LeetCodeExamples_ReturnsOneValueInRangePerCall(int calls, int[] published) =>
        AssertOneValueInRangePerCall(ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling, calls, published);

    [Theory]
    [MemberData(nameof(Examples))]
    public void Rand10ByRecycledRejectionSampling_LeetCodeExamples_ReturnsOneValueInRangePerCall(int calls, int[] published) =>
        AssertOneValueInRangePerCall(ImplementRand10UsingRand7Solution.Rand10ByRecycledRejectionSampling, calls, published);

    [Fact]
    public void Rand10ByRejectionSampling_ManyCalls_AlwaysStaysInRange() =>
        AssertAlwaysInRange(ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling);

    [Fact]
    public void Rand10ByRecycledRejectionSampling_ManyCalls_AlwaysStaysInRange() =>
        AssertAlwaysInRange(ImplementRand10UsingRand7Solution.Rand10ByRecycledRejectionSampling);

    [Fact]
    public void Rand10ByRejectionSampling_ManyCalls_EventuallyReturnsEveryValueRoughlyUniformly() =>
        AssertRoughlyUniform(ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling);

    [Fact]
    public void Rand10ByRecycledRejectionSampling_ManyCalls_EventuallyReturnsEveryValueRoughlyUniformly() =>
        AssertRoughlyUniform(ImplementRand10UsingRand7Solution.Rand10ByRecycledRejectionSampling);

    // The first two faces finish on 40 of their 49 cells, 4 per value, whatever the
    // last two are: 4 * 49 = 196 sequences per value. The other 9 cells retry on the
    // last two faces, which finish on 4 cells per value: 9 * 4 = 36 more, 232 per
    // value. Both pairs miss in 9 * 9 = 81 sequences, which need a fifth face.
    [Fact]
    public void Rand10ByRejectionSampling_EveryFourFaceSequence_SplitsTheFinishedOnesEvenly()
    {
        var outcomes = OutcomesOverEveryFourFaceSequence(ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling);
        var finishedOnEachValue = outcomes.FinishedOn[1..];

        Assert.All(finishedOnEachValue, sequences => Assert.Equal(232, sequences));
        Assert.Equal(81, outcomes.Unfinished);
    }

    // The first two faces finish on 40 of 49 cells: 4 * 49 = 196 per value. The 9
    // cells past 40 and the third face make 63 cells, 60 of which finish, 6 per value,
    // whatever the fourth face is: 6 * 7 = 42 more. The 3 cells past 60 and the fourth
    // face make 21 cells, 20 of which finish: 2 more, 240 per value. One sequence in
    // 2401 needs a fifth face, against the plain sampler's 81.
    [Fact]
    public void Rand10ByRecycledRejectionSampling_EveryFourFaceSequence_SplitsTheFinishedOnesEvenly()
    {
        var outcomes = OutcomesOverEveryFourFaceSequence(ImplementRand10UsingRand7Solution.Rand10ByRecycledRejectionSampling);
        var finishedOnEachValue = outcomes.FinishedOn[1..];

        Assert.All(finishedOnEachValue, sequences => Assert.Equal(240, sequences));
        Assert.Equal(1, outcomes.Unfinished);
    }

    // Each try costs two draws and succeeds with probability 40/49, so a call averages
    // 2 * 49/40 = 2.45 draws. The standard deviation of one call's count is about 1.05,
    // so the mean over 20,000 calls strays from 2.45 by about 0.0074; 0.05 is
    // over six of those.
    [Fact]
    public void Rand10ByRejectionSampling_ManyCalls_AveragesTwoAndNineTwentiethsDraws() =>
        Assert.InRange(AverageDrawsPerCall(ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling), 2.40, 2.50);

    // A call always draws twice, draws a third time on the 9/49 that miss, and a
    // fourth on the 3/63 of those that miss again; the 1/21 of those that miss a
    // third time start over. So E = 2 + 9/49 + (9/49)(3/63) + E/2401, which solves to
    // E = (752/343)(2401/2400) = 5264/2400, about 2.193 - under the plain sampler's 2.45,
    // the point of LeetCode's follow-up.
    [Fact]
    public void Rand10ByRecycledRejectionSampling_ManyCalls_AveragesUnderThePlainSamplersDraws() =>
        Assert.InRange(AverageDrawsPerCall(ImplementRand10UsingRand7Solution.Rand10ByRecycledRejectionSampling), 2.14, 2.24);

    private static void AssertOneValueInRangePerCall(Func<IRand7, int> rand10, int calls, int[] published)
    {
        var rand7 = new SeededRandomRand7(new Random(1));
        var drawn = Enumerable.Range(0, calls).Select(_ => rand10(rand7)).ToArray();

        Assert.Equal(published.Length, drawn.Length);
        Assert.All(drawn.Concat(published), value => Assert.InRange(value, 1, Rand10Maximum));
    }

    private static void AssertAlwaysInRange(Func<IRand7, int> rand10)
    {
        var rand7 = new SeededRandomRand7(new Random(1));

        for (var i = 0; i < 2_000; i++)
        {
            Assert.InRange(rand10(rand7), 1, Rand10Maximum);
        }
    }

    private static void AssertRoughlyUniform(Func<IRand7, int> rand10)
    {
        var rand7 = new SeededRandomRand7(new Random(1));
        var counts = new int[Rand10Maximum + 1];

        for (var i = 0; i < Trials; i++)
        {
            counts[rand10(rand7)]++;
        }

        for (var value = 1; value <= Rand10Maximum; value++)
        {
            // Expected count per value is trials/10 = 2000; a wide tolerance keeps
            // this deterministic-seed test robust to the rejection sampler's own
            // variance.
            Assert.InRange(counts[value], 1_500, 2_500);
        }
    }

    private static double AverageDrawsPerCall(Func<IRand7, int> rand10)
    {
        var rand7 = new SeededRandomRand7(new Random(1));

        for (var i = 0; i < Trials; i++)
        {
            rand10(rand7);
        }

        return (double)rand7.Draws / Trials;
    }

    // Plays each of the 2401 sequences of four faces once, in a fresh source, and
    // records which value the strategy finished on, or that it outran the sequence.
    private static FaceSequenceOutcomes OutcomesOverEveryFourFaceSequence(Func<IRand7, int> rand10)
    {
        var finishedOn = new int[Rand10Maximum + 1];
        var unfinished = 0;

        for (var sequence = 0; sequence < ScriptCount; sequence++)
        {
            var faces = FacesOf(sequence);

            if (FinishedValue(rand10, faces) is { } value)
            {
                finishedOn[value]++;
            }
            else
            {
                unfinished++;
            }
        }

        return new FaceSequenceOutcomes(finishedOn, unfinished);
    }

    // The value a strategy returns on one sequence of faces, or null when it needed
    // more faces than the sequence holds.
    private static int? FinishedValue(Func<IRand7, int> rand10, int[] faces)
    {
        var rand7 = new ScriptedRand7(faces);
        var value = rand10(rand7);

        return rand7.OutranScript ? null : value;
    }

    // The sequence's number written in base 7, one digit per face, so 0..2400 name
    // every sequence of four faces exactly once.
    private static int[] FacesOf(int sequence)
    {
        var faces = new int[ScriptLength];
        var rest = sequence;

        for (var position = 0; position < ScriptLength; position++)
        {
            faces[position] = 1 + (rest % DieFaces);
            rest /= DieFaces;
        }

        return faces;
    }

    // How the 2401 four-face sequences ended: FinishedOn[v] counts those a strategy
    // returned v on, and Unfinished those it needed a fifth face for.
    private readonly record struct FaceSequenceOutcomes(int[] FinishedOn, int Unfinished);

    // The stand-in for LeetCode's black-box Rand7(): a seeded System.Random drawn
    // from per call, counting its draws. One type serves every seeded test here, so
    // the seed stays the only thing a test picks.
    private sealed class SeededRandomRand7(Random random) : IRand7
    {
        public int Draws { get; private set; }

        public int Draw()
        {
            Draws++;

            return random.Next(1, DieFaces + 1);
        }
    }

    // Plays one fixed sequence of faces in order. Past its end it answers 1, a face
    // on which both strategies finish within two more draws, so a call that outruns
    // the sequence still returns, and OutranScript says its value came from faces the
    // sequence never held.
    private sealed class ScriptedRand7(int[] faces) : IRand7
    {
        private const int FaceBothStrategiesFinishOn = 1;

        private int _drawn;

        public bool OutranScript => _drawn > faces.Length;

        public int Draw()
        {
            var position = _drawn;
            _drawn++;

            if (position >= faces.Length)
            {
                return FaceBothStrategiesFinishOn;
            }

            return faces[position];
        }
    }
}
