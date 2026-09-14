using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestUploadedPrefix;

// LeetCode 2424. Longest Uploaded Prefix: a stream of n videos numbered 1..n
// arrives in any order, and after each upload the server reports the largest k such
// that videos 1..k have all arrived.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared IUploadedPrefix surface below, the same shape
// SeatReservationManagerSolution uses for its own instance-API problem (LC 1845).
//
// The whole problem is where the prefix length comes from: rescanned from video 1
// on every query, or carried as a frontier that only ever moves forward.
internal static class LongestUploadedPrefixSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IUploadedPrefix
    {
        void Upload(int video);

        int Longest();
    }

    // The textbook baseline this composition has to justify itself against: one
    // bool per video and a rescan from video 1 on every query, so the walk past the
    // already-uploaded prefix is repeated in full each time - O(n) per query, O(n^2)
    // over a whole stream. Deliberately written without this repo's Set<int>.
    //
    // This is the arm that needs LeetCode's n at all: it sizes its array from the
    // stream's declared capacity up front.
    internal sealed class UploadedPrefixByRescanArray(int videoCount) : IUploadedPrefix
    {
        private readonly bool[] _uploaded = new bool[videoCount + 1];

        public void Upload(int video) => _uploaded[video] = true;

        public int Longest()
        {
            var longest = 0;

            while (longest + 1 < _uploaded.Length && _uploaded[longest + 1])
            {
                longest++;
            }

            return longest;
        }
    }

    // The composed answer: this repo's own Set<int> (backed by HashMap) records
    // which videos have arrived, and a frontier advances past every already-seen
    // number immediately after each upload. The frontier never moves backwards, so
    // the total advancing work across a whole stream is O(n) - O(1) amortized per
    // upload - and Longest is a field read.
    //
    // Membership is the only question asked of the store, and only ever about one
    // specific next value, so no ordered or range structure is needed. LeetCode's n
    // plays no part in correctness here, which is why this strategy never asks for
    // it.
    internal sealed class UploadedPrefixBySetFrontier : IUploadedPrefix
    {
        private readonly Set<int> _uploaded = new();
        private int _longest;

        public void Upload(int video)
        {
            _uploaded.TryAdd(video);

            while (_uploaded.Has(_longest + 1))
            {
                _longest++;
            }
        }

        public int Longest() => _longest;
    }
}
