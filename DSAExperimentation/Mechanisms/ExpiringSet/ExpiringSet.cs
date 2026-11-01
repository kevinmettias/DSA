using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Mechanisms.ExpiringSet;

// Keys that lapse a fixed time after their life last started, counted at a moment the caller names.
// A mechanism under ARCHITECTURE.md §17.6b: a key leaves when its time-to-live runs out, which no
// caller operation names. Time is passed in (§17.6c) and must not run backwards, and with a fixed
// time-to-live the order lives start in is the order they end in. A queue of (key, expiry) in start
// order therefore holds every life in expiry order: counting pops the lives that have ended, skips
// any a later Set or renewal has replaced, and lets their keys go - amortized O(1) per call and no
// allocation, where reading every key's expiry costs O(n). A key is alive while its expiry is later
// than now, so a life ending at t is over for any call at t.
/// <summary>A set of keys that each lapse a fixed time after their life last started.</summary>
public sealed class ExpiringSet<TKey>
    where TKey : notnull
{
    private const string TimeRunsBackwardsMessage = "Time must not run backwards.";

    private readonly HashMap<TKey, int> _expiryByKey = new();
    private readonly DataStructures.Queue.Queue<(TKey Key, int Expiry)> _livesByExpiry = new();
    private readonly int _timeToLive;
    private int _latest = int.MinValue;

    /// <summary>Creates an empty set whose keys each live for a fixed time.</summary>
    /// <param name="timeToLive">How long a key lives after its life starts, at least 1.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeToLive"/> is below 1.</exception>
    public ExpiringSet(int timeToLive)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeToLive, 1);
        _timeToLive = timeToLive;
    }

    /// <summary>Starts a key's life now, whether or not it was alive.</summary>
    /// <param name="key">The key.</param>
    /// <param name="now">The current time, no earlier than any time named before.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="now"/> is earlier than a time named before.</exception>
    public void Set(TKey key, int now)
    {
        Advance(now);
        StartLife(key, now);
    }

    /// <summary>Starts a living key's life again from now; a key that is unknown or has lapsed stays out.</summary>
    /// <param name="key">The key.</param>
    /// <param name="now">The current time, no earlier than any time named before.</param>
    /// <returns><see langword="true"/> when the key was alive and its life started again.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="now"/> is earlier than a time named before.</exception>
    public bool TryRenew(TKey key, int now)
    {
        Advance(now);

        if (!_expiryByKey.TryGetValue(key, out var expiry) || expiry <= now)
        {
            return false;
        }

        StartLife(key, now);
        return true;
    }

    /// <summary>Counts the keys still alive now, letting every lapsed one go.</summary>
    /// <param name="now">The current time, no earlier than any time named before.</param>
    /// <returns>The number of keys whose life has not yet ended.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="now"/> is earlier than a time named before.</exception>
    public int CountAlive(int now)
    {
        Advance(now);

        while (_livesByExpiry.TryPeek(out var life) && life.Expiry <= now)
        {
            _livesByExpiry.TryDequeue(out _);

            if (_expiryByKey.TryGetValue(life.Key, out var expiry) && expiry == life.Expiry)
            {
                _expiryByKey.TryRemove(life.Key);
            }
        }

        return _expiryByKey.Count;
    }

    // Time runs forwards: a call naming an earlier moment than one before it is the caller's bug, and
    // the queue's expiry order depends on it.
    private void Advance(int now)
    {
        if (now < _latest)
        {
            throw new ArgumentOutOfRangeException(nameof(now), now, TimeRunsBackwardsMessage);
        }

        _latest = now;
    }

    private void StartLife(TKey key, int now)
    {
        var expiry = now + _timeToLive;

        _expiryByKey.Set(key, expiry);
        _livesByExpiry.Enqueue((key, expiry));
    }
}
