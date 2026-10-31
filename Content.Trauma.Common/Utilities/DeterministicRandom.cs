using Robust.Shared.Random;

namespace Content.Trauma.Common.Utilities;

/// <summary>
/// Saved state of a <see cref="DeterministicRandom"/>
/// </summary>
[DataRecord]
public readonly partial record struct DeterministicRandomState(
    ulong State,
    ulong Increment
);

/// <summary>
/// A deterministic random implementation that allows to preserve its
/// state in a <see cref="DeterministicRandomState"/> struct.
/// Adapted from
/// </summary>
/// <remarks>
/// The implementation is adapted from PCG32 deterministic random function.
/// </remarks>
[DataDefinition, Serializable, NetSerializable]
public sealed partial class DeterministicRandom : IRobustRandom
{
    [DataField("state")]
    private ulong _state;

    [DataField("increment")]
    private ulong _increment;

    public DeterministicRandom(ulong seed, ulong stream = 54)
    {
        _state = 0;
        _increment = (stream << 1) | 1UL;

        NextUInt32();
        _state = unchecked(_state + seed);
        NextUInt32();
    }

    private DeterministicRandom(DeterministicRandomState snapshot)
    {
        if ((snapshot.Increment & 1UL) == 0)
        {
            throw new ArgumentException(
                "The snapshot has an invalid increment.",
                nameof(snapshot));
        }

        _state = snapshot.State;
        _increment = snapshot.Increment;
    }

    public DeterministicRandomState CaptureState()
    {
        return new DeterministicRandomState(_state, _increment);
    }

    public static DeterministicRandom Restore(DeterministicRandomState snapshot)
    {
        return new DeterministicRandom(snapshot);
    }

    private uint NextUInt32()
    {
        var oldState = _state;

        _state = unchecked(
            oldState * 6364136223846793005UL + _increment);

        var xorShifted = unchecked(
            (uint) (((oldState >> 18) ^ oldState) >> 27));

        var rotation = (int) (oldState >> 59);

        return (xorShifted >> rotation) |
               (xorShifted << (-rotation & 31));
    }

    private ulong NextUInt64()
    {
        return ((ulong) NextUInt32() << 32) | NextUInt32();
    }

    private ulong NextUInt64(ulong exclusiveMax)
    {
        ArgumentOutOfRangeException.ThrowIfZero(exclusiveMax);

        // Rejection sampling avoids modulo bias.
        var threshold = unchecked(0UL - exclusiveMax) % exclusiveMax;

        ulong value;
        do
        {
            value = NextUInt64();
        } while (value < threshold);

        return value % exclusiveMax;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// WARNING: Use this only if you absolutely have to.
    /// This method adds a lot of overhead, so use IRobustRandom and put
    /// the <see cref="DeterministicRandom"/> instance in it instead if you can.
    /// </remarks>
    public Random GetRandom()
    {
        return new RandomAdapter(this);
    }

    /// <inheritdoc/>
    public void SetSeed(int seed)
    {
        var unsignedSeed = unchecked((ulong) seed);

        var seeded = new DeterministicRandom(
            unsignedSeed,
            _increment >> 1);

        var state = seeded.CaptureState();

        _state = state.State;
    }

    /// <inheritdoc/>
    public double NextDouble()
    {
        var a = NextUInt32() >> 5;
        var b = NextUInt32() >> 6;

        return (a * 67108864.0 + b) /
               9007199254740992.0;
    }

    private int NextInt(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxExclusive),
                "Must be greater than minInclusive.");
        }

        var range = (uint) ((long) maxExclusive - minInclusive);
        var threshold = unchecked(0u - range) % range;

        uint value;
        do
        {
            value = NextUInt32();
        } while (value < threshold);

        return (int) (minInclusive + value % range);
    }

    /// <inheritdoc/>
    public float NextFloat()
    {
        return (NextUInt32() >> 8) * (1.0f / 16777216.0f);
    }

    /// <inheritdoc/>
    public int Next()
    {
        return NextInt(0, int.MaxValue);
    }

    /// <inheritdoc/>
    public int Next(int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxValue);
        return maxValue == 0 ? 0 : NextInt(0, maxValue);
    }

    /// <inheritdoc/>
    public int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxValue),
                "Must be greater than or equal to minValue.");
        }

        return minValue == maxValue ? minValue : NextInt(minValue, maxValue);
    }

    /// <inheritdoc/>
    public long NextLong()
    {
        return NextLong(0, long.MaxValue);
    }

    /// <inheritdoc/>
    public long NextLong(long maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxValue);

        if (maxValue == 0)
            return 0;

        return (long) NextUInt64((ulong) maxValue);
    }

    /// <inheritdoc/>
    public long NextLong(long minValue, long maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxValue),
                "Must be greater than or equal to minValue.");
        }

        if (minValue == maxValue)
            return minValue;

        // Unsigned subtraction correctly represents the entire range,
        // even when the difference exceeds long.MaxValue.
        var range = unchecked(
            (ulong) maxValue - (ulong) minValue);

        var offset = NextUInt64(range);

        return unchecked((long) ((ulong) minValue + offset));
    }

    /// <inheritdoc/>
    public TimeSpan Next(TimeSpan minTime, TimeSpan maxTime)
    {
        if (minTime > maxTime)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxTime),
                "Must be greater than or equal to minTime.");
        }

        if (minTime == maxTime)
            return minTime;

        return minTime + (maxTime - minTime) * NextDouble();
    }

    /// <inheritdoc/>
    public TimeSpan Next(TimeSpan maxTime)
    {
        return Next(TimeSpan.Zero, maxTime);
    }

    /// <inheritdoc/>
    public void NextBytes(byte[] buffer)
    {
        NextBytes(buffer.AsSpan());
    }

    public void NextBytes(Span<byte> buffer)
    {
        var i = 0;

        while (i < buffer.Length)
        {
            var value = NextUInt32();

            for (int shift = 0; shift < 32 && i < buffer.Length; shift += 8)
            {
                buffer[i++] = (byte) (value >> shift);
            }
        }
    }

    /// <summary>
    /// Adapts this deterministic random to System.Random.
    /// </summary>
    private sealed class RandomAdapter(DeterministicRandom source) : Random
    {
        protected override double Sample()
        {
            return source.NextDouble();
        }

        public override int Next()
        {
            return source.Next();
        }

        public override int Next(int maxValue)
        {
            return source.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            return source.Next(minValue, maxValue);
        }

        public override long NextInt64()
        {
            return source.NextLong();
        }

        public override long NextInt64(long maxValue)
        {
            return source.NextLong(maxValue);
        }

        public override long NextInt64(long minValue, long maxValue)
        {
            return source.NextLong(minValue, maxValue);
        }

        public override double NextDouble()
        {
            return source.NextDouble();
        }

        public override float NextSingle()
        {
            return source.NextFloat();
        }

        public override void NextBytes(byte[] buffer)
        {
            source.NextBytes(buffer);
        }

        public override void NextBytes(Span<byte> buffer)
        {
            source.NextBytes(buffer);
        }
    }
}
